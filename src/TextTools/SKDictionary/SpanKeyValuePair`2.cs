// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace TextTools
{
	public struct SpanKeyValuePair<TKeyElement, TValue>
	{
		public SpanKeyValuePair(TKeyElement[] key, TValue value)
		{
			_key = key;
			Value = value;
		}

		public ReadOnlySpan<TKeyElement> Key => _key;
		public TValue Value { get; }

		readonly TKeyElement[] _key;
	}
}
