namespace OhHeck.Core.Helpers;

public static class NullableExtensions
{
	public static bool NullabilityEqual<T>(this T? x, T? y)
	{
		if (x is not null)
		{
			// true if both x and y are not null
			return y is not null;
		}

		// true if both x and y are null
		return y is null;
	}
}