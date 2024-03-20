using Luban.Types;

namespace Luban.TypeVisitors;

public class TypeNameVisitor : ITypeFuncVisitor<string>
{
    public static TypeNameVisitor Ins { get; } = new TypeNameVisitor();
    
    private string accept(TType type)
    {
        return type.TypeName;
    }
    public string Accept(TBool type)
    {
        return accept(type);
    }

    public string Accept(TByte type)
    {
        return accept(type);
    }

    public string Accept(TShort type)
    {
        return accept(type);
    }

    public string Accept(TInt type)
    {
        return accept(type);
    }

    public string Accept(TLong type)
    {
        return accept(type);
    }

    public string Accept(TFloat type)
    {
        return accept(type);
    }

    public string Accept(TDouble type)
    {
        return accept(type);
    }

    public string Accept(TEnum type)
    {
        return type.DefEnum.Name;
    }

    public string Accept(TString type)
    {
        return accept(type);
    }

    public string Accept(TDateTime type)
    {
        return accept(type);
    }

    public string Accept(TBean type)
    {
        return type.DefBean.Name;
    }

    public string Accept(TArray type)
    {
        return accept(type);
    }

    public string Accept(TList type)
    {
        return accept(type);
    }

    public string Accept(TSet type)
    {
        return accept(type);
    }

    public string Accept(TMap type)
    {
        return accept(type);
    }

    public string Accept(TUint type)
    {
        return accept(type);
    }
}
