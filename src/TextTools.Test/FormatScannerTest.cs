// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	class FormatScannerTest
	{
		[Test]
		public void Empty()
		{
			var scanner = New(string.Empty);
			AssertEnd(ref scanner);
		}

		[Test]
		public void TextOnly()
		{
			var scanner = New("FooBar");
			AssertText(ref scanner, "FooBar");
			AssertEnd(ref scanner);
		}

		[Test]
		public void Escaped()
		{
			var scanner = New("A{{B}}C");
			AssertText(ref scanner, "A{");
			AssertText(ref scanner, "B}");
			AssertText(ref scanner, "C");
			AssertEnd(ref scanner);
		}

		[Test]
		public void Escaped2()
		{
			var scanner = New("{{}}");
			AssertText(ref scanner, "{");
			AssertText(ref scanner, "}");
			AssertEnd(ref scanner);
		}

		[Test]
		public void ArgumentOnly()
		{
			var scanner = New("{arg}");
			AssertArg(ref scanner, "arg");
			AssertEnd(ref scanner);
		}

		[Test]
		public void ArgumentFormat()
		{
			var scanner = New("{X:Y}");
			AssertArg(ref scanner, "X", format: "Y");
			AssertEnd(ref scanner);
		}

		[Test]
		public void ArgumentAlignment()
		{
			var scanner = New("{X,Y}");
			AssertArg(ref scanner, "X", alignment: "Y");
			AssertEnd(ref scanner);
		}

		[Test]
		public void ArgumentFormatAlignment()
		{
			var scanner = New("{X,Y:Z}");
			AssertArg(ref scanner, "X", alignment: "Y", format: "Z");
			AssertEnd(ref scanner);
		}

		[Test]
		public void EscapeInFormat()
		{
			var scanner = New("{X:Y{{}}}");
			AssertArg(ref scanner, "X", format: "Y{{}}");
			AssertEnd(ref scanner);
		}

		[Test]
		public void Mixed()
		{
			var scanner = New("A{B:C}{D}E{F,G}H");
			AssertText(ref scanner, "A");
			AssertArg(ref scanner, "B", format: "C");
			AssertArg(ref scanner, "D");
			AssertText(ref scanner, "E");
			AssertArg(ref scanner, "F", alignment: "G");
			AssertText(ref scanner, "H");
			AssertEnd(ref scanner);
		}

		static FormatScanner New(string format) => new FormatScanner(format.AsSpan());

		static void AssertText(ref FormatScanner scanner, string text)
		{
			Assert.That(scanner.MoveNext(), Is.True, nameof(scanner.MoveNext));
			Assert.That(scanner.IsArgument, Is.False, nameof(scanner.IsArgument));
			Assert.That(scanner.Text.ToString(), Is.EqualTo(text), nameof(scanner.Text));
			Assert.That(scanner.ArgumentFormat.ToString(), Is.Empty, nameof(scanner.ArgumentFormat));
			Assert.That(scanner.ArgumentAlignment.ToString(), Is.Empty, nameof(scanner.ArgumentAlignment));
		}

		static void AssertArg(ref FormatScanner scanner, string text, string alignment = "", string format = "")
		{
			Assert.That(scanner.MoveNext(), Is.True, nameof(scanner.MoveNext));
			Assert.That(scanner.IsArgument, Is.True, nameof(scanner.IsArgument));
			Assert.That(scanner.Text.ToString(), Is.EqualTo(text), nameof(scanner.Text));
			Assert.That(scanner.ArgumentAlignment.ToString(), Is.EqualTo(alignment), nameof(scanner.ArgumentAlignment));
			Assert.That(scanner.ArgumentFormat.ToString(), Is.EqualTo(format), nameof(scanner.ArgumentFormat));
		}

		static void AssertEnd(ref FormatScanner scanner)
		{
			Assert.That(scanner.MoveNext(), Is.False, nameof(scanner.MoveNext));
		}
	}
}
