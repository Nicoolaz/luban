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
        return UnrealTemplateExtension.MakeCppNameWithoutPerfix(type.DefBean);
    }

    public override string Accept(TEnum type)
    {
        return UnrealTemplateExtension.MakeCppNameWithoutPerfix(type.DefEnum);
    }
}
