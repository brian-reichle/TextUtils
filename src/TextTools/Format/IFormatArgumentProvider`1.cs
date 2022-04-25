// Copyright (c) Brian Reichle.  All Rights Reserved.  Licensed under the MIT License.  See License.txt in the project root for license information.
using System;

namespace TextTools
{
	public interface IFormatArgumentProvider<T>
	{
		object? GetArgument(ReadOnlySpan<char> identifier, T data);
	}
}
