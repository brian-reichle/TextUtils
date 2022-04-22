// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	class StringPoolTest
	{
		[Test]
		public void GetString_EmptySpan()
		{
			var pool = new StringPool();
			Assert.That(pool.GetString(ReadOnlySpan<char>.Empty), Is.SameAs(string.Empty));
		}

		[Test]
		public void GetString_NewSpan()
		{
			var pool = new StringPool();
			Assert.That(pool.GetString("FooBarBaz".AsSpan(3, 3)), Is.EqualTo("Bar"));
		}

		[Test]
		public void GetString_NewString()
		{
			var str = "Bar";

			var pool = new StringPool();
			Assert.That(pool.GetString(str), Is.SameAs(str));
		}

		[Test]
		public void GetString_ExistingSpan()
		{
			var pool = new StringPool();
			var existing = pool.GetString("FooBarBaz".AsSpan(3, 3));
			Assert.That(pool.GetString("BazBarFoo".AsSpan(3, 3)), Is.SameAs(existing));
		}

		[Test]
		public void GetString_ExistingString()
		{
#pragma warning disable CA1846 // Prefer 'AsSpan' over 'Substring'
			var pool = new StringPool();
			var existing = pool.GetString("FooBarBaz".Substring(3, 3));
			Assert.That(pool.GetString("BazBarFoo".Substring(3, 3)), Is.SameAs(existing));
#pragma warning restore CA1846 // Prefer 'AsSpan' over 'Substring'
		}

		[Test]
		public void TryGetString_NewSpan()
		{
			var pool = new StringPool();
			Assert.That(pool.TryGetString("FooBarBaz".AsSpan(3, 3), out var result), Is.False);
			Assert.That(result, Is.Null);
		}

		[Test]
		public void TestgetString_NewString()
		{
			var pool = new StringPool();
			Assert.That(pool.TryGetString("Baz", out var result), Is.False);
			Assert.That(result, Is.Null);
		}

		[Test]
		public void TryGetString_ExistingSpan()
		{
			var pool = new StringPool();
			var existing = pool.GetString("Bar");
			Assert.That(pool.TryGetString("FooBarBaz".AsSpan(3, 3), out var result), Is.True);
			Assert.That(result, Is.SameAs(existing));
		}

		[Test]
		public void TestgetString_ExistingString()
		{
			var pool = new StringPool();
			var existing = pool.GetString("Bar");
			Assert.That(pool.TryGetString("Bar", out var result), Is.True);
			Assert.That(result, Is.SameAs(existing));
		}

		[Test]
		public void MultipleStrings()
		{
			var pool = new StringPool();
			var foo = pool.GetString("Foo");
			var bar = pool.GetString("Bar".AsSpan());
			var baz = pool.GetString("Baz".AsSpan());

			Assert.That(foo, Is.Not.EqualTo(bar));
			Assert.That(foo, Is.Not.EqualTo(baz));
			Assert.That(bar, Is.Not.EqualTo(baz));

			Assert.That(pool.GetString("Foo".AsSpan()), Is.SameAs(foo));
			Assert.That(pool.GetString("Bar"), Is.SameAs(bar));
			Assert.That(pool.GetString("Baz".AsSpan()), Is.SameAs(baz));
		}

		[Test]
		public void GrowCapacity()
		{
			var pool = new StringPool(3);
			pool.GetString("A");
			pool.GetString("B");
			pool.GetString("C");
			pool.GetString("D");

			Assert.That(pool.TryGetString("A", out _), Is.True);
			Assert.That(pool.TryGetString("B", out _), Is.True);
			Assert.That(pool.TryGetString("C", out _), Is.True);
			Assert.That(pool.TryGetString("D", out _), Is.True);
			Assert.That(pool.TryGetString("E", out _), Is.False);
		}
	}
}
