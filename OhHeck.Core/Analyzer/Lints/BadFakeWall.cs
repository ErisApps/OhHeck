using System;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Models.beatmap.v3;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.Lints;

[BeatmapWarning("bad-fake-wall")]
public class BadFakeWall : IFieldAnalyzer {

	public void Validate(Type fieldType, object? value, IWarningOutput outerWarningOutput)
	{
		if (value is not ObstacleData obstacleData || obstacleData.ObstacleCustomData is null)
		{
			return;
		}

		var obstacleCustomData = obstacleData.ObstacleCustomData;

		var fakeNullable = obstacleCustomData.Fake?.IsTrue();
		var interactableNullable = obstacleCustomData.Cuttable?.IsTrue();

		var fake = fakeNullable ?? false;
		var interactable = interactableNullable ?? true;

		// if fake == true || interactable == false, fake wall
		// if the counterpart field is not the opposite value, unoptimized wall
		if ((fakeNullable is not null || interactableNullable is not null) && fake != interactable)
		{
			return;
		}

		outerWarningOutput.WriteWarning($"Fake/Uninteractable walls should be \"_fake\": true and \"_interactable\": false. Got _fake {fake} and _interactable {interactable}", GetType());
	}
}