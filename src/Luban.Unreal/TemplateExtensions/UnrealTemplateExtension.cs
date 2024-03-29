using System.Text;
using Luban.Defs;
using Luban.Types;
using Luban.Unreal.TypeVisitors;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.Unreal.TemplateExtensions;

public class UnrealTemplateExtension : ScriptObject
{
    public static string TypeNamePerfix(DefTypeBase type)
    {
        if (type is DefBean) return "F";

        if (type is DefEnum) return "E";

        return "";
    }
    public static string MakeCppName(DefTypeBase type)
    {
        return TypeUtil.MakeCppFullName("", TypeNamePerfix(type) + type.Name);
    }

    public static string DeclaringTypeName(TType type)
    {
        return type.Apply(UnrealTypeNameVisitor.Ins);
    }
    
    public static string GenerateExtraInclude(DefBean bean)
    {
        var includes = new StringBuilder();
        var names = new HashSet<string>();
        foreach (var field in bean.Fields)
        {
            
            if (field.CType.Apply(TypeNeedsExtraIncludeVisitor.Ins) )
            {
                GetHeaderNames(names, field.CType, includes);
            }
        }
        
        return includes.ToString();
    }

    private static void GetHeaderNames(HashSet<string> names, TType type, StringBuilder sb)
    {
        if (type.IsCollection)
        {
            if (type is TMap)
            {
                GetHeaderNames(names, (type as TMap).KeyType, sb);
                GetHeaderNames(names, (type as TMap).ValueType, sb);
            }
            else
            {
                GetHeaderNames(names, type.ElementType, sb);
            }
        }
        else
        {
            var name = type.Apply(UnrealTypeNameVisitor.Ins);
            if (type.Apply(TypeNeedsExtraIncludeVisitor.Ins) && !names.Contains(name))
            {
                names.Add(name);
                sb.AppendLine($"#include \"{EnvManager.Current.GetOptionOrDefault("", ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix)}/{name}.h\"");
            }
                
        }
        
    }
}
