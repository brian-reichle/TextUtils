// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TextTools
{
	public sealed class StringPool
	{
		public bool TryGetString(string text, [NotNullWhen(true)] out string? value) => _lookup.TryGetValue(text, out value);

		public bool TryGetString(ReadOnlySpan<char> text, [NotNullWhen(true)] out string? value) => TryGetString(text.ToString(), out value);

		public string GetString(string text)
		{
			if (TryGetString(text, out var value))
			{
				return value;
			}

			_lookup.Add(text, text);
			return text;
		}

		public string GetString(ReadOnlySpan<char> text) => GetString(text.ToString());

		readonly Dictionary<string, string> _lookup = new Dictionary<string, string>();
	}
}
