// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	class StringBuilderPoolTest
	{
		[Test]
		public void RentMultiple()
		{
			var builder1 = StringBuilderPool.Rent();
			var builder2 = StringBuilderPool.Rent();
			Assert.That(builder1, Is.Not.SameAs(builder2));
		}

		[Test]
		public void Recycle()
		{
			var builder1 = StringBuilderPool.Rent();
			builder1.Append("Foo");

			StringBuilderPool.Return(builder1);

			var builder2 = StringBuilderPool.Rent();
			Assert.That(builder2, Is.SameAs(builder1), "Should reuse the returned builder.");
			Assert.That(builder2.ToString(), Is.Empty, "Should have cleared the returnd builder.");
		}

		[Test]
		public void ToStringAndReturn()
		{
			var builder1 = StringBuilderPool.Rent();
			builder1.Append("Bar");

			Assert.That(StringBuilderPool.ToStringAndReturn(builder1), Is.EqualTo("Bar"));

			var builder2 = StringBuilderPool.Rent();
			Assert.That(builder2, Is.SameAs(builder1));
		}

		[Test]
		public void DontReuseOversizedBuilders()
		{
			var builder1 = StringBuilderPool.Rent();
			builder1.Capacity = 201;
			StringBuilderPool.Return(builder1);

			var builder2 = StringBuilderPool.Rent();
			Assert.That(builder2, Is.Not.SameAs(builder1));
		}

		[SetUp]
		protected void Setup()
		{
			StringBuilderPool.Rent(); // discard whatever happens to be cached;
		}
	}
}
