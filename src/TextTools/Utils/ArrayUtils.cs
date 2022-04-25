// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace TextTools.Utils
{
	static class ArrayUtils
	{
		public static void Fill(int[] array, int value)
		{
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP
			Array.Fill(array, -1);
#else
			for (var i = 0; i < array.Length; i++)
			{
				array[i] = value;
			}
#endif
		}

		public static T[] CloneArray<T>(T[] source)
		{
			var result = new T[source.Length];
			Array.Copy(source, result, source.Length);
			return result;
		}
	}
}
