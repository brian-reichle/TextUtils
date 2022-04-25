// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
#if NET6_0_OR_GREATER
using System;

namespace TextTools.Test.TestUtils
{
	sealed class DummySpanFormattable : ISpanFormattable
	{
		public DummySpanFormattable(char c, int length, string expectedFormat = "")
		{
			_c = c;
			_length = length;
			_expectedFormat = expectedFormat;
		}

		public bool TryFormat(Span<char> destination, out int charsWritten, ReadOnlySpan<char> format, IFormatProvider? provider)
		{
			if (!format.Equals(_expectedFormat.AsSpan(), StringComparison.Ordinal))
			{
				throw new ArgumentException("incorrect format");
			}

			if (destination.Length < _length)
			{
				charsWritten = 0;
				return false;
			}

			destination = destination.Slice(0, _length);

			for (var i = 0; i < destination.Length; i++)
			{
				destination[i] = _c;
			}

			charsWritten = destination.Length;
			return true;
		}

		string IFormattable.ToString(string? format, IFormatProvider? formatProvider) => throw new NotImplementedException();

		readonly char _c;
		readonly int _length;
		readonly string _expectedFormat;
	}
}
#endif
