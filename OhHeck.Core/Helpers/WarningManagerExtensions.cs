using System.Collections.Generic;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Helpers;

public static class WarningManagerExtensions
{
	// Nonnull
	public static ICollection<AnalyzeProcessedData> Analyze(this WarningManager warningManager, IAnalyzable analyzable, ICollection<AnalyzeProcessedData>? analyzeDatas = null) => warningManager.Analyze(analyzable, null, analyzable.GetType(), analyzeDatas);
	public static ICollection<AnalyzeProcessedData> Analyze(this WarningManager warningManager, IAnalyzable analyzable, IAnalyzable parent, ICollection<AnalyzeProcessedData>? analyzeDatas = null) => warningManager.Analyze(analyzable, parent, analyzable.GetType(), analyzeDatas);

	public static ICollection<AnalyzeProcessedData> AnalyzeBeatmap(this WarningManager warningManager, BeatmapSaveData beatmapSaveData, ICollection<AnalyzeProcessedData>? analyzeDatas = null)
	{
		analyzeDatas ??= new List<AnalyzeProcessedData>();

		warningManager.Analyze(beatmapSaveData, analyzeDatas);

		// We have to iterate the list of types
		beatmapSaveData.Events.ForEach(x => warningManager.Analyze(x, analyzeDatas));
		beatmapSaveData.Notes.ForEach(x => warningManager.Analyze(x, analyzeDatas));
		beatmapSaveData.Obstacles.ForEach(x => warningManager.Analyze(x, analyzeDatas));

		if (beatmapSaveData.BeatmapCustomData is null)
		{
			return analyzeDatas;
		}

		var customData = beatmapSaveData.BeatmapCustomData;

		customData.CustomEvents?.ForEach(x => warningManager.Analyze(x, analyzeDatas));
		customData.EnvironmentEnhancements?.ForEach(x => warningManager.Analyze(x, analyzeDatas));
		customData.PointDefinitions?.ForEach(x => warningManager.Analyze(x, analyzeDatas));

		return analyzeDatas;
	}
}