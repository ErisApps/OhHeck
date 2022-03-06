using System;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("similar-point-data")]
public class SimilarPointData : IFieldAnalyzer {

	// The minimum difference for considering not similar
	private const float DIFFERENCE_THRESHOLD = 1f; // TODO: Configurable
	private const float TIME_DIFFERENCE_THRESHOLD = 0.03f; // TODO: Configurable

	// I've got no idea if this is a reliable algorithm
	// Compares two points and attempts to make a rough estimate of whether they're similar/redundant
	// based on their keyframe difference and time difference
	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput) =>
		PointLintHelper.AnalyzePoints(value,  outerWarningOutput, (pointDataDictionary, warningOutput) =>
		{

			foreach (var (s, pointDatas) in pointDataDictionary)
			{
				// redundant point checking
				if (pointDatas.Count <= 1)
				{
					continue;
				}

				var prevPoint = pointDatas.First(); // if empty, we have a problem
				for (var i = 1; i < pointDatas.Count; i++)
				{
					var point = pointDatas[i];
					var nextPoint = i + 1 < pointDatas.Count ? pointDatas[i + 1] : null;

					// ignore points who have different easing or smoothness since those can
					// be considered not similar even with small time differences
					if (point.Smooth != prevPoint.Smooth || point.Easing != prevPoint.Easing || (nextPoint is not null && (nextPoint.Smooth != point.Smooth || nextPoint.Easing != point.Easing)))
					{
						continue;
					}

					var leftMiddleTimeDifference = MathF.Abs(point.Time - prevPoint.Time);

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
					if (prevPoint.Data.AreFloatsSimilar(point.Data, DIFFERENCE_THRESHOLD)
					    &&
						    // time difference is small
						    leftMiddleTimeDifference <= TIME_DIFFERENCE_THRESHOLD
					    // if no point after this
					    // or if the next point datas are similar to the middle, which makes the middle redundant
					    && (nextPoint is null || nextPoint.Data.AreFloatsSimilar(point.Data, DIFFERENCE_THRESHOLD))
					   )
					{
						var message = $"Point data {s} are too similar relative to the time difference {DIFFERENCE_THRESHOLD}: Point1 {prevPoint.Data.ArrayToString()}:{prevPoint.Time} " +
						              $"Point2: {point.Data.ArrayToString()}:{point.Time}";

						if (nextPoint is not null)
						{
							message += $" Point3: {nextPoint.Data.ArrayToString()}:{nextPoint.Time}";
						}

						warningOutput.WriteWarning(message, GetType());
					}

					prevPoint = point;
				}
			}
		});
}