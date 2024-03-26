using System.Text;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;

namespace Luban.CSharp.TypeVisitors;

public class DotNetSqliteUnderlyingDeserializeVisitor : ITypeFuncVisitor<string, string, int, string>
{
    public static new DotNetSqliteUnderlyingDeserializeVisitor Ins { get; } = new DotNetSqliteUnderlyingDeserializeVisitor();
    public string Accept(TBool type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetBoolean({index});";
    }

    public string Accept(TByte type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetByte({index});";
    }

    public string Accept(TShort type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetInt16({index});";
    }

    public string Accept(TInt type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetInt32({index});";
    }

    public string Accept(TLong type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetInt64({index});";
    }

    public string Accept(TFloat type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetFloat({index});";
    }

    public string Accept(TDouble type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetDouble({index});";
    }

    public string Accept(TEnum type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = ({type.Apply(UnderlyingDeclaringTypeNameVisitor.Ins)}){readerName}.GetInt32({index});";
    }

    public string Accept(TString type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetString({index});";
    }

    public string Accept(TDateTime type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = {readerName}.GetDateTime({index}).Ticks;";
    }

    public string Accept(TBean type, string fieldName, string readerName, int index)
    {
        if (type.DefBean.IsTableBean)
        {
            return type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, readerName, fieldName, 0);
        }
        else
        {
            var s = new StringBuilder();
            s.AppendLine($"_buf.Replace((byte[]){readerName}[{index}]);");
            s.AppendLine(type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, "_buf", fieldName , 0));
            return s.ToString();
        }
    }

    public string Accept(TArray type, string fieldName, string readerName, int index)
    {
        var s = new StringBuilder();
        s.AppendLine($"_buf.Replace((byte[]){readerName}[{index}]);");
        s.AppendLine(type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, "_buf", fieldName , 0));
        return s.ToString();
    }

    public string Accept(TList type, string fieldName, string readerName, int index)
    {
        var s = new StringBuilder();
        s.AppendLine($"_buf.Replace((byte[]){readerName}[{index}]);");
        s.AppendLine(type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, "_buf", fieldName , 0));
        return s.ToString();
    }

    public string Accept(TSet type, string fieldName, string readerName, int index)
    {
        var s = new StringBuilder();
        s.AppendLine($"_buf.Replace((byte[]){readerName}[{index}]);");
        s.AppendLine(type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, "_buf", fieldName , 0));
        return s.ToString();
    }

    public string Accept(TMap type, string fieldName, string readerName, int index)
    {
        var s = new StringBuilder();
        s.AppendLine($"_buf.Replace((byte[]){readerName}[{index}]);");
        s.AppendLine(type.Apply(BinaryUnderlyingDeserializeVisitor.Ins, "_buf", fieldName , 0));
        return s.ToString();
    }

    public string Accept(TUint type, string fieldName, string readerName, int index)
    {
        return $"{fieldName} = (uint){readerName}.GetInt64({index});";
    }
}
