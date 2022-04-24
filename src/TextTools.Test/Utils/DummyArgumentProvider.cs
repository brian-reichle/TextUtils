// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Collections.Generic;

namespace TextTools.Test.Utils
{
	sealed class DummyArgumentProvider : IFormatArgumentProvider<IReadOnlyDictionary<string, object>>
	{
		public static DummyArgumentProvider Instance { get; } = new DummyArgumentProvider();

		public object GetArgument(ReadOnlySpan<char> identifier, IReadOnlyDictionary<string, object> data)
			=> data[identifier.ToString()];

		DummyArgumentProvider()
		{
		}
	}
}
