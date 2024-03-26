using Luban.Datas;
using Luban.DataTarget;
using Luban.Defs;
using Luban.Serialization;
using Luban.Types;
using Luban.Utils;
using Microsoft.Data.Sqlite;

namespace Luban.DataExporter.Builtin.SQLite;

[DataTarget("sqlite3")]
public class SQLiteDataTarget : DataTargetBase
{
    protected override string OutputFileExt => "db";

    public override AggregationType AggregationType => AggregationType.Other;
    
    
    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        throw new NotSupportedException();
    }

    public void ExportTable(DefTable table, List<Record> records, SqliteConnection connection)
    {
        if (table.ValueTType.DefBean.IsAbstractType)
        {
            throw new Exception("Sqlite Exporter do not support virtual table bean!");
        }
        if (SQLiteUtil.IsTableExists(connection, table.Name))
        {
            SQLiteUtil.DeleteTable(connection, table.Name);
        }
        SQLiteUtil.CreateTable(connection, table);

        //lock (connection)
        {
            using (SqliteCommand insertCommand = SQLiteUtil.GetInsertDataCommand(connection, table))
            {
                //try
                {
                    List<SqliteParameter> parameters = new List<SqliteParameter>();
                    var defFields = table.ValueTType.DefBean.HierarchyFields;
                    for (int i = 0; i < defFields.Count; i++)
                    {
                        if(!defFields[i].NeedExport()) continue;
                        if (defFields[i].CType.IsBean && SQLiteUtil.IsKeyIndex(defFields[i].Name, table))
                        {
                            foreach (var field in ((TBean)defFields[i].CType).DefBean.Fields)
                            {
                                parameters.Add(new SqliteParameter($"@{field.Name}", field.CType.Apply(SqliteTTypeVisitor.Ins)));
                            }
                        }
                        else
                        {
                            parameters.Add(new SqliteParameter($"@{defFields[i].Name}", defFields[i].CType.Apply(SqliteTTypeVisitor.Ins)));
                        }
                    }
                    
                    insertCommand.Parameters.AddRange(parameters.ToArray());
                
                    for (int i = 0; i < records.Count; i++)
                    {
                        var record = records[i];
                        int index = 0;
                        for (int j = 0; j < record.Data.Fields.Count; j++)
                        {
                            if (!defFields[j].NeedExport()) continue;
                            if (defFields[j].IsNullable && record.Data.Fields[j] == null)
                            {
                                insertCommand.Parameters[index++].Value = 
                                switch (defFields[j].CType.Apply(SqliteTTypeVisitor.Ins))
                                {
                                    case SqliteType.Blob:
                                    {
                                        ByteBuf _buf = new ByteBuf();
                                        _buf.WriteBool(false);
                                        insertCommand.Parameters[index++].Value = _buf.Bytes;
                                        break;
                                    }
                                    case SqliteType.Integer:
                                    {
                                        insertCommand.Parameters[index++].Value = 0;
                                        break;
                                    }
                                    case SqliteType.Real:
                                    {
                                        insertCommand.Parameters[index++].Value = 0;
                                        break;
                                    }
                                    case SqliteType.Text:
                                    {
                                        insertCommand.Parameters[index++].Value = "";
                                        break;
                                    }
                                        
                                }
                            }
                            else
                            {
                                if (defFields[j].CType.IsBean && SQLiteUtil.IsKeyIndex(defFields[j].Name, table))
                                {
                                    foreach (var field in ((DBean)record.Data.Fields[j]).Fields)
                                    {
                                        field.Apply(SQLiteDataVisiter.Ins, insertCommand.Parameters[index++]);
                                    }
                                }
                                else
                                {
                                    record.Data.Fields[j].Apply(SQLiteDataVisiter.Ins, insertCommand.Parameters[index++]);
                                }
                            }
                        }

                        if (record.Data.Type.IsAbstractType)
                        {
                            insertCommand.Parameters[index].Value = record.Data.ImplType.Id;
                        }

                        insertCommand.ExecuteNonQuery();
                    }
                
                    
                }
                
            }
        }
    }
}
