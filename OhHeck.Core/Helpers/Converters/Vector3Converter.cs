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
			throw new JsonException($"Expected {JsonTokenType.StartArray}, got {reader.TokenType}. Mappers, what");
		}

		reader.Read();
		Vector3 v = new();
		v.x = (float)reader.GetDouble();
		reader.Read();
		v.y = (float)reader.GetDouble();
		reader.Read();
		if (reader.TokenType == JsonTokenType.EndArray)
		{
			// Some Vector3s are actually Vector2s... Something should probably be fixed in the map?
			// TODO: This makes Vector2s nearly obsolete, since their conversion no longer applies.
			reader.Read();
			return v;
		}
		v.z = (float)reader.GetDouble();
		reader.Read();
		if (reader.TokenType != JsonTokenType.EndArray)
		{
			throw new JsonException($"Expected {JsonTokenType.EndArray}, got {reader.TokenType} for {nameof(Vector3)}");
		}
		reader.Read();
		return v;
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