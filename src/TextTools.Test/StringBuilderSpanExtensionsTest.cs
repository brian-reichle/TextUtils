// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Text;
using NUnit.Framework;

namespace TextTools.Test
{
	[TestFixture]
	class StringBuilderSpanExtensionsTest
	{
		[Test]
		public void AppendSpan()
		{
			var result = StringBuilderSpanExtensions.Append(new StringBuilder("#"), "Text".AsSpan()).ToString();
			Assert.That(result, Is.EqualTo("#Text"));
		}
	}
}
