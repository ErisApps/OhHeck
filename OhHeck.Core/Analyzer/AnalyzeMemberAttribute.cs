using System;

namespace OhHeck.Core.Analyzer;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class AnalyzeMemberAttribute : Attribute
{
	public string Name { get; }

	public AnalyzeMemberAttribute(string name) => Name = name;
}