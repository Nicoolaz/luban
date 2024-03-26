using System.Text;
using Luban.CodeFormat;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.TemplateExtensions;

public class TypeTemplateExtension : ScriptObject
{
    public static bool NeedMarshalBoolPrefix(TType type)
    {
        return type.IsNullable;
    }

    public static string FormatMethodName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatMethod(name);
    }

    public static string FormatFieldName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatField(name);
    }

    public static string FormatPropertyName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatProperty(name);
    }

    public static string FormatEnumItemName(ICodeStyle codeStyle, string name)
    {
        return codeStyle.FormatEnumItemName(name);
    }
//YK Begin 修改Ref生成规则

    //给模板调用的方法
    public static DefTable GetRefTableEx(DefField field)
    {
        var (cfgTable, indexInfo) = GetRefTable(field);

        return cfgTable;
    }

    //给模板调用的方法
    public static IndexInfo GetIndexInfoEx(DefField field)
    {
        var (cfgTable, indexInfo) = GetRefTable(field);

        return indexInfo;
    }

    public static DefTable GetCollectionRefTableEx(DefField field)
    {
        var (cfgTable, indexInfo) = GetCollectionRefTable(field);

        return cfgTable;
    }
    
    public static IndexInfo GetCollectionIndexInfoEx(DefField field)
    {
        var (cfgTable, indexInfo) = GetCollectionRefTable(field);

        return indexInfo;
    }
    public static bool CanGenerateRef(DefField field)
    {
        if (field.CType.IsCollection)
        {
            return false;
        }
        
        var(cfgTable, indexInfo) = GetRefTable(field);

        return cfgTable != null && indexInfo != null;
    }

    public static bool CanGenerateCollectionRef(DefField field)
    {
        if (!field.CType.IsCollection)
        {
            return false;
        }

        var (cfgTable, indexInfo) = GetCollectionRefTable(field);
        return cfgTable != null && indexInfo != null;
    }

    public static (DefTable, IndexInfo) GetCollectionRefTable(DefField field)
    {
        var refTag = field.CType.GetTag("ref");
        if (refTag == null)
        {
            refTag = field.CType.ElementType.GetTag("ref");
        }
        if (refTag == null)
        {
            return (null, null);
        }
        return GetRefTable(refTag);
    }

    public static (DefTable, IndexInfo) GetRefTable(string refTag)
    {
        var (tableName, fieldName, ignoreDefault) = DefUtil.ParseRefString(refTag);
        if (GenerationContext.Current.Assembly.GetCfgTable(tableName) is { } cfgTable)
        {
            switch (cfgTable.Mode)
            {
                case(TableMode.MAP):
                    return (cfgTable, cfgTable.IndexList[0]);
                case TableMode.ONE:
                    //单例表不支持ref导出
                    return (null, null);
                case TableMode.LIST:
                {
                    var indexInfo = cfgTable.IndexList.Find(i => !i.IsUnionIndex && i.IndexField.Name == fieldName);
                    if (indexInfo != null)
                    {
                        return (cfgTable, indexInfo);
                    }
                    else
                    {
                        return (null, null);
                    }
                }
            }
        }
        return (null, null);
    }
    public static (DefTable, IndexInfo) GetRefTable(DefField field)
    {
        var refTag = field.CType.GetTag("ref");
        if (refTag == null)
        {
            return (null, null);
        }

        return GetRefTable(refTag);
    }

    public static TType GetRefType(DefField field)
    {
        var (cfgTable, indexInfo) = GetRefTable(field);
        if (indexInfo == null) return null;

        TType result = null;
        if (indexInfo.IsListMode)
        {
            result = TList.Create(false, null, cfgTable.ValueTType, false);
        }
        else
        {
            result = cfgTable.ValueTType;
        }
        return result;
    }
    //YK End

    public static bool IsFieldBeanNeedResolveRef(DefField field)
    {
        return field.CType is TBean bean && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool IsFieldArrayLikeNeedResolveRef(DefField field)
    {
        return field.CType.ElementType is TBean bean && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType && field.CType is not TMap;
    }

    public static bool IsFieldMapNeedResolveRef(DefField field)
    {
        return field.CType is TMap { ValueType: TBean bean } && bean.DefBean.TypeMappers == null && !bean.DefBean.IsValueType;
    }

    public static bool HasIndex(DefField field)
    {
        TType type = field.CType;
        return type.HasTag("index") && type is TArray or TList;
    }

    public static string GetIndexName(DefField field)
    {
        return field.CType.GetTag("index");
    }

    public static DefField GetIndexField(DefField field)
    {
        string indexName = GetIndexName(field);
        return ((TBean)field.CType.ElementType).DefBean.GetField(indexName);
    }

    public static TMap GetIndexMapType(DefField field)
    {
        DefField indexField = GetIndexField(field);
        return TMap.Create(false, null, indexField.CType, field.CType.ElementType, false);
    }

    public static string ImplDataType(DefBean type, DefBean parent)
    {
        return DataUtil.GetImplTypeName(type, parent);
    }

    public static string EscapeComment(string comment)
    {
        return System.Web.HttpUtility.HtmlEncode(comment).Replace("\n", "<br/>");
    }
}
