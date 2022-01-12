using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Beatmap;
using OhHeck.Core.Beatmap.ModCustomData;

namespace OhHeck.Core.json;

public class PointDataListConverter : JsonConverter<List<PointData>>
{
	private static PointData ParsePointData(ref Utf8JsonReader reader)
	{
		var smooth = false;
		Functions? easing = null;

		List<float> preData = new();

		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
				break;

			switch (reader.TokenType)
			{
				// Point data is [...]
				case JsonTokenType.Number:
					preData.Add((float) reader.GetDouble());
					break;
				case JsonTokenType.String:
					var str = reader.GetString()!;
					if (str == "splineCatmullRom")
					{
						smooth = true;
					}
					else
					{
						easing = Enum.Parse(typeof(Functions), str) as Functions?;
					}

					break;

				default:
					throw new InvalidOperationException($"Not a array or number. Received {reader.TokenType}");
			}
		}

		var time = preData.Last();
		var animationData = preData.GetRange(0, preData.Count - 1).ToArray();

		return new PointData(time: time, data: animationData, smooth: smooth, easing: easing);
	}

	private static void WritePointData(ref Utf8JsonWriter writer, PointData point)
	{
		// single point def
		writer.WriteStartArray();
		foreach (var x in point.Data)
		{
			writer.WriteNumberValue(x);
		}

		if (point.Easing is not null)
		{
			writer.WriteStringValue(point.Easing.ToString());
		}

		if (point.Smooth)
		{
			writer.WriteStringValue("splineCatmullRom");
		}
		writer.WriteEndArray();
	}

	public override List<PointData>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		List<PointData> pointDatas = new();


		if (reader.TokenType != JsonTokenType.StartArray)
		{
			throw new InvalidDataException("Not an array");
		}

		// Parses an array of floats/strings and inner arrays of floats/strings
		// [0, 0, 0, "ease"] and [[0, 0, 0], [1, 1, 1, "easeLinear"]]
		while (reader.Read())
		{
			if (reader.TokenType == JsonTokenType.EndArray)
				break;

			switch (reader.TokenType)
			{
				// point data is [[...], [...]]
				case JsonTokenType.StartArray:
					pointDatas.Add(ParsePointData(ref reader));
					break;
				// point data is [...]
				case JsonTokenType.Number:
				case JsonTokenType.String:
					pointDatas.Add(ParsePointData(ref reader));
					break;

				default:
					throw new InvalidOperationException($"Not a array or number. Received {reader.TokenType}");
			}
		}

		return pointDatas;
	}

	public override void Write(Utf8JsonWriter writer, List<PointData> value, JsonSerializerOptions options)
	{
		if (value.Count == 1)
		{
			var point = value.First();
			WritePointData(ref writer, point);
		}
		else
		{
			writer.WriteStartArray();
			foreach (var point in value)
			{
				WritePointData(ref writer, point);
			}
			writer.WriteEndArray();
		}
	}
}