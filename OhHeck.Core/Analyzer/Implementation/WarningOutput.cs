using System.Collections.Generic;

namespace OhHeck.Core.Analyzer.Implementation;

public class WarningOutput : IWarningOutput
{
	private readonly Stack<WarningInfo> _analyzables = new();
	private readonly List<Warning> _warningOutput = new();


	public WarningInfo GetCurrentWarningInfo() => _analyzables.Peek();

	public void PushWarningInfo(WarningInfo warningInfo) => _analyzables.Push(warningInfo);

	public void PopWarningInfo() => _analyzables.Pop();

	public void WriteWarning(string message) => _warningOutput.Add(new Warning(message, _analyzables.Peek()));

	public IEnumerable<Warning> GetWarnings() => _warningOutput;
}