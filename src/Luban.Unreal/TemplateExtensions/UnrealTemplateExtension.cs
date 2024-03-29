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
        string packageDir = EnvManager.Current.GetOptionOrDefault("", ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix);
        foreach (var field in bean.Fields)
        {
            if (field.CType.Apply(TypeNeedsExtraIncludeVisitor.Ins))
            {
                includes.AppendLine($"#include \"{packageDir}{field.CType.Apply(UnrealTypeNameVisitor.Ins)}.h\"");
            }
        }
        return includes.ToString();
    }
}
