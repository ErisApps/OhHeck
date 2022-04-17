using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

public class PointDefinitionProxyConverter : JsonConverter<PointDefinitionDataProxy>
{
	public override PointDefinitionDataProxy? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.TokenType == JsonTokenType.String ? new PointDefinitionDataProxy(reader.GetString()!) : new PointDefinitionDataProxy(JsonSerializer.Deserialize<PointDefinitionData>(ref reader, options)!);

	public override void Write(Utf8JsonWriter writer, PointDefinitionDataProxy value, JsonSerializerOptions options)
	{
		if (value.Name is not null)
		{
			writer.WriteStringValue(value.Name);
		}

		JsonSerializer.Serialize(writer, value, options);
	}
}