using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

public class BeatmapCustomEventListConverter: JsonConverter<List<BeatmapCustomEvent>>
{
	public override List<BeatmapCustomEvent> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new JsonException("Not an array");
		}

		List<BeatmapCustomEvent> list = new();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
			{
				break;
			}


			list.Add(ReadEvent(ref reader, options));
		}


		return list;
	}

	private static BeatmapCustomEvent ReadEvent(ref Utf8JsonReader reader, JsonSerializerOptions options)
	{
		var restore = reader;
		// var tempReader = new Utf8JsonReader(reader., reader.IsFinalBlock, reader.CurrentState, options);
		var basicBeatmapCustomEvent = JsonSerializer.Deserialize<BeatmapCustomEvent>(ref reader, options)!;


		switch (basicBeatmapCustomEvent.Type)
		{
			case EventTypes.ANIMATE_TRACK:
				reader = restore;
				basicBeatmapCustomEvent = JsonSerializer.Deserialize<AnimateTrackEvent>(ref reader, options)!;
				break;
			case EventTypes.ASSIGN_PATH_ANIMATION:
				reader = restore;
				basicBeatmapCustomEvent = JsonSerializer.Deserialize<AssignPathAnimationEvent>(ref reader, options)!;
				break;
		}

		if (basicBeatmapCustomEvent is not AnimateEvent animateEvent)
		{
			return basicBeatmapCustomEvent;
		}

		var propDictionary = animateEvent.Data
				// skip keys that are not PointData types
			.Where(s => s.Key is not "_track" or "_duration" or "_easing")
			.ToDictionary(s => s.Key, s => s.Value);
		animateEvent.PointProperties = PointHelper.PointDefinitionDatasFromDictionary(propDictionary, (PointDefinitionReferenceResolver) options.ReferenceHandler!.CreateResolver());

		return basicBeatmapCustomEvent;
	}



	public override void Write(Utf8JsonWriter writer, List<BeatmapCustomEvent> value, JsonSerializerOptions options) => JsonSerializer.Serialize(writer, value, options);
}