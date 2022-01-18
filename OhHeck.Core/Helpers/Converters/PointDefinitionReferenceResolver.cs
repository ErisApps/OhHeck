using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using OhHeck.Core.Models.ModData.Tracks;

namespace OhHeck.Core.Helpers.Converters;

public class PointDefinitionReferenceResolver : ReferenceResolver
{
	private readonly Dictionary<string, PointDefinitionData> _pointDefinitionDatas = new();

	public override void AddReference(string referenceId, object value)
	{
		if (value is not PointDefinitionData pointDefinitionData || !_pointDefinitionDatas.TryAdd(referenceId, pointDefinitionData))
		{
			throw new JsonException();
		}
	}

	public override string GetReference(object value, out bool alreadyExists)
	{
		if (value is not PointDefinitionData { Name: null } pointDefinitionData)
		{
			throw new JsonException();
		}

		alreadyExists = _pointDefinitionDatas.ContainsKey(pointDefinitionData.Name!);

		return pointDefinitionData.Name!;
	}

	public override PointDefinitionData ResolveReference(string referenceId)
	{
		if (!_pointDefinitionDatas.TryGetValue(referenceId, out var value))
		{
			throw new JsonException();
		}

		return value;
	}
}

public class PointDefinitionReferenceHandler : ReferenceHandler
{
	public PointDefinitionReferenceHandler() => _rootedResolver = new PointDefinitionReferenceResolver();

	private ReferenceResolver _rootedResolver;
	public override ReferenceResolver CreateResolver() => _rootedResolver;
	public void Reset() => _rootedResolver = new PointDefinitionReferenceResolver();
}