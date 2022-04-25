// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;

namespace TextTools.Utils
{
	static class StringBuilderPool
	{
		public static StringBuilder Rent()
		{
			var result = _cache ?? new StringBuilder();
			_cache = null;
			return result;
		}

		public static void Return(StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			ReturnCore(builder);
		}

		public static string ToStringAndReturn(StringBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			var result = builder.ToString();
			ReturnCore(builder);
			return result;
		}

		static void ReturnCore(StringBuilder builder)
		{
			if (builder.Capacity <= 200 && _cache == null)
			{
				builder.Clear();
				_cache = builder;
			}
		}

		[ThreadStatic]
		static StringBuilder? _cache;
	}
}
