// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;

namespace TextTools
{
	public static class StringBuilderSpanExtensions
	{
#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
		public static StringBuilder Append(this StringBuilder builder, ReadOnlySpan<char> span)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

#if NETCOREAPP || NETSTANDARD2_1_OR_GREATER
			return builder.Append(span);
#else
			unsafe
			{
				fixed (char* ptr = span)
				{
					return builder.Append(ptr, span.Length);
				}
			}
#endif
		}
	}
}
