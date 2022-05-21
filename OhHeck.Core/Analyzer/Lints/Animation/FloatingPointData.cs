using System;
using System.Collections.Generic;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

[BeatmapWarning("floating-point-data")]
public class FloatingPointData : IFieldOptimizer
{
	[WarningConfigProperty("decimals")]
	private uint _decimals = 4;

	[WarningConfigProperty("time_decimals")]
	private uint _timeDecimals = 5;

	private void IteratePoints(IReadOnlyDictionary<string, List<PointData>> pointDataDictionary)
	{

		foreach (var (_, pointDatas) in pointDataDictionary)
		{
			// redundant point checking
			if (pointDatas.Count <= 1)
			{
				continue;
			}

			foreach (var pointData in pointDatas)
			{
				for (var i = 0; i < pointData.Data.Length; i++)
				{
					pointData.Data[i] = MathF.Round(pointData.Data[i], (int) _decimals);
				}

				pointData.Time = MathF.Round(pointData.Time, (int) _timeDecimals);
			}
		}
	}

	public void Optimize(ref object? value) =>
		PointLintHelper.AnalyzePoints(this, ref value,
			static (pointDataDictionary, self) =>
				self.IteratePoints(pointDataDictionary));
}