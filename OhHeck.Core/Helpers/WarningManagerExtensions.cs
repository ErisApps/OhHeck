using System.Collections.Generic;
using System.Threading.Tasks;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Helpers;

public static class WarningManagerExtensions
{
	// Nonnull
	public static void Analyze(this WarningManager warningManager, IAnalyzable analyzable, IWarningOutput warningOutput) => warningManager.Analyze(analyzable, null, analyzable.GetType(), warningOutput);
	public static void Analyze(this WarningManager warningManager, IAnalyzable analyzable, IAnalyzable parent, IWarningOutput warningOutput) => warningManager.Analyze(analyzable, parent, analyzable.GetType(), warningOutput);

	public static async Task AnalyzeBeatmap(this WarningManager warningManager, BeatmapSaveData beatmapSaveData, IWarningOutput warningOutput)
	{
		var tasks = new List<Task>();
		warningManager.Analyze(beatmapSaveData, warningOutput);

		// We have to iterate the list of types
		tasks.Add(Task.Run(() => beatmapSaveData.Events.ForEach(x => warningManager.Analyze(x, warningOutput))));
		tasks.Add(Task.Run(() => beatmapSaveData.Notes.ForEach(x => warningManager.Analyze(x, warningOutput))));
		tasks.Add(Task.Run(() => beatmapSaveData.Obstacles.ForEach(x => warningManager.Analyze(x, warningOutput))));

		if (beatmapSaveData.BeatmapCustomData is null)
		{
			return;
		}

		var customData = beatmapSaveData.BeatmapCustomData;

		tasks.Add(Task.Run(() => customData.CustomEvents?.ForEach(x => warningManager.Analyze(x, warningOutput))));
		tasks.Add(Task.Run(() => customData.EnvironmentEnhancements?.ForEach(x => warningManager.Analyze(x, warningOutput))));
		tasks.Add(Task.Run(() => customData.PointDefinitions?.ForEach(x => warningManager.Analyze(x, warningOutput))));

		await Task.WhenAll(tasks);
	}
}