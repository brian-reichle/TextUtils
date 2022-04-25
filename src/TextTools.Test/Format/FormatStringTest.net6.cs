// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
#if NET6_0_OR_GREATER
using System;
using System.Text;
using Moq;
using NUnit.Framework;
using TextTools.Test.TestUtils;

namespace TextTools.Test
{
	[TestFixture]
	partial class FormatStringTest
	{
		[Test]
		public void AppendFormatted_SpanFormattable()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);
			var item = new DummySpanFormattable('x', 15);

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				item,
				ReadOnlySpan<char>.Empty)
				.ToString();

			Assert.That(result, Is.EqualTo("#" + new string('x', 15)));
		}

		[Test]
		public void AppendFormatted_SpanFormattable_Format()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);
			var item = new DummySpanFormattable('y', 15, expectedFormat: "Foo");

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				item,
				"Foo".AsSpan())
				.ToString();

			Assert.That(result, Is.EqualTo("#" + new string('y', 15)));
		}

		[Test]
		public void AppendFormatted_SpanFormattable_Grow()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);
			var item = new DummySpanFormattable('z', 40);

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				item,
				ReadOnlySpan<char>.Empty)
				.ToString();

			Assert.That(result, Is.EqualTo("#" + new string('z', 40)));
		}
	}
}
#endif
