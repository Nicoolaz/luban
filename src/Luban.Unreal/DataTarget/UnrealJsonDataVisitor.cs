using System.Text.Json;
using Luban.DataLoader;
using Luban.DataLoader.Builtin;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Utils;

namespace Luban.Unreal.DataTarget;

public class UnrealJsonDataVisitor : IDataActionVisitor<Utf8JsonWriter>
{
    public static UnrealJsonDataVisitor Ins { get; } = new();

    public void Accept(DBool type, Utf8JsonWriter x)
    {
        x.WriteBooleanValue(type.Value);
    }

    public void Accept(DByte type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DShort type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DInt type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DLong type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DFloat type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DDouble type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public virtual void Accept(DEnum type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }

    public void Accept(DString type, Utf8JsonWriter x)
    {
        x.WriteStringValue(type.Value);
    }

    public virtual void Accept(DDateTime type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.UnixTimeOfCurrentContext());
    }
    
    //YK Begin
    public void Accept(DUint type, Utf8JsonWriter x)
    {
        x.WriteNumberValue(type.Value);
    }
    //YK End

    public virtual void Accept(DBean type, Utf8JsonWriter x)
    {
        x.WriteStartObject();

        if (type.Type.IsAbstractType)
        {
            throw new NotSupportedException("unreal json data table does not support abstract type");
            x.WritePropertyName(FieldNames.JsonTypeNameKey);
            x.WriteStringValue(DataUtil.GetImplTypeName(type));
        }
        
        string indexName = "";
        if (type.ImplType.IsTableBean)
        {
            
            var table = type.ImplType.ConnectedTable;
            if (table.Mode == TableMode.ONE)
            {
                x.WritePropertyName(ConstStrings.DataTableNameField);
                x.WriteStringValue(type.ImplType.Name);
            }
            else
            {
                indexName = type.ImplType.ConnectedTable.Index ?? "";
                if (string.IsNullOrEmpty(indexName))
                {
                    throw new Exception($"table : {table.Name} does has an single index key!!");
                }
            }
            
        }

        var defFields = type.ImplType.HierarchyFields;
        int index = 0;
        foreach (var d in type.Fields)
        {
            var defField = (DefField)defFields[index++];

            // 特殊处理 bean 多态类型
            // 另外，不生成  xxx:null 这样
            if (d == null || !defField.NeedExport())
            {
                //x.WriteNullValue();
            }
            else
            {
                x.WritePropertyName(defField.Name);
                d.Apply(this, x);
            }

            //为DataTable添加Name字段
            if (!string.IsNullOrEmpty(indexName) && defField.Name == indexName)
            {
                x.WritePropertyName(ConstStrings.DataTableNameField);
                d.Apply(this, x);
            }
        }
        x.WriteEndObject();
    }

    public void WriteList(List<DType> datas, Utf8JsonWriter x)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Apply(this, x);
        }
        x.WriteEndArray();
    }

    public void Accept(DArray type, Utf8JsonWriter x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DList type, Utf8JsonWriter x)
    {
        WriteList(type.Datas, x);
    }

    public void Accept(DSet type, Utf8JsonWriter x)
    {
        WriteList(type.Datas, x);
    }

    public virtual void Accept(DMap type, Utf8JsonWriter x)
    {
        x.WriteStartArray();
        foreach (var d in type.Datas)
        {
            x.WriteStartArray();
            d.Key.Apply(this, x);
            d.Value.Apply(this, x);
            x.WriteEndArray();
        }
        x.WriteEndArray();
    }
}
