// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;
using System.Runtime.InteropServices;

namespace TextTools.Test.TestUtils
{
	sealed class KeyCollider<T> : ISpanEqualityComparer<T>
		where T : struct
	{
		public bool Equals(ReadOnlySpan<T> x, ReadOnlySpan<T> y)
			=> MemoryExtensions.SequenceEqual(
				MemoryMarshal.Cast<T, byte>(x),
				MemoryMarshal.Cast<T, byte>(y));

		public int GetHashCode(ReadOnlySpan<T> obj) => 42;
	}
}
