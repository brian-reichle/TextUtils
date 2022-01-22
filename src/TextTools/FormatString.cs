// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace TextTools
{
	public static class FormatString
	{
		public static bool ContainsEscapes(ReadOnlySpan<char> formatString, out int unescapedLength)
		{
			unescapedLength = formatString.Length;
			ReadOnlySpan<char> braces = stackalloc char[] { '{', '}' };

			var i = formatString.IndexOfAny(braces);

			if (i < 0)
			{
				return false;
			}

			do
			{
				if (i + 1 >= formatString.Length || formatString[i + 1] != formatString[i])
				{
					InvalidFormat();
				}

				formatString = formatString.Slice(i + 2);
				unescapedLength--;
				i = formatString.IndexOfAny(braces);
			}
			while (i >= 0);

			return true;
		}

		public static int WriteUnescaped(ReadOnlySpan<char> formatString, Span<char> unescaped)
		{
			ReadOnlySpan<char> braces = stackalloc char[] { '{', '}' };

			var i = formatString.IndexOfAny(braces);
			var length = 0;

			while (i >= 0)
			{
				if (i + 1 >= formatString.Length || formatString[i + 1] != formatString[i])
				{
					InvalidFormat();
				}

				formatString.Slice(0, i + 1).CopyTo(unescaped);
				length += i + 1;
				formatString = formatString.Slice(i + 2);
				unescaped = unescaped.Slice(i + 1);

				i = formatString.IndexOfAny(braces);
			}

			formatString.CopyTo(unescaped);
			length += formatString.Length;

			return length;
		}

		public static string Unescape(ReadOnlySpan<char> formatString)
		{
			if (!ContainsEscapes(formatString, out var unescapedLength))
			{
				return formatString.ToString();
			}

			var buffer = ArrayPool<char>.Shared.Rent(unescapedLength);

			try
			{
				var span = buffer.AsSpan(0, unescapedLength);
				WriteUnescaped(formatString, span);
				return span.ToString();
			}
			finally
			{
				ArrayPool<char>.Shared.Return(buffer);
			}
		}

		[DoesNotReturn]
		static void InvalidFormat() => throw new FormatException("Input string was not in a correct format.");
	}
}
