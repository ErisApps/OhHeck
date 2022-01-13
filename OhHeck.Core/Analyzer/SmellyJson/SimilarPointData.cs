using System;
using System.Collections.Generic;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.SmellyJson;

[BeatmapWarning("similar-point-data")]
public class SimilarPointData : IBeatmapWarning {

	// TODO: Test
	// I've got no idea if this is a reliable algorithm
	public string? Validate(Type fieldType, object? value)
	{
		if (value is not List<PointData> { Count: > 1 } pointDatas)
		{
			return null;
		}

		var prevPoint = pointDatas.First(); // if empty, we have a problem
		for (var i = 1; i < pointDatas.Count; i++)
		{
			var point = pointDatas[i];

			var timeDifference = MathF.Abs(point.Time - prevPoint.Time);

			// example point data
			// "_name":"colorWave","_points":[
			// [1,1,1,1,0],
			// [0,0,4,1,0.125],
			// [0,0.5,2,1,0.25],
			// [0,0,1,1,0.375],
			// [1,1,2,1,0.5],
			// [0,0,4,1,0.625],
			// [0,0.25,2,1,0.75],
			// [0.1,0.2,1,1,0.875],
			// [2,2,2,2,1]
			// ]}

			// Both points are identical
			if (prevPoint.Data.AreFloatsSimilar(point.Data, timeDifference))
			{
				return $"Point data are too similar relative to the time difference {timeDifference}: Point1 {prevPoint.Data.ArrayToString()}:{prevPoint.Time} Point2: {point.Data.ArrayToString()}:{point.Time}";
			}

			prevPoint = point;
		}

		return null;
	}
}