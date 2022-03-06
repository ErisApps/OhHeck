using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

// Parses BeatmapCustomData's _pointDefinitions
public class BeatmapPointDefinitionConverter : JsonConverter<List<PointDefinitionData>>
{
	public override List<PointDefinitionData> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var pointDefs = JsonSerializer.Deserialize<List<PointDefinitionData>>(ref reader, options)!;

		var resolver = (PointDefinitionReferenceResolver) options.ReferenceHandler!.CreateResolver();


		foreach (var pointDef in pointDefs)
		{
			if (pointDef.Name is null)
			{
				throw new JsonException();
			}

			resolver.AddReference(pointDef.Name, pointDef);
		}

		return pointDefs;
	}

	public override void Write(Utf8JsonWriter writer, List<PointDefinitionData> value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, options);
}