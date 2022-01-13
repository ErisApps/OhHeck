using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Helpers.Converters;

public class Vector2Converter : JsonConverter<Vector2>
{
	public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException($"Expected {JsonTokenType.StartArray}, got {reader.TokenType}. Mappers, what");
		}
		Vector2 v = new();
		reader.Read();
		v.x = (float) reader.GetDouble();
		reader.Read();
		v.y = (float) reader.GetDouble();
		reader.Read();
		if (reader.TokenType != JsonTokenType.EndArray)
		{
			throw new JsonException($"Expected {JsonTokenType.EndArray}, got {reader.TokenType} for {nameof(Vector2)}");
		}
		reader.Read();
		return v;
	}

	public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options)
	{
		writer.WriteStartArray();
		writer.WriteNumberValue(value.x);
		writer.WriteNumberValue(value.y);
		writer.WriteEndArray();
	}
}