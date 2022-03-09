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
	public static void AnalyzePoints<T>(T @this, object? value, IWarningOutput warningOutput, Action<IReadOnlyDictionary<string, List<PointData>>, T, IWarningOutput> analyzeFn)
	{
		if (value is List<BeatmapCustomEvent> customEvents)
		{
			foreach (var beatmapCustomEvent in customEvents)
			{
				if (beatmapCustomEvent is not AnimateEvent animateEvent)
				{
					continue;
				}

				var friendlyName = beatmapCustomEvent.GetFriendlyName();


				warningOutput.PushWarningInfo(new WarningContext(friendlyName, warningOutput.GetCurrentWarningInfo().MemberLocation, warningOutput.GetCurrentWarningInfo().Parent));
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
}