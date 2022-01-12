using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Helpers.Converters;

public class Vector3Converter : JsonConverter<Vector3>
{
	public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new InvalidOperationException($"Expected {JsonTokenType.StartArray}, got {reader.TokenType}. Mappers, what");
		}

		var values = new float[3];
		var count = 0;

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				break;
			}

			if (count >= 3)
			{
				continue;
			}

			values[count] = (float) reader.GetDouble();
			count++;
		}

		return new Vector3(values[0], values[1], values[2]);
	}

	public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		writer.WriteNumberValue(value.x);
		writer.WriteNumberValue(value.y);
		writer.WriteNumberValue(value.z);
		writer.WriteEndArray();
	}
}