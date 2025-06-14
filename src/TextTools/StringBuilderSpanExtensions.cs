// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;

namespace TextTools
{
	public static class StringBuilderSpanExtensions
	{
#if NET || NETSTANDARD2_1_OR_GREATER
		[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
#endif
		public static StringBuilder Append(this StringBuilder builder, ReadOnlySpan<char> span)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

#if NET || NETSTANDARD2_1_OR_GREATER
			return builder.Append(span);
#else
			// Ensure there will be enough capacity for the span before pinning,
			// to reduce the risk of triggering a GC while span is pinned.
			builder.EnsureCapacity(builder.Length + span.Length);

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
