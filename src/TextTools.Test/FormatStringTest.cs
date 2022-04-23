// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Buffers;
using System.Text;
using Moq;
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	partial class FormatStringTest
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

		[Test]
		public void AppendFormatted_Null()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				null,
				"Format".AsSpan())
				.ToString();

			Assert.That(result, Is.EqualTo("#"));
		}

		[Test]
		public void AppendFormatted_String()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				"Text",
				"Format".AsSpan())
				.ToString();

			Assert.That(result, Is.EqualTo("#Text"));
		}

		[Test]
		public void AppendFormatted_Object()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);

			var mockItem = new Mock<object>(MockBehavior.Strict);
			mockItem.Setup(x => x.ToString()).Returns("BaseToString");

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				mockItem.Object,
				"Format".AsSpan())
				.ToString();

			Assert.That(result, Is.EqualTo("#BaseToString"));
		}

		[Test]
		public void AppendFormatted_Formattable()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);

			var mockItem = new Mock<IFormattable>(MockBehavior.Strict);
			mockItem.Setup(x => x.ToString("Format", mockProvider.Object)).Returns("FormattedToString");

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				mockItem.Object,
				"Format".AsSpan())
				.ToString();

			Assert.That(result, Is.EqualTo("#FormattedToString"));
		}

		[Test]
		public void AppendFormatted_Formattable_EmptyFormat()
		{
			var mockProvider = new Mock<IFormatProvider>(MockBehavior.Strict);

			var mockItem = new Mock<IFormattable>(MockBehavior.Strict);
			mockItem.Setup(x => x.ToString(null, mockProvider.Object)).Returns("FormattedToString");

			var result = FormatString.AppendFormatted(
				new StringBuilder("#"),
				mockProvider.Object,
				mockItem.Object,
				ReadOnlySpan<char>.Empty)
				.ToString();

			Assert.That(result, Is.EqualTo("#FormattedToString"));
		}
	}
}
