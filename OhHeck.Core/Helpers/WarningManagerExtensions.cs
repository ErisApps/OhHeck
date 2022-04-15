using System.Collections.Generic;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.beatmap.v3;

namespace OhHeck.Core.Helpers;

public static class WarningManagerExtensions
{
	// Nonnull
	public static ICollection<AnalyzeProcessedData> AnalyzeBeatmap(this WarningManager warningManager, BeatmapSaveData beatmapSaveData, ICollection<AnalyzeProcessedData>? analyzeDatas = null)
	{
		analyzeDatas ??= new List<AnalyzeProcessedData>();

		warningManager.Analyze(beatmapSaveData, analyzeDatas);

		return analyzeDatas;
	}
}