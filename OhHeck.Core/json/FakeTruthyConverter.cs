using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Structs;

namespace OhHeck.Core.Json;

public class FakeTruthyConverter : JsonConverter<FakeTruthy>
{
	public override FakeTruthy Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		switch (reader.TokenType)
		{
			case JsonTokenType.True:
				return FakeTruthy.TRUE;
			case JsonTokenType.False:
				return FakeTruthy.FALSE;
			case JsonTokenType.String:
				var str = reader.GetString()!;

				return bool.Parse(str) ? FakeTruthy.STRING_TRUE : FakeTruthy.STRING_FALSE;

			default:
				throw new InvalidOperationException($"Boolean type is {reader.TokenType} mappers why please stop this end mii now");
		}
	}

	public override void Write(Utf8JsonWriter writer, FakeTruthy value, JsonSerializerOptions options) => writer.WriteBooleanValue(value == FakeTruthy.TRUE);
}