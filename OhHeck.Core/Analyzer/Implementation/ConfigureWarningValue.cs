namespace OhHeck.Core.Analyzer.Implementation;

public class ConfigureWarningValue
{
	public static ConfigureWarningValue FromString(string s)
	{
		// warningName:propName=value
		var warningPropName = s.Split(":", 2); // [warningName, propName=value]
		var warningName = warningPropName[0]; // warningName
		var propValueRaw = warningPropName[1].Split("="); // [propName, value]
		var propName = propValueRaw[0]; // propName
		var propValue = propValueRaw[1]; // value

		return new ConfigureWarningValue(warningName, propName, propValue);
	}

	public ConfigureWarningValue(string warningName, string property, string value)
	{
		WarningName = warningName;
		Property = property;
		Value = value;
	}

	public string WarningName { get; }

	public string Property { get; }

	public string Value { get; }
}