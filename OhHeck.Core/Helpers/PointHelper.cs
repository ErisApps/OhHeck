using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers;

public static class PointHelper
{
	public static List<PointData>? GetPointDatas(object? v) =>
		v switch
		{
			List<PointData> list => list,
			PointDefinitionData pointDefinitionData => pointDefinitionData.Points,
			_ => null
		};

	public static Dictionary<string, List<PointData>>? GetPointDataDictionary(object? v) =>
		v switch
		{
			List<PointData> list => new Dictionary<string, List<PointData>> {{string.Empty, list}},
			PointDefinitionData pointDefinitionData => new Dictionary<string, List<PointData>> {{string.Empty, pointDefinitionData.Points}},
			Dictionary<string, PointDefinitionData> dictionary => dictionary.ToDictionary(s => s.Key, (pair => pair.Value.Points)),
			_ => null
		};
}