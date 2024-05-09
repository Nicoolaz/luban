using Luban.Types;
using Luban.TypeVisitors;
using Luban.Unreal.TemplateExtensions;
namespace Luban.Unreal.DataTarget;

public class UnrealMemberDefaultVisitor:DecoratorFuncVisitor<string>
{
    public static UnrealMemberDefaultVisitor Ins{ get; } = new();

    public override string DoAccept(TType type)
    {
        return "";
    }

    public override string Accept(TBool type)
    {
        return " = false";
    }

    public override string Accept(TByte type)
    {
        return " = 0";
    }

    public override string Accept(TShort type)
    {
        return " = 0";
    }

    public override string Accept(TInt type)
    {
        return " = 0";
    }

    public override string Accept(TLong type)
    {
        return " = 0";
    }

    public override string Accept(TFloat type)
    {
        return " = 0";
    }

    public override string Accept(TDouble type)
    {
        return " = 0";
    }

    public override string Accept(TEnum type)
    {
        string enumName = UnrealTemplateExtension.MakeCppName(type.DefEnum);
        if (type.DefEnum.Items.Count > 0)
        {
            string defaultItemName = type.DefEnum.Items[0].Name;
            return $" = {enumName}::{defaultItemName}";
        }
        
        return $" = {enumName}(0)";
    }

    public override string Accept(TUint type)
    {
        return " = 0";
    }
}
