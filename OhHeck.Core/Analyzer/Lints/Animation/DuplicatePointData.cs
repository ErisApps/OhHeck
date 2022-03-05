﻿using System;
using System.Linq;
using OhHeck.Core.Helpers.Enumerable;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("duplicate-point-data")]
public class DuplicatePointData : IBeatmapAnalyzer {

	// Checks if points are duplicated
	public void Validate(Type fieldType, object? value, IWarningOutput warningOutput) =>
		PointLintHelper.AnalyzePoints(value, warningOutput, pointDataDictionary =>
		{
			foreach (var (s, pointDatas) in pointDataDictionary)
			{
				var prevPoint = pointDatas.First(); // if empty, we have a problem
				for (var i = 1; i < pointDatas.Count; i++)
				{
					var point = pointDatas[i];

					// Both points are identical
					if (prevPoint.Data.AreArrayElementsIdentical(point.Data))
					{
						warningOutput.WriteWarning($"Point data {s} are identical: Point1 {prevPoint.Data.ArrayToString()}:{prevPoint.Time} Point2: {point.Data.ArrayToString()}:{point.Time}");
					}

					prevPoint = point;
				}
			}
		});
}