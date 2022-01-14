using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;

namespace OhHeck.Core.Models.Structs;

[JsonConverter(typeof(FakeTruthyConverter))]
public enum FakeTruthy
{
	TRUE,
	FALSE,
	STRING_TRUE,
	STRING_FALSE
}

public static class FakeTruthyExtensions
{
	public static bool isTrue(this FakeTruthy fakeTruthy) => fakeTruthy is FakeTruthy.TRUE or FakeTruthy.STRING_TRUE;

	public static bool isFalse(this FakeTruthy fakeTruthy) => fakeTruthy is FakeTruthy.FALSE or FakeTruthy.STRING_FALSE;

	public static bool IsString(this FakeTruthy fakeTruthy) => fakeTruthy is FakeTruthy.STRING_TRUE or FakeTruthy.STRING_FALSE;
}