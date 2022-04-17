using System;
using System.Collections.Generic;
using OhHeck.Core.Helpers;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Analyzer.Lints.Animation;

public static class PointLintHelper
{
	// we love reusing code and lambdas
	/// <summary>
	///
	/// </summary>
	/// <param name="this">pass in context to avoid allocation micro optimization yay</param>
	/// <param name="value"></param>
	/// <param name="warningOutput"></param>
	/// <param name="analyzeFn"></param>
	/// <typeparam name="T"></typeparam>
	public static void AnalyzePoints<T>(T @this, in object? value, IWarningOutput warningOutput, Action<IReadOnlyDictionary<string, List<PointData>>, T, IWarningOutput> analyzeFn)
	{
		if (value is List<BeatmapCustomEvent> customEvents)
		{
			foreach (var beatmapCustomEvent in customEvents)
			{
				if (beatmapCustomEvent is not AnimateEvent animateEvent)
				{
					continue;
				}

				var (memberName, _, warningContext) = warningOutput.GetCurrentWarningInfo();

				warningOutput.PushWarningInfo(new WarningContext(memberName, beatmapCustomEvent, warningContext));
				// pass in warningOutput to avoid allocation moment
				analyzeFn(PointHelper.GetPointDataDictionary(animateEvent.PointProperties)!, @this, warningOutput);
				warningOutput.PopWarningInfo();
			}
		}

		var pointDataDictionary = PointHelper.GetPointDataDictionary(value);

		if (pointDataDictionary is null)
		{
			return;
		}

		analyzeFn(pointDataDictionary, @this, warningOutput);
	}

	public static void AnalyzePoints<T>(T @this, ref object? value, Action<IReadOnlyDictionary<string, List<PointData>>, T> analyzeFn)
	{
		if (value is List<BeatmapCustomEvent> customEvents)
		{
			foreach (var beatmapCustomEvent in customEvents)
			{
				if (beatmapCustomEvent is not AnimateEvent animateEvent)
				{
					continue;
				}

				// pass in warningOutput to avoid allocation moment
				analyzeFn(PointHelper.GetPointDataDictionary(animateEvent.PointProperties)!, @this);
			}
		}

		var pointDataDictionary = PointHelper.GetPointDataDictionary(value);

		if (pointDataDictionary is null)
		{
			return;
		}

		analyzeFn(pointDataDictionary, @this);
	}
}