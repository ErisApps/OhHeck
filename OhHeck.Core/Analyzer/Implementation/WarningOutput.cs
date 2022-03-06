using System;
using System.Collections.Generic;

namespace OhHeck.Core.Analyzer.Implementation;

public class WarningOutput : IWarningOutput
{
	private readonly Stack<WarningContext> _analyzables = new();
	private readonly List<Warning> _warningOutput = new();


	public WarningContext GetCurrentWarningInfo() => _analyzables.Peek();

	public void PushWarningInfo(WarningContext warningContext) => _analyzables.Push(warningContext);

	public void PopWarningInfo() => _analyzables.Pop();

	public void WriteWarning(string message, Type analyzeType, params object?[]? formatArgs) => _warningOutput.Add(new Warning(message, _analyzables.Peek(), analyzeType, formatArgs?.Length is null or 0 ? null : formatArgs));

	public IEnumerable<Warning> GetWarnings() => _warningOutput;
}