using System;
using System.Linq;
using OhHeck.Core.Helpers;
using OhHeck.Core.Helpers.Enumerable;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("similar-point-data")]
public class SimilarPointData : IBeatmapAnalyzer {

	// I've got no idea if this is a reliable algorithm
	// Compares two points and attempts to make a rough estimate of whether they're similar/redundant
	// based on their keyframe difference and time difference
	public void Validate(Type fieldType, object? value, IWarningOutput warningOutput) =>
		PointLintHelper.AnalyzePoints(value,  warningOutput, pointDataDictionary =>
		{

			foreach (var (s, pointDatas) in pointDataDictionary)
			{
				var prevPoint = pointDatas.First(); // if empty, we have a problem
				for (var i = 1; i < pointDatas.Count; i++)
				{
					var point = pointDatas[i];

					// ignore points who have different easing or smoothness since those can
					// be considered not similar even with small time differences
					if (point.Smooth != prevPoint.Smooth || point.Easing != prevPoint.Easing)
					{
						continue;
					}

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
						warningOutput.WriteWarning(
							$"Point data {s} are too similar relative to the time difference {timeDifference}: Point1 {prevPoint.Data.ArrayToString()}:{prevPoint.Time} Point2: {point.Data.ArrayToString()}:{point.Time}");
					}

					prevPoint = point;
				}
			}
		});
}