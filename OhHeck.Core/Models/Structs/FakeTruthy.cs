using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using OhHeck.Core.Helpers.Converters;

namespace OhHeck.Core.Models.Structs;

[JsonConverter(typeof(FakeTruthyConverter))]
[SuppressMessage("Design", "CA1069:Enums values should not be duplicated", Justification = "STRING_TRUE and STRING_FALSE are marked equivalent")]
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

	public static bool IsString(this FakeTruthy fakeTruthy) => fakeTruthy is FakeTruthy.STRING_TRUE or FakeTruthy.STRING_FALSE;
}