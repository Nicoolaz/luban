using System.Text;
using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class DotNetSqliteDeserializeVisitor : DecoratorFuncVisitor<string, string, int, string>
{
    public static new DotNetSqliteDeserializeVisitor Ins { get; } = new DotNetSqliteDeserializeVisitor();
    public override string DoAccept(TType type, string fieldName, string readerName, int index)
    {
        if (type.IsNullable)
        {
            return $"if({readerName}.IsDBNull({index})){{ {fieldName} = null; }}else{{ {type.Apply(DotNetSqliteUnderlyingDeserializeVisitor.Ins, fieldName, readerName, index)} }}";
        }
        else
        {
            return type.Apply(DotNetSqliteUnderlyingDeserializeVisitor.Ins, fieldName, readerName, index);
        }
    }
}
