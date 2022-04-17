using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers;

public static class PointHelper
{

	public static void SortPoints(List<PointData> pointDatas) => pointDatas.Sort((a, b) =>
	{
		if (a.Time > b.Time)
		{
			return 1;
		}

		if (a.Time < b.Time)
		{
			return -1;
		}

		return 0;
	});

	public static List<PointData>? GetPointDatas(object? v) =>
		v switch
		{
			List<PointData> list => list,
			PointDefinitionData pointDefinitionData => pointDefinitionData.Points,
			PointDefinitionDataProxy proxy => proxy.PointDefinitionData.Points,
			_ => null
		};

	public static Dictionary<string, List<PointData>>? GetPointDataDictionary(object? v) =>
		v switch
		{
			List<PointData> list => new Dictionary<string, List<PointData>> { { string.Empty, list } },
			PointDefinitionDataProxy proxy => new Dictionary<string, List<PointData>> { { string.Empty, proxy.PointDefinitionData.Points } },
			PointDefinitionData pointDefinitionData => new Dictionary<string, List<PointData>> { { string.Empty, pointDefinitionData.Points } },
			Dictionary<string, PointDefinitionData> dictionary => dictionary.ToDictionary(s => s.Key, (pair => pair.Value.Points)),
			Dictionary<string, PointDefinitionDataProxy> dictionary => dictionary.ToDictionary(s => s.Key, (pair => pair.Value.PointDefinitionData.Points)),
			AnimateEvent beatmapCustomEvent => beatmapCustomEvent.PointProperties.ToDictionary(s => s.Key, (pair => pair.Value.PointDefinitionData.Points)),
			_ => null
		};

	public static Dictionary<string, PointDefinitionDataProxy> PointDefinitionDatasFromDictionary(Dictionary<string, object> dictionary)
	{
		Dictionary<string, PointDefinitionDataProxy> pointDefinitionDatas = new();

		foreach (var (name, temp) in dictionary)
		{
			var o = temp;

			if (temp is JsonElement jsonElement)
			{
				o = jsonElement.ToNativeType();
			}

			switch (o)
			{
				case string pointName:
				{
					var pointDefinition = new PointDefinitionDataProxy(pointName);

					pointDefinitionDatas[name] = pointDefinition;
					break;
				}
				case IEnumerable keyframes:
				{
					// TODO: Consolidate into a common method
					var pointDatas = new List<PointData>();
					var tempObjs = new List<object>(); // I hate this
					foreach (var keyframe in keyframes)
					{
						// is [[...],[...]]
						if (keyframe is IEnumerable keyframeData)
						{
							pointDatas.Add(new PointData(keyframeData));
						}
						// is [...]
						else
						{
							tempObjs.Add(keyframe);
						}
					}

					if (tempObjs.Count > 0)
					{
						pointDatas.Add(new PointData(tempObjs.OfType<float>().ToArray(), 0, null, false));
					}

					SortPoints(pointDatas);
					pointDefinitionDatas[name] = new PointDefinitionDataProxy(new PointDefinitionData(null, pointDatas));
					break;
				}
			}
		}

		return pointDefinitionDatas;
	}
}