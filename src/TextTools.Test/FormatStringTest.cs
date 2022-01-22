// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	class FormatStringTest
	{
		[TestCase("", ExpectedResult = "")]
		[TestCase("Foo", ExpectedResult = "Foo")]
		[TestCase("{{}}", ExpectedResult = "{}")]
		[TestCase("Foo{{Bar}}Baz", ExpectedResult = "Foo{Bar}Baz")]
		public string Unescape(string formatString) => FormatString.Unescape(formatString.AsSpan());

		[TestCase("", 0, ExpectedResult = false)]
		[TestCase("Foo", 3, ExpectedResult = false)]
		[TestCase("{{}}", 2, ExpectedResult = true)]
		[TestCase("Foo{{Bar}}Baz", 11, ExpectedResult = true)]
		public bool ContainsEscapes(string formatString, int unescapedLength)
		{
			var result = FormatString.ContainsEscapes(formatString.AsSpan(), out var len);
			Assert.That(len, Is.EqualTo(unescapedLength));
			return result;
		}

		[TestCase("", ExpectedResult = "")]
		[TestCase("Foo", ExpectedResult = "Foo")]
		[TestCase("{{}}", ExpectedResult = "{}")]
		[TestCase("Foo{{Bar}}Baz", ExpectedResult = "Foo{Bar}Baz")]
		public string Write(string formatString)
		{
			var buffer = ArrayPool<char>.Shared.Rent(12);
			var len = FormatString.WriteUnescaped(formatString.AsSpan(), buffer.AsSpan());
			var result = buffer.AsSpan(0, len).ToString();
			ArrayPool<char>.Shared.Return(buffer);
			return result;
		}
	}
}
