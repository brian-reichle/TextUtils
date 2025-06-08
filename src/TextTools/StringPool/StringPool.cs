// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using TextTools.Utils;

namespace TextTools
{
	public sealed class StringPool : ICloneable
	{
		public StringPool(int capacity = DefaultCapacity)
			: this(Environment.TickCount, capacity)
		{
		}

		public StringPool(int seed, int capacity = DefaultCapacity)
		{
			if (capacity < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(capacity), capacity, "Value cannot be negative.");
			}

			_seed = unchecked((uint)seed);
			capacity = Primes.GetPrime(capacity);

			_values = new string[capacity];
			_hashes = new int[capacity];
			_nextIndexes = new int[capacity];
			_firstIndexes = new int[capacity];
			ResetIndexes(_nextIndexes);
			ResetIndexes(_firstIndexes);
		}

		public bool TryGetString(string text, [NotNullWhen(true)] out string? value)
		{
			if (text == null)
			{
				throw new ArgumentNullException(nameof(text));
			}

			return TryGetString(text.AsSpan(), out value);
		}

		public bool TryGetString(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? value)
		{
			if (text.Length == 0)
			{
				value = string.Empty;
				return true;
			}

			return TryGetCore(text, Hash(text), out value);
		}

		public string GetString(string text)
		{
			if (text == null)
			{
				throw new NullReferenceException(nameof(text));
			}

			if (text.Length == 0)
			{
				return string.Empty;
			}

			var textSpan = text.AsSpan();
			var hash = Hash(textSpan);

			if (!TryGetCore(textSpan, hash, out var result))
			{
				result = text;
				Add(hash, result);
			}

			return result;
		}

		public string GetString(ReadOnlySpan<char> text)
		{
			if (text.Length == 0)
			{
				return string.Empty;
			}

			var hash = Hash(text);

			if (!TryGetCore(text, hash, out var result))
			{
				result = text.ToString();
				Add(hash, result);
			}

			return result;
		}

		public StringPool Clone() => new StringPool(this);

		object ICloneable.Clone() => Clone();

		StringPool(StringPool pool)
		{
			_seed = pool._seed;
			_count = pool._count;
			_values = CloneArray(pool._values);
			_hashes = CloneArray(pool._hashes);
			_nextIndexes = CloneArray(pool._nextIndexes);
			_firstIndexes = CloneArray(pool._firstIndexes);
		}

		static void ResetIndexes(int[] indexes)
		{
#if NET || NETSTANDARD2_1_OR_GREATER
			Array.Fill(indexes, -1);
#else
			for (var i = 0; i < indexes.Length; i++)
			{
				indexes[i] = -1;
			}
#endif
		}

		static T[] CloneArray<T>(T[] source)
		{
			var result = new T[source.Length];
			Array.Copy(source, result, source.Length);
			return result;
		}

		bool TryGetCore(ReadOnlySpan<char> text, int hash, [NotNullWhen(true)] out string? result)
		{
			var i = _firstIndexes[Index(hash)];

			while (true)
			{
				if (i < 0)
				{
					result = null;
					return false;
				}

				if (_hashes[i] == hash && _values[i].AsSpan().SequenceEqual(text))
				{
					result = _values[i];
					return true;
				}

				i = _nextIndexes[i];
			}
		}

		void Add(int hash, string value)
		{
			if (_count == _firstIndexes.Length)
			{
				GrowCapacity();
			}

			var id = _count;
			_nextIndexes[id] = -1;
			_values[id] = value;
			_hashes[id] = hash;
			_count++;

			LinkId(hash, id);
		}

		void LinkId(int hash, int id)
		{
			ref var i = ref _firstIndexes[Index(hash)];

			while (true)
			{
				if (i < 0)
				{
					i = id;
					return;
				}

				i = ref _nextIndexes[i];
			}
		}

		void GrowCapacity()
		{
			var newCapacity = Primes.GetPrime((_firstIndexes.Length * 2) + 1);
			Array.Resize(ref _values, newCapacity);
			Array.Resize(ref _hashes, newCapacity);

			_nextIndexes = new int[newCapacity];
			_firstIndexes = new int[newCapacity];
			ResetIndexes(_nextIndexes);
			ResetIndexes(_firstIndexes);

			for (var i = 0; i < _count; i++)
			{
				LinkId(_hashes[i], i);
			}
		}

		int Hash(ReadOnlySpan<char> text) => MurmurHash.Hash(_seed, MemoryMarshal.AsBytes(text));

		int Index(int hash) => unchecked((int)((uint)hash % (uint)_values.Length));

		const int DefaultCapacity = 100;

		readonly uint _seed;
		int _count;
		string[] _values;
		int[] _hashes;
		int[] _nextIndexes;
		int[] _firstIndexes;
	}
}
