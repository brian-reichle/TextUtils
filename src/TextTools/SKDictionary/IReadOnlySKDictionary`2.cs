// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TextTools
{
	public interface IReadOnlySKDictionary<TKeyElement, TValue> : IReadOnlyCollection<SpanKeyValuePair<TKeyElement, TValue>>
	{
		bool ContainsKey(ReadOnlySpan<TKeyElement> key);
		bool TryGetValue(ReadOnlySpan<TKeyElement> key, [MaybeNullWhen(false)] out TValue value);
		TValue this[ReadOnlySpan<TKeyElement> key] { get; }
	}
}
