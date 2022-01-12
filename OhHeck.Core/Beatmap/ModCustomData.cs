using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using OhHeck.Core.Json;

namespace OhHeck.Core.Beatmap.ModCustomData;

#region Chroma
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LookupMethod
{
	Regex,
	Exact,
	Contains
}

public class EnvironmentEnhancement
{
	// {
	// 	"_id": "^.*\\[\\d*[13579]\\]BigTrackLaneRing\\(Clone\\)$",
	// 	"_lookupMethod": "Regex",
	// 	"_scale": [0.1, 0.1, 0.1]
	// }

	public EnvironmentEnhancement(LookupMethod lookupMethod, string id)
	{
		LookupMethod = lookupMethod;
		Id = id;
	}

	[JsonPropertyName("_id")]
	public string Id { get; }


	[JsonPropertyName("_lookupMethod")]
	public LookupMethod LookupMethod { get; }

	[JsonExtensionData]
	public Dictionary<string, object> DontCareAboutThisData { get; set; } = new();
}
#endregion

#region Tracks
public class PointData
{
	public float[] Data { get; }

	public float Time { get; }

	// if null, it SHOULD mean the first point
	public Functions? Easing { get; }

	public bool Smooth { get; } = false;

	public PointData(float[] data, float time, Functions? easing, bool smooth)
	{
		Data = data;
		Time = time;
		Easing = easing;
		Smooth = smooth;
	}

	public PointData(List<object> data)
	{
		List<float> preData = new();

		foreach (var o in data)
		{
			switch (o)
			{
				case int:
				case float:
					preData.Add((float) o);
					break;
				case string str:
				{
					if (str == "splineCatmullRom")
					{
						Smooth = true;
					}
					else
					{
						Easing = Enum.Parse(typeof(Functions), str) as Functions?;
					}
					break;
				}
			}
		}

		Time = preData.Last();
		Data = preData.GetRange(0, preData.Count - 1).ToArray();
	}
}

public class PointDefinitionData
{
	public PointDefinitionData(string name, List<PointData> points)
	{
		Name = name;
		Points = points;
	}

	[JsonPropertyName("_name")]
	public string Name { get; }

	// We might need a custom parser for this to make this actually not stupid
	// We also need to parse [...] -> PointData
	// Most of the time it will be [[...], [...]] -> List<PointData> though
	[JsonPropertyName("_points")]
	[JsonConverter(typeof(PointDataListConverter))]
	public List<PointData> Points { get; }
}

#endregion