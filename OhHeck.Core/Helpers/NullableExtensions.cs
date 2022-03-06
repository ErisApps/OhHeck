namespace OhHeck.Core.Helpers;

public static class NullableExtensions
{
	// returns true if x and y are both null or both not null
	public static bool NullabilityEqual<T>(this T? x, T? y) => x is null == y is null;
}