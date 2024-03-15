using Luban.CSharp.TypeVisitors;
using Luban.Defs;
using Luban.Types;
using Scriban.Runtime;

namespace Luban.CSharp.TemplateExtensions;

public class CsharpSqliteTemplateExtension : ScriptObject
{
    public static string Deserialize(string readerName, string fieldName, TType type, int index)
    {
        return type.Apply(DotNetSqliteDeserializeVisitor.Ins, fieldName, readerName, index);
    }

    public static string DeserializeByteBuf(string bufName, string fieldName, TType type)
    {
        return type.Apply(BinaryDeserializeVisitor.Ins, bufName, fieldName);
    }

    public static string DeserializeByteBufTabtoy(string buffName, string fieldName, TType type)
    {
        return type.Apply(BinaryDeserializerTabtoy.Ins, buffName, fieldName);
    }

    public static bool IsTableIndexField(DefBean bean, DefField field)
    {
        if (bean.ConnectedTable != null)
        {
            return bean.ConnectedTable.IndexList.Any(t => t.IndexField.Name == field.Name);
        }

        return false;
    }
}
