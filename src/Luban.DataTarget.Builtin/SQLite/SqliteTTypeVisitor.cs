using Luban.Types;
using Luban.TypeVisitors;
using Microsoft.Data.Sqlite;

namespace Luban.DataExporter.Builtin.SQLite;

public class SqliteTTypeVisitor:ITypeFuncVisitor<SqliteType>
{
    public static new SqliteTTypeVisitor Ins { get; } = new SqliteTTypeVisitor();
    public SqliteType Accept(TBool type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TByte type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TShort type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TInt type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TLong type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TFloat type)
    {
        return SqliteType.Real;
    }

    public SqliteType Accept(TDouble type)
    {
        return SqliteType.Real;
    }

    public SqliteType Accept(TEnum type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TString type)
    {
        return SqliteType.Text;
    }

    public SqliteType Accept(TDateTime type)
    {
        return SqliteType.Integer;
    }

    public SqliteType Accept(TBean type)
    {
        return SqliteType.Blob;
    }

    public SqliteType Accept(TArray type)
    {
        return SqliteType.Blob;
    }

    public SqliteType Accept(TList type)
    {
        return SqliteType.Blob;
    }

    public SqliteType Accept(TSet type)
    {
        return SqliteType.Blob;
    }

    public SqliteType Accept(TMap type)
    {
        return SqliteType.Blob;
    }

    public SqliteType Accept(TUint type)
    {
        return SqliteType.Integer;
    }
}
