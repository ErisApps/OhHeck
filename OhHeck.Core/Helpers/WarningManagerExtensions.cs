using System.Collections.Generic;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.beatmap.v3;

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

		return analyzeDatas;
	}
}