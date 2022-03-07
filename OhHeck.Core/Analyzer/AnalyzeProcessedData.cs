using System;

namespace OhHeck.Core.Analyzer;

public record AnalyzeProcessedData(Type MemberType, object? Value, WarningContext? WarningContext);