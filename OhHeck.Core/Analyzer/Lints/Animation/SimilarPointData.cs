using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Helpers.Enumerable;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("similar-point-data")]
public class SimilarPointData : IFieldAnalyzer, IFieldOptimizer
{
	// The minimum difference for considering not similar
	[WarningConfigProperty("difference_threshold")]
	private float _differenceThreshold = 1f; // 3f;

	[WarningConfigProperty("time_difference_threshold")]
	private float _timeDifferenceThreshold = 0.03f; // 0.1f;

	// I've got no idea if this is a reliable algorithm
	// Compares two points and attempts to make a rough estimate of whether they're similar/redundant
	// based on their keyframe difference and time difference
	public void Validate(Type fieldType, in object? value, IWarningOutput outerWarningOutput) =>
		PointLintHelper.AnalyzePoints(this, value, outerWarningOutput, static (pointDataDictionary, self, warningOutput) =>
		{
			self.IteratePoints(pointDataDictionary, warningOutput, false);
		});

	private void IteratePoints(IReadOnlyDictionary<string, List<PointData>> pointDataDictionary, IWarningOutput? warningOutput,
		bool optimize)
	{

		foreach (var (s, pointDatas) in pointDataDictionary)
		{
			// redundant point checking
			if (pointDatas.Count <= 1)
			{
				continue;
			}

			for (var i = 1; i < pointDatas.Count; i++)
			{
				var prevPoint = pointDatas[i - 1]; // if empty, we have a problem
				var point = pointDatas[i];
				var nextPoint = i + 1 < pointDatas.Count ? pointDatas[i + 1] : null;

				// ignore points who have different easing or smoothness since those can
				// be considered not similar even with small time differences
				if (point.Smooth != prevPoint.Smooth || point.Easing != prevPoint.Easing || (nextPoint is not null && (nextPoint.Smooth != point.Smooth || nextPoint.Easing != point.Easing)))
				{
					continue;
				}

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


				// ReSharper disable once InvertIf
				if (PointSimilar(prevPoint, point)

				    // if no point after this
				    // or if the next point datas are similar to the middle, which makes the middle redundant
				    && (nextPoint is null || PointSimilar(point, nextPoint))
				   )
				{
					if (warningOutput is not null)
					{
						const string message = "Point data {s} are too similar relative to the time difference {differenceThreshold}: Point1 {prevPointData}:{prevPointTime} " +
						                       "Point2: {P2}:{P2t}";
						const string message2 = message + "Point3: {nextPoint}:{nextPointTime}";

						if (nextPoint is not null)
						{
							var args = new object[]
							{
								s,
								_differenceThreshold.ToString(CultureInfo.InvariantCulture),
								prevPoint.Data,
								prevPoint.Time.ToString(CultureInfo.InvariantCulture),
								point.Data,
								point.Time.ToString(CultureInfo.InvariantCulture),
								nextPoint.Data,
								nextPoint.Time.ToString(CultureInfo.InvariantCulture)
							};
							warningOutput.WriteWarning(message2, GetType(), args.ToArray());
						}
						else
						{
							var args = new object[]
							{
								s,
								_differenceThreshold.ToString(CultureInfo.InvariantCulture),
								prevPoint.Data,
								prevPoint.Time.ToString(CultureInfo.InvariantCulture),
								point.Data,
								point.Time.ToString(CultureInfo.InvariantCulture)
							};

							warningOutput.WriteWarning(message, GetType(), args.ToArray());
						}
					}

					if (!optimize)
					{
						continue;
					}

					// remove middle if chekcing 3 points
					// remove previous if only 2 points checked
					pointDatas.RemoveAt(nextPoint is not null ? i : i - 1);
					i--;
				}
			}
		}
	}

	public void Optimize(ref object? value) =>
		PointLintHelper.AnalyzePoints(this, ref value,
			static (pointDataDictionary, self) =>
				self.IteratePoints(pointDataDictionary, null,
					true));

	private bool PointSimilar(PointData a, PointData b) =>
		// Both points are identical
		a.Data.AreFloatsSimilar(b.Data, _differenceThreshold)
		// time difference is small
		&& Math.Abs(a.Time - b.Time) <= _timeDifferenceThreshold;
}