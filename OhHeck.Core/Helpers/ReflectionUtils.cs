using System;
using System.Collections.Generic;
using System.Reflection;
using DryIoc;
using OhHeck.Core.Analyzer;
using OhHeck.Core.Models.Beatmap;

namespace OhHeck.Core.Helpers;

internal static class ReflectionUtils
{
	internal static List<MemberInfo> GetTypeFieldsSuperRecursive(IReflect type, List<MemberInfo>? memberInfos = null)
	{
		memberInfos ??= new List<MemberInfo>();

		while (true)
		{
			var fieldInfos = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
			var propInfos = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			memberInfos.AddRange(fieldInfos);
			memberInfos.AddRange(propInfos);

			var parentType = type.UnderlyingSystemType.BaseType;

			// done
			if (parentType is null || !parentType.IsAssignableTo<IAnalyzable>())
			{
				return memberInfos;
			}

			type = parentType;
		}
	}
}