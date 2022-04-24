// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

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

		public static string Format<T>(IFormatProvider? formatProvider, ReadOnlySpan<char> format, IFormatArgumentProvider<T> argumentProvider, T data)
		{
			if (argumentProvider == null)
			{
				throw new ArgumentNullException(nameof(argumentProvider));
			}

			return StringBuilderPool.ToStringAndReturn(AppendFormatCore(StringBuilderPool.Rent(), formatProvider, format, argumentProvider, data));
		}

		public static StringBuilder AppendFormat<T>(this StringBuilder builder, IFormatProvider? formatProvider, ReadOnlySpan<char> format, IFormatArgumentProvider<T> argumentProvider, T data)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}
			else if (argumentProvider == null)
			{
				throw new ArgumentNullException(nameof(argumentProvider));
			}

			return AppendFormatCore(builder, formatProvider, format, argumentProvider, data);
		}

		public static StringBuilder AppendFormatted(this StringBuilder builder, IFormatProvider? formatProvider, object? item, ReadOnlySpan<char> format)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			return AppendValue(builder, formatProvider, item, format);
		}

		static StringBuilder AppendFormatCore<T>(StringBuilder builder, IFormatProvider? formatProvider, ReadOnlySpan<char> format, IFormatArgumentProvider<T> argumentProvider, T data)
		{
			var scanner = new FormatScanner(format);

			while (scanner.MoveNext())
			{
				if (!scanner.IsArgument)
				{
					builder.Append(scanner.Text);
				}
				else
				{
					var value = argumentProvider.GetArgument(scanner.Text, data);
					AppendFormatArgument(builder, formatProvider, value, scanner.ArgumentFormat, scanner.ArgumentAlignment);
				}
			}

			return builder;
		}

		static void AppendFormatArgument(StringBuilder builder, IFormatProvider? formatProvider, object? value, ReadOnlySpan<char> argumentFormat, ReadOnlySpan<char> argumentAlignment)
		{
			var pos = builder.Length;

			if (ContainsEscapes(argumentFormat, out var unescapedLength))
			{
				var buffer = ArrayPool<char>.Shared.Rent(unescapedLength);
				var span = buffer.AsSpan(0, unescapedLength);
				WriteUnescaped(argumentFormat, span);
				builder.AppendValue(formatProvider, value, span);
				ArrayPool<char>.Shared.Return(buffer);
			}
			else
			{
				builder.AppendValue(formatProvider, value, argumentFormat);
			}

			if (argumentAlignment.Length > 0)
			{
				if (!TryParseInt(argumentAlignment, out var alignment))
				{
					throw new FormatException();
				}

				var written = builder.Length - pos;

				if (alignment > written)
				{
					builder.Append(' ', alignment - written);
				}
				else if (-alignment > written)
				{
					builder.Insert(pos, " ", -alignment - written);
				}
			}
		}

		static StringBuilder AppendValue(this StringBuilder builder, IFormatProvider? formatProvider, object? item, ReadOnlySpan<char> format)
		{
			return item switch
			{
				null => builder,
#if NET6_0_OR_GREATER
				ISpanFormattable spanFormattable => builder.AppendSpanFormattableValue(formatProvider, spanFormattable, format),
#endif
				IFormattable formattable => builder.AppendFormattableValue(formatProvider, formattable, format),
				_ => builder.Append(item.ToString()),
			};
		}

		static StringBuilder AppendFormattableValue(this StringBuilder builder, IFormatProvider? formatProvider, IFormattable item, ReadOnlySpan<char> format)
			=> builder.Append(item.ToString(format.Length == 0 ? null : format.ToString(), formatProvider));

#if NET6_0_OR_GREATER
		static StringBuilder AppendSpanFormattableValue(this StringBuilder builder, IFormatProvider? formatProvider, ISpanFormattable item, ReadOnlySpan<char> format)
		{
			var size = 16;
			var pool = ArrayPool<char>.Shared;
			var buffer = pool.Rent(size);
			int written;

			while (!item.TryFormat(buffer, out written, format, formatProvider))
			{
				size = buffer.Length * 2;
				pool.Return(buffer);
				buffer = pool.Rent(size);
			}

			builder.Append(buffer.AsSpan(0, written));
			pool.Return(buffer);
			return builder;
		}
#endif

		static bool TryParseInt(ReadOnlySpan<char> text, out int value)
		{
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
			return int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
#else
			return int.TryParse(text.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
#endif
		}

		[DoesNotReturn]
		static void InvalidFormat() => throw new FormatException("Input string was not in a correct format.");
	}
}
