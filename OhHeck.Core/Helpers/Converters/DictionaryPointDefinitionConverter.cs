using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

// Parses a dictionary of property -> PointDefinitionData
// for example being "_position": "somePointDefinition" or "_rotation": [5, 2, 3]
public class DictionaryPointDefinitionConverter : JsonConverter<Dictionary<string, PointDefinitionDataProxy>>
{
	public override Dictionary<string, PointDefinitionDataProxy> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartObject)
		{
			throw new JsonException();
		}

		var pointDataListConverter = new PointDataListConverter();
		Dictionary<string, PointDefinitionDataProxy> pointDefinitionDatas = new();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndObject)
			{
				break;
			}

			if (reader.TokenType != JsonTokenType.PropertyName)
			{
				throw new JsonException();
			}

			var key = reader.GetString()!;
			reader.Read();

			switch (reader.TokenType)
			{
				case JsonTokenType.String:
				{
					var pointName = reader.GetString()!;
					var pointDefinition = new PointDefinitionDataProxy(pointName);

					pointDefinitionDatas[key] = pointDefinition;
					break;
				}
				case JsonTokenType.StartArray:
				{
					var pointDatas = pointDataListConverter.Read(ref reader, typeToConvert, options);

					pointDefinitionDatas[key] = new PointDefinitionDataProxy(new PointDefinitionData(pointDatas!));
					break;
				}
				default:
					throw new JsonException();
			}
		}

		return pointDefinitionDatas;
	}

	public override void Write(Utf8JsonWriter writer, Dictionary<string, PointDefinitionDataProxy> pointDefinitionDatas, JsonSerializerOptions options)
	{
		writer.WriteStartObject();
		foreach (var (key, pointDefinitionData) in pointDefinitionDatas)
		{
			if (pointDefinitionData.Name is not null)
			{
				writer.WriteString(key, pointDefinitionData.Name);
			}
			else
			{
				writer.WritePropertyName(key);
				JsonSerializer.Serialize(writer, pointDefinitionData, options);
			}
		}
		writer.WriteEndObject();
	}
}