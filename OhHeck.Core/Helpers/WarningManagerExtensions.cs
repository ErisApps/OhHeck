using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Analyzer;

public static class WarningManagerExtensions
{
	public static void AnalyzeBeatmap(this WarningManager warningManager, BeatmapSaveData beatmapSaveData)
	{
		warningManager.Analyze(beatmapSaveData);

		beatmapSaveData.Events.ForEach(warningManager.Analyze);
		beatmapSaveData.Notes.ForEach(warningManager.Analyze);
		beatmapSaveData.Obstacles.ForEach(warningManager.Analyze);

		if (beatmapSaveData.BeatmapCustomData is null)
		{
			return;
		}

		var customData = beatmapSaveData.BeatmapCustomData;
		warningManager.Analyze(customData);

		customData.CustomEvents?.ForEach(warningManager.Analyze);
		customData.EnvironmentEnhancements?.ForEach(warningManager.Analyze);
		customData.PointDefinitions?.ForEach(warningManager.Analyze);
	}
}