using System.Collections.Generic;

namespace OhHeck.Core.Analyzer;

public interface IWarningOutput
{
	void PushWarningInfo(WarningInfo warningInfo);
	void PopWarningInfo();

	void WriteWarning(string message);

	IEnumerable<Warning> GetWarnings();
}