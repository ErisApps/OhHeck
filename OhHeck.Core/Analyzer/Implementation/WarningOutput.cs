using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace OhHeck.Core.Analyzer.Implementation;

public class WarningOutput : IWarningOutput
{
	private readonly ConcurrentStack<WarningContext> _analyzables = new();
	private readonly ConcurrentBag<Warning> _warningOutput = new();


	public WarningContext GetCurrentWarningInfo()
	{
		return _analyzables.TryPeek(out var r) ? r : throw new InvalidOperationException();
	}

	public void PushWarningInfo(WarningContext warningContext) => _analyzables.Push(warningContext);

	public void PopWarningInfo()
	{
		if (!_analyzables.TryPop(out var r))
		{
			throw new InvalidOperationException();
		}
	}

	public void WriteWarning(string message, Type analyzeType) => _warningOutput.Add(new Warning(message, GetCurrentWarningInfo(), analyzeType));

	public IEnumerable<Warning> GetWarnings() => _warningOutput;
}