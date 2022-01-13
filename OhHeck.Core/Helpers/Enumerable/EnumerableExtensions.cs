using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace OhHeck.Core.Helpers.Enumerable;

public static class EnumerableExtensions
{
	public static bool AreArrayElementsIdentical<T>(this T[] enumerable1, T[] enumerable2)
	{
		if (enumerable1.Length != enumerable2.Length)
		{
			return false;
		}

		for (var i = 0; i < enumerable1.Length; i++)
		{
			var element1 = enumerable1[i];
			var element2 = enumerable2[i];

			if ((element1 is null && element2 is null) || !Equals(element1, element2))
			{
				return false;
			}
		}

		return true;
	}

	public static bool AreArrayElementsIdentical<T>(this IReadOnlyList<T> enumerable1, IReadOnlyList<T> enumerable2)
	{
		if (enumerable1.Count != enumerable2.Count)
		{
			throw new InvalidOperationException($"Arrays are not matching lengths. First: {enumerable1.Count} Second: {enumerable2.Count}");
		}

		for (var i = 0; i < enumerable1.Count; i++)
		{
			var element1 = enumerable1[i];
			var element2 = enumerable2[i];

			if ((element1 is null && element2 is null) || !Equals(element1, element2))
			{
				return false;
			}
		}

		return true;
	}
}