// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System.Text;
using NUnit.Framework;

namespace TextTools.Utils.Test
{
	[TestFixture]
	class MurmurHashTest
	{
		[TestCase(0, "", ExpectedResult = 0u)]
		[TestCase(42, "", ExpectedResult = 142593372u)]
		[TestCase(0, "MultipleOf04", ExpectedResult = 1581625577u)]
		[TestCase(42, "MultipleOf04", ExpectedResult = 2241771124u)]
		[TestCase(0, "1Over", ExpectedResult = 1850922185u)]
		[TestCase(42, "1Over", ExpectedResult = 2281453132u)]
		[TestCase(0, "ShortText", ExpectedResult = 763083174u)]
		[TestCase(42, "ShortText", ExpectedResult = 1710374205u)]
		public uint Hash(int seed, string data) => HashCore(seed, data);

		[TestCase(0, ExpectedResult = 1115577742u)]
		[TestCase(42, ExpectedResult = 3833143635u)]
		public uint LongData(int seed)
			=> HashCore(seed, "Little Bo Peep has lost her sheep, but I know where to find them. In the deep freeze with a packet of pea's beside them.");

		static uint HashCore(int seed, string data)
		{
			var buffer = Encoding.ASCII.GetBytes(data);
			return unchecked((uint)MurmurHash.Hash(unchecked((uint)seed), buffer));
		}
	}
}
