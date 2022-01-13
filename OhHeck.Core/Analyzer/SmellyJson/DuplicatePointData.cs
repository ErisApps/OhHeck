using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.SmellyJson;

[BeatmapWarning("duplicate-point-data")]
public class DuplicatePointData : IBeatmapWarning {

	// TODO: Test
	public string? Validate(FieldInfo fieldInfo, object? value)
	{
		if (value is not List<PointData> { Count: > 1 } pointDatas)
		{
			return null;
		}

		var prevPoint = pointDatas.First(); // if empty, we have a problem
		for (var i = 1; i < pointDatas.Count; i++)
		{
			var point = pointDatas[i];

			// Both points are identical
			if (prevPoint.Data.AreArrayElementsIdentical(point.Data))
			{
				return $"Point data are identical: Point1 {prevPoint.Data}:{prevPoint.Time} Point2: {point.Data}:{point.Time}";
			}
		}

		return null;
	}
}