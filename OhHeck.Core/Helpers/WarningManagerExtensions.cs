using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Helpers;

public static class WarningManagerExtensions
{
	// Nonnull
	public static void Analyze(this WarningManager warningManager, IAnalyzable analyzable, IWarningOutput warningOutput) => warningManager.Analyze(analyzable, null, analyzable.GetType(), warningOutput);
	public static void Analyze(this WarningManager warningManager, IAnalyzable analyzable, IAnalyzable parent, IWarningOutput warningOutput) => warningManager.Analyze(analyzable, parent, analyzable.GetType(), warningOutput);

	public static void AnalyzeBeatmap(this WarningManager warningManager, BeatmapSaveData beatmapSaveData, IWarningOutput warningOutput)
	{
		warningManager.Analyze(beatmapSaveData, warningOutput);

		beatmapSaveData.Events.ForEach(x => warningManager.Analyze(x, warningOutput));
		beatmapSaveData.Notes.ForEach(x => warningManager.Analyze(x, warningOutput));
		beatmapSaveData.Obstacles.ForEach(x => warningManager.Analyze(x, warningOutput));

		if (beatmapSaveData.BeatmapCustomData is null)
		{
			return;
		}

		var customData = beatmapSaveData.BeatmapCustomData;
		warningManager.Analyze(customData, warningOutput);

		customData.CustomEvents?.ForEach(x => warningManager.Analyze(x, warningOutput));
		customData.EnvironmentEnhancements?.ForEach(x => warningManager.Analyze(x, warningOutput));
		customData.PointDefinitions?.ForEach(x => warningManager.Analyze(x, warningOutput));
	}
}