using Luban.Types;
using Luban.TypeVisitors;
using Luban.Unreal.TemplateExtensions;

namespace Luban.Unreal.TypeVisitors;

public class UnrealFileNameVisitor:DecoratorFuncVisitor<string>
{
    public static UnrealFileNameVisitor Ins { get; } = new();
    public override string DoAccept(TType type)
    {
        return type.Apply(UnrealTypeNameVisitor.Ins);
    }

    public override string Accept(TBean type)
    {
        return $"{EnvManager.Current.GetOptionOrDefault("", ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix)}/{UnrealTemplateExtension.MakeCppNameWithoutPerfix(type.DefBean)}";
    }

    public override string Accept(TEnum type)
    {
        return $"{EnvManager.Current.GetOptionOrDefault("", ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix)}/{UnrealTemplateExtension.MakeCppNameWithoutPerfix(type.DefEnum)}";
    }

    public override string Accept(TString type)
    {
        return type.GetTag("header");
    }
}
