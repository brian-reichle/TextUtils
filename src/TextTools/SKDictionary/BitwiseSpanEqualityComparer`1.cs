// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;
using TextTools.Utils;

namespace TextTools
{
	public sealed class BitwiseSpanEqualityComparer<T> : ISpanEqualityComparer<T>
		where T : struct
	{
		public BitwiseSpanEqualityComparer()
			: this(Environment.TickCount)
		{
		}

		public BitwiseSpanEqualityComparer(int seed)
		{
			_seed = unchecked((uint)seed);
		}

		public bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
			=> MemoryExtensions.SequenceEqual(
				MemoryMarshal.Cast<T, byte>(x),
				MemoryMarshal.Cast<T, byte>(y));

		public int GetHashCode(ReadOnlySpan<T> obj)
			=> MurmurHash.Hash(_seed, MemoryMarshal.Cast<T, byte>(obj));

		readonly uint _seed;
	}
}
