using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Json;

namespace OhHeck.Core.Models.Structs;

[JsonConverter(typeof(FakeTruthyConverter))]
public enum FakeTruthy
{
	TRUE = 1,
	FALSE = 0,
	STRING_TRUE = 1,
	STRING_FALSE = 0
}

public static class FakeTruthyExtensions
{
	public static bool isTrue(this FakeTruthy fakeTruthy) => fakeTruthy is FakeTruthy.TRUE or FakeTruthy.STRING_TRUE;

	public static bool isFalse(this FakeTruthy fakeTruthy) => fakeTruthy is FakeTruthy.FALSE or FakeTruthy.STRING_FALSE;
}