// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using NUnit.Framework;

namespace TextTools.Utils.Test
{
	[TestFixture]
	class PrimeTest
	{
		[TestCase(1, ExpectedResult = 3)]
		[TestCase(10, ExpectedResult = 11)]
		[TestCase(15, ExpectedResult =17)]
		[TestCase(47, ExpectedResult = 47)]
		[TestCase(1000, ExpectedResult = 1361)]
		[TestCase(889871 * 2, ExpectedResult = 1779761)]
		public int GetPrime(int value) => Primes.GetPrime(value);
	}
}
