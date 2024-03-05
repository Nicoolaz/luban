using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class UnderlyingDeclaringKeyFieldNameVisitorTabtoy : ITypeFuncVisitor<string, string, string>
{
    public static UnderlyingDeclaringKeyFieldNameVisitorTabtoy Ins { get; } = new();

    private string _accept(TType type, string fieldRoot, string fieldName)
    {
        return $"{fieldRoot}.{fieldName}";
    }
    public string Accept(TBool type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TByte type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TShort type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TInt type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TLong type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TFloat type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TDouble type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public virtual string Accept(TEnum type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TString type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TBean type, string fieldRoot, string fieldName)
    {
        //return type.DefBean.TypeNameWithTypeMapper() ?? (type.DefBean.FullTypeName);
        var fileds = type.DefBean.Fields.Select(f => $"{fieldRoot}.{fieldName}.{f.Name}");
        return $"({string.Join(',', fileds)})";
    }

    public string Accept(TArray type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TList type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TSet type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public string Accept(TMap type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }

    public virtual string Accept(TDateTime type, string fieldRoot, string fieldName)
    {
        return _accept(type, fieldRoot, fieldName);
    }
}
