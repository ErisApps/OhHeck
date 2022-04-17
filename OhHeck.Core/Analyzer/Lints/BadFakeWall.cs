using System;
using OhHeck.Core.Analyzer.Attributes;
using OhHeck.Core.Models.Beatmap;
using OhHeck.Core.Models.Structs;

namespace OhHeck.Core.Analyzer.Lints;

[BeatmapWarning("bad-fake-wall")]
public class BadFakeWall : IFieldAnalyzer,IFieldOptimizer {

	public void Validate(Type fieldType, in object? value, IWarningOutput outerWarningOutput)
	{
		if (value is not ObstacleCustomData obstacleCustomData)
		{
			return;
		}

		var fakeNullable = obstacleCustomData.Fake?.IsTrue();
		var interactableNullable = obstacleCustomData.Cuttable?.IsTrue();

		var fake = fakeNullable ?? false;
		var interactable = interactableNullable ?? true;

		// if fake == true || interactable == false, fake wall
		// if the counterpart field is not the opposite value, unoptimized wall
		if ((fakeNullable is null && interactableNullable is null) || fake != interactable)
		{
			return;
		}

		outerWarningOutput.WriteWarning($"Fake/Uninteractable walls should be \"_fake\": true and \"_interactable\": false. Got _fake {fake} and _interactable {interactable}", GetType());
	}

	public void Optimize(ref object? value)
	{
		if (value is not ObstacleCustomData obstacleCustomData)
		{
			return;
		}

		var fakeNullable = obstacleCustomData.Fake?.IsTrue();
		var interactableNullable = obstacleCustomData.Cuttable?.IsTrue();

		var fake = fakeNullable ?? false;
		var interactable = interactableNullable ?? true;

		// if fake == true || interactable == false, fake wall
		// if the counterpart field is not the opposite value, unoptimized wall
		if ((fakeNullable is null && interactableNullable is null) || fake != interactable)
		{
			return;
		}

		obstacleCustomData.Fake = FakeTruthy.TRUE;
		obstacleCustomData.Cuttable = FakeTruthy.TRUE;
	}
}