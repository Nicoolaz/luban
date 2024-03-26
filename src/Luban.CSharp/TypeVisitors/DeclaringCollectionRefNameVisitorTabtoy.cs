using Luban.Defs;
using Luban.TemplateExtensions;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class DeclaringCollectionRefNameVisitorTabtoy : ITypeFuncVisitor<string>
{
    public static DeclaringCollectionRefNameVisitorTabtoy Ins { get; } = new();
    public string Accept(TBool type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TByte type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TShort type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TInt type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TLong type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TFloat type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDouble type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TEnum type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TString type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TDateTime type)
    {
        throw new NotImplementedException();
    }

    public string Accept(TBean type)
    {
        throw new NotImplementedException();
    }
//YK Begin
    public string Accept(TArray type)
    {
        var (refTable, indexInfo) = GetCollectionRefTable(type);
        if (refTable != null && indexInfo != null)
        {
            if (indexInfo.IsListMode)
            {
                var listType = TList.Create(false, null, indexInfo.IndexField.CType, false);
                return listType.Apply(DeclaringTypeNameVisitorTabtoy.Ins) + "[]";
            }
            else
            {
                return indexInfo.IndexField.CType.Apply(DeclaringTypeNameVisitorTabtoy.Ins) + "[]";
            }
        }
        throw new Exception($"解析'{type.ElementType}[]' 的ref失败");
    }

    public string Accept(TList type)
    {
        var (refTable, indexInfo) = GetCollectionRefTable(type);
        if (refTable != null && indexInfo != null)
        {
            if (indexInfo.IsListMode)
            {
                var listType = TList.Create(false, null, indexInfo.IndexField.CType, false);
                return $"{ConstStrings.ListTypeName}<{listType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}>";
            }
            else
            {
                return $"{ConstStrings.ListTypeName}<{indexInfo.IndexField.CType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}>";
            }
            
        }
        throw new Exception($"解析'{ConstStrings.ListTypeName}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TSet type)
    {
        var (refTable, indexInfo) = GetCollectionRefTable(type);
        if (refTable != null && indexInfo != null)
        {
            if (indexInfo.IsListMode)
            {
                var listType = TList.Create(false, null, indexInfo.IndexField.CType, false);
                return $"{ConstStrings.HashSetTypeName}<{listType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}>";
            }
            else
            {
                return $"{ConstStrings.HashSetTypeName}<{indexInfo.IndexField.CType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}>";
            }
        }
        throw new Exception($"解析'{ConstStrings.HashSetTypeName}<{type.ElementType}>' 的ref失败");
    }

    public string Accept(TMap type)
    {
        var (refTable, indexInfo) = GetCollectionRefTable(type);
        if (refTable != null && indexInfo != null)
        {
            if (indexInfo.IsListMode)
            {
                var listType = TList.Create(false, null, indexInfo.IndexField.CType, false);
                return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}, {listType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}>";
            }
            else
                return $"{ConstStrings.HashMapTypeName}<{type.KeyType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}, {indexInfo.IndexField.CType.Apply(DeclaringTypeNameVisitorTabtoy.Ins)}>";
        }
        throw new Exception($"解析'{ConstStrings.HashMapTypeName}<{type.KeyType}, {type.ValueType}>' 的ref失败");
    }

    
    public string Accept(TUint type)
    {
        throw new NotImplementedException();
    }
    

    private static (DefTable, IndexInfo) GetCollectionRefTable(TType type)
    {
        var refTag = type.GetTag("ref");
        if (refTag == null)
        {
            refTag = type.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return (null, null);
        }

        return TypeTemplateExtension.GetRefTable(refTag);
    }
    //YK End
}
