using Luban.Datas;
using Luban.TypeVisitors;
using Luban.Types;
using Luban.Defs;

namespace Luban.DataExporter.Builtin.SQLite;

public class SQLiteTypeVisitor : ITypeFuncVisitor<string>
{
    public static new SQLiteTypeVisitor Ins { get; } = new();

    private string _nullStr(TType type)
    {
        return type.IsNullable ? "" : "NOT NULL";
    }
    
    public string Accept(TBool type)
    {
        return $"BOOLEAN {_nullStr(type)} ";
    }

    public string Accept(TByte type)
    {
        return $"INT8 {_nullStr(type)} ";
    }

    public string Accept(TShort type)
    {
        return $"SMALLINT {_nullStr(type)} ";
    }

    public string Accept(TInt type)
    {
        return $"INT {_nullStr(type)} ";
    }

    public string Accept(TLong type)
    {
        return $"BIGINT {_nullStr(type)} ";
    }

    public string Accept(TFloat type)
    {
        return $"FLOAT {_nullStr(type)} ";
    }

    public string Accept(TDouble type)
    {
        return $"DOUBLE {_nullStr(type)} ";
    }

    public string Accept(TEnum type)
    {
        return $"INT {_nullStr(type)} ";
    }

    public string Accept(TString type)
    {
        return $"TEXT {_nullStr(type)} ";
    }

    public string Accept(TDateTime type)
    {
        return $"DATETIME {_nullStr(type)} ";
    }

    public string Accept(TBean type)
    {
        return $"BOLB {_nullStr(type)} ";
    }

    public string Accept(TArray type)
    {
        return $"BOLB {_nullStr(type)} ";
    }

    public string Accept(TList type)
    {
        return $"BOLB {_nullStr(type)} ";
    }

    public string Accept(TSet type)
    {
        return $"BOLB {_nullStr(type)} ";
    }

    public string Accept(TMap type)
    {
        return $"BOLB {_nullStr(type)} ";
    }

    public string Accept(TUint type)
    {
        return $"INT unsigned {_nullStr(type)} ";
    }
}
