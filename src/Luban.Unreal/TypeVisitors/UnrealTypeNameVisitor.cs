using Luban.Types;
using Luban.TypeVisitors;
using Luban.Unreal.TemplateExtensions;

namespace Luban.Unreal.TypeVisitors;

public class UnrealTypeNameVisitor : ITypeFuncVisitor<string>
{
    public static UnrealTypeNameVisitor Ins { get; } = new UnrealTypeNameVisitor();
    public string Accept(TBool type)
    {
        return "bool";
    }

    public string Accept(TByte type)
    {
        return "uint8";
    }

    public string Accept(TShort type)
    {
        //Type 'int16' is not supported by blueprint.
        return "int32";
    }

    public string Accept(TInt type)
    {
        return "int32";
    }

    public string Accept(TLong type)
    {
        return "int64";
    }

    public string Accept(TFloat type)
    {
        return "float";
    }

    public string Accept(TDouble type)
    {
        return "double";
    }

    public string Accept(TEnum type)
    {
        return UnrealTemplateExtension.MakeCppName(type.DefEnum);
    }

    public string Accept(TString type)
    {
        return "FString";
    }

    public string Accept(TDateTime type)
    {
        return "FDateTime";
    }

    public string Accept(TBean type)
    {
        return UnrealTemplateExtension.MakeCppName(type.DefBean);
    }

    public string Accept(TArray type)
    {
        return $"TArray<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TList type)
    {
        return $"TArray<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TSet type)
    {
        return $"TArray<{type.ElementType.Apply(this)}>";
    }

    public string Accept(TMap type)
    {
        return $"TMap<{type.KeyType.Apply(this)}, {type.ValueType.Apply(this)}>";
    }

    public string Accept(TUint type)
    {
        //Type 'uint32' is not supported by blueprint.
        return "int64";
    }
}
