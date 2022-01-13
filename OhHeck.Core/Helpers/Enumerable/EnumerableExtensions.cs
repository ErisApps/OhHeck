using System;
using System.Collections.Generic;
using System.Text;

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

			if (element1 is null && element2 is null)
			{
				continue;
			}

			if (!element1.NullabilityEqual(element2) || !Equals(element1, element2))
			{
				return false;
			}
		}

		return true;
	}

	public static string ArrayToString<T>(this IEnumerable<T?> enumerable)
	{
		StringBuilder builder = new("{");
		foreach (var x in enumerable)
		{
			builder.Append($"{x}, ");
		}

		builder.Append('}');

		return builder.ToString();
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

			if (element1 is null && element2 is null)
			{
				continue;
			}

			if (!element1.NullabilityEqual(element2) || !Equals(element1, element2))
			{
				return false;
			}
		}

		return true;
	}

	// returns true if all floats are similar
	public static bool AreFloatsSimilar(this float[] enumerable1, float[] enumerable2, float threshold)
	{
		if (enumerable1.Length != enumerable2.Length)
		{
			return false;
		}

		for (var i = 0; i < enumerable1.Length; i++)
		{
			var element1 = enumerable1[i];
			var element2 = enumerable2[i];

			if (MathF.Abs(element1 - element2) >= threshold)
			{
				return false;
			}
		}

		return true;
	}

	// returns true if all floats are similar
	public static bool AreFloatsSimilar(this IReadOnlyList<float> enumerable1, IReadOnlyList<float> enumerable2, float threshold)
	{
		if (enumerable1.Count != enumerable2.Count)
		{
			throw new InvalidOperationException($"Arrays are not matching lengths. First: {enumerable1.Count} Second: {enumerable2.Count}");
		}

		for (var i = 0; i < enumerable1.Count; i++)
		{
			var element1 = enumerable1[i];
			var element2 = enumerable2[i];

			if (MathF.Abs(element1 - element2) >= threshold)
			{
				return false;
			}
		}

		return true;
	}
}