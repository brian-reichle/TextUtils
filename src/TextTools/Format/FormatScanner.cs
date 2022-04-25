// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Diagnostics.CodeAnalysis;

namespace TextTools
{
	public ref struct FormatScanner
	{
		public FormatScanner(ReadOnlySpan<char> format)
		{
			_format = format;
			IsArgument = false;
			_index = 0;
			Text = ReadOnlySpan<char>.Empty;
			ArgumentFormat = ReadOnlySpan<char>.Empty;
			ArgumentAlignment = ReadOnlySpan<char>.Empty;
		}

		public bool IsArgument { get; set; }
		public ReadOnlySpan<char> Text { get; set; }
		public ReadOnlySpan<char> ArgumentFormat { get; set; }
		public ReadOnlySpan<char> ArgumentAlignment { get; set; }

		public bool MoveNext()
		{
			if (_index >= _format.Length)
			{
				IsArgument = false;
				Text = ReadOnlySpan<char>.Empty;
				ArgumentFormat = ReadOnlySpan<char>.Empty;
				ArgumentAlignment = ReadOnlySpan<char>.Empty;
				return false;
			}

			var c = _format[_index];

			if (c != '{' || (_index + 1 < _format.Length && _format[_index + 1] == '{'))
			{
				return MoveNextText();
			}
			else
			{
				return MoveNextArgument();
			}
		}

		static int ArgumentFormatLength(ReadOnlySpan<char> source)
		{
			ReadOnlySpan<char> braces = stackalloc char[] { '{', '}' };
			var result = 0;

			while (true)
			{
				var i = source.IndexOfAny(braces);

				if (i < 0)
				{
					return i;
				}

				if (i + 1 < source.Length && source[i + 1] == source[i])
				{
					result += i + 2;
					source = source.Slice(i + 2);
				}
				else if (source[i] == '}')
				{
					return result + i;
				}
				else
				{
					InvalidFormat();
				}
			}
		}

		[DoesNotReturn]
		static void InvalidFormat() => throw new FormatException("Input string was not in a correct format.");

		bool MoveNextText()
		{
			var remaining = _format.Slice(_index);
			var i = remaining.IndexOfAny(stackalloc char[] { '{', '}' });

			if (i < 0)
			{
				SetText(remaining);
				_index = _format.Length;
				return true;
			}

			var next = _index + i;

			if (i + 1 < remaining.Length && remaining[i + 1] == remaining[i])
			{
				i++;
				next += 2;
			}

			SetText(remaining.Slice(0, i));
			_index = next;

			return true;
		}

		bool MoveNextArgument()
		{
			var remaining = _format.Slice(_index);
			var c = remaining[0];
			var i = remaining.IndexOfAny(stackalloc char[] { ',', ':', '}' });

			if (i < 0)
			{
				InvalidFormat();
			}

			var arg = remaining.Slice(1, i - 1);

			c = remaining[i];
			_index += i + 1;

			ReadOnlySpan<char> alignment;

			if (c == ',')
			{
				remaining = remaining.Slice(i + 1);
				i = remaining.IndexOfAny(stackalloc char[] { ':', '}' });

				if (i < 0)
				{
					InvalidFormat();
				}

				_index += i + 1;
				alignment = remaining.Slice(0, i);
				c = remaining[i];
			}
			else
			{
				alignment = ReadOnlySpan<char>.Empty;
			}

			ReadOnlySpan<char> format;

			if (c == ':')
			{
				remaining = remaining.Slice(i + 1);
				i = ArgumentFormatLength(remaining);

				if (i < 0)
				{
					InvalidFormat();
				}

				_index += i + 1;
				format = remaining.Slice(0, i);
			}
			else
			{
				format = ReadOnlySpan<char>.Empty;
			}

			SetArgument(arg, format, alignment);
			return true;
		}

		void SetText(ReadOnlySpan<char> text)
		{
			Text = text;
			IsArgument = false;
			ArgumentFormat = ReadOnlySpan<char>.Empty;
			ArgumentAlignment = ReadOnlySpan<char>.Empty;
		}

		void SetArgument(ReadOnlySpan<char> arg, ReadOnlySpan<char> format, ReadOnlySpan<char> alignment)
		{
			Text = arg;
			IsArgument = true;
			ArgumentFormat = format;
			ArgumentAlignment = alignment;
		}

		readonly ReadOnlySpan<char> _format;
		int _index;
	}
}
