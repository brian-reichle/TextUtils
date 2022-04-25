// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	class BitwiseSpanEqualityComparerTest
	{
		[TestCase(0, "Foo", ExpectedResult = -1153327528)]
		[TestCase(42, "Foo", ExpectedResult = 267039464)]
		[TestCase(0, "Bar", ExpectedResult = -1234700324)]
		[TestCase(42, "Bar", ExpectedResult = -606159809)]
		public int GetHashCode(int seed, string data)
		{
			var comparer = new BitwiseSpanEqualityComparer<char>(seed);

			return comparer.GetHashCode(data.AsSpan());
		}

		[TestCase("", "", ExpectedResult = true)]
		[TestCase("Foo", "", ExpectedResult = false)]
		[TestCase("Foo", "Foo", ExpectedResult = true)]
		[TestCase("Foo", "Bar", ExpectedResult = false)]
		public bool Equals(string value1, string value2)
		{
			var comparer = new BitwiseSpanEqualityComparer<char>();
			return comparer.Equals(value1.AsSpan(), value2.AsSpan());
		}
	}
}
