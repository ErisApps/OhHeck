namespace OhHeck.Core.Analyzer;

public record WarningInfo(string Type, string MemberLocation, IAnalyzable? parent);
public record Warning(string Message, WarningInfo WarningInfo);