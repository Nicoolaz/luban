using Microsoft.Data.Sqlite;
using Luban.DataExporter.Builtin.Binary;
using Luban.Datas;
using Luban.DataVisitors;
using Luban.Defs;
using Luban.Serialization;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.SQLite;

public class SQLiteDataVisiter: IDataActionVisitor<SqliteParameter>
{

    public static new SQLiteDataVisiter Ins { get; } = new SQLiteDataVisiter();
    
    public void Accept(DBool type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DByte type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DShort type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DInt type, SqliteParameter x)
    {
         x.Value = type.Value;
    }

    public void Accept(DLong type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DFloat type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DDouble type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DEnum type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DString type, SqliteParameter x)
    {
        x.Value = type.Value;
    }

    public void Accept(DDateTime type, SqliteParameter x)
    {
        x.Value = type.UnixTimeOfCurrentContext;
    }

    public void Accept(DBean type, SqliteParameter x)
    {
        ByteBuf bytes = new ByteBuf();
        type.Apply(BinaryDataVisitor.Ins, bytes);
        x.Value = bytes.Bytes;
    }

    public void Accept(DArray type, SqliteParameter x)
    {
        ByteBuf bytes = new ByteBuf();
        type.Apply(BinaryDataVisitor.Ins, bytes);
        x.Value = bytes.Bytes;
    }

    public void Accept(DList type, SqliteParameter x)
    {
        ByteBuf bytes = new ByteBuf();
        type.Apply(BinaryDataVisitor.Ins, bytes);
        x.Value = bytes.Bytes;
    }

    public void Accept(DSet type, SqliteParameter x)
    {
        ByteBuf bytes = new ByteBuf();
        type.Apply(BinaryDataVisitor.Ins, bytes);
        x.Value = bytes.Bytes;
    }

    public void Accept(DMap type, SqliteParameter x)
    {
        ByteBuf bytes = new ByteBuf();
        type.Apply(BinaryDataVisitor.Ins, bytes);
        x.Value = bytes.Bytes;
    }

    public void Accept(DUint type, SqliteParameter x)
    {
        x.Value = type.Value;
    }
}
