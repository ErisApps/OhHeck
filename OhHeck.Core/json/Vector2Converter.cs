using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Structs;

namespace OhHeck.Core.Json;

public class Vector2Converter : JsonConverter<Vector2>
{
	public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new InvalidOperationException($"Expected {JsonTokenType.StartArray}, got {reader.TokenType}. Mappers, what");
		}

		var values = new float[2];
		var count = 0;

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				break;
			}

			if (count >= 2)
			{
				continue;
			}

			values[count] = (float) reader.GetDouble();
			count++;
		}

		return new Vector2(values[0], values[1]);
	}

	public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		writer.WriteNumberValue(value.x);
		writer.WriteNumberValue(value.y);
		writer.WriteEndArray();
	}
}