using System.Data;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Text;
using Luban.Defs;
using Luban.Types;
using Luban.Utils;

namespace Luban.DataExporter.Builtin.SQLite;

public static class SQLiteUtil
{
    public static bool IsKeyIndex(string keyName, DefTable t)
    {
        return t.IndexList.Any(t => t.IndexField.Name == keyName);
    }

    public static bool IsSqlitePrimaryKey(string keyName, DefTable t)
    {
        if (t.IndexList == null) return false;
        for (int i = 0; i < t.IndexList.Count; i++)
        {
            if (!t.IndexList[i].Type.Apply(CanBeSQLiteKeyVisitor.Ins))
            {
                continue;
            }

            return t.IndexList[i].IndexField.Name == keyName;
        }

        return false;
    }

    public static string PrimaryKey(string keyName, DefTable t)
    {
        return IsSqlitePrimaryKey(keyName, t) ? "PRIMARY KEY" : "";
    }
    
    public static SqliteConnection CreateAndConnectToDB(string filePath)
    {
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }

        SqliteConnection connection = new SqliteConnection($"Data Source={filePath};");
        connection.Open();
        return connection;
    }

    public static void CreateTable(SqliteConnection connection, DefTable table)
    {
        //lock (connection)
        {
            using (var command = connection.CreateCommand())
            {
                List<string> tKeys = new List<string>();
                foreach (var key in table.ValueTType.DefBean.Fields)
                {
                    if(!key.NeedExport())continue;
                    if (IsKeyIndex(key.Name, table) && key.CType.IsBean)
                    {
                        foreach (var field in ((TBean)key.CType).DefBean.Fields)
                        {
                            if(!field.NeedExport()) continue;
                            tKeys.Add($"{field.Name} {field.CType.Apply(SQLiteTypeVisitor.Ins)}");
                        }
                    }
                    else
                    {
                        tKeys.Add($"{key.Name} {key.CType.Apply(SQLiteTypeVisitor.Ins)} {PrimaryKey(key.Name, table)}");
                    }
                    
                }

                try
                {
                    command.CommandText = $"CREATE TABLE {table.Name}({string.Join(',', tKeys)})";
                    command.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    throw e;
                }
                
                //transaction.Commit();

                if (table.IndexList.Count > 1)
                {
                    //using (var indexCommand = new SqliteCommand(connection))
                    {
                        foreach (var index_field in table.IndexList)
                        {
                            if (!IsSqlitePrimaryKey(index_field.IndexField.Name, table))
                            {
                                var index = index_field.IndexField.Name;
                                if (index_field.Type.IsBean)
                                {
                                    index = string.Join(',', ((TBean)index_field.Type).DefBean.Fields.Select(t => t.Name));
                                }
                                
                                command.CommandText = $"CREATE INDEX IF NOT EXISTS idx_{index_field.IndexField.Name} ON {table.Name}({index})";
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }
    }

    public static void DeleteTable(SqliteConnection connection, string tableName)
    {
        //lock (connection)
        {
            //using (SqliteTransaction transaction = connection.BeginTransaction())
            {
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = $"DROP TABLE IF EXISTS {tableName}";
                    command.ExecuteNonQuery();
                }
                //transaction.Commit();
            } 
        }
        
    }

    public static SqliteCommand GetInsertDataCommand(SqliteConnection connection, DefTable table)
    {
        var command = connection.CreateCommand();

        command.CommandText = $"INSERT INTO {table.Name} VALUES ( {string.Join(',', table.ValueTType.DefBean.Fields.Where(t=>t.NeedExport()).Select(t => {
                if (IsKeyIndex(t.Name, table) && t.CType.IsBean) {
                    return string.Join(',', ((TBean)t.CType).DefBean.Fields.Where(t => 
                        t.NeedExport()).Select(t=>$"@{t.Name}") 
                    );
                }
                return $"@{t.Name}";
        }))} ) ";

        return command;
    }

    public static bool IsTableExists(SqliteConnection connection, string tableName)
    {
        //lock (connection)
        {
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{tableName}'";
                var da = command.ExecuteReader();
                var ds = new DataTable();
                ds.Load(da);

                return ds.Rows.Count > 0;
            }
        }
    }

    // public static DataSet GetDataSet(string sqlCmd, SqliteConnection connection)
    // {
    //     
    // } 
}
