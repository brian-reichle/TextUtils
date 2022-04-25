// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace TextTools
{
	public interface ISKDictionary<TKeyElement, TValue> : IReadOnlySKDictionary<TKeyElement, TValue>
	{
		new TValue this[ReadOnlySpan<TKeyElement> key] { get; set; }
		void Add(ReadOnlySpan<TKeyElement> key, TValue value);
		bool Remove(ReadOnlySpan<TKeyElement> key);
	}
}
