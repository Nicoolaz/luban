using Microsoft.Data.Sqlite;
using Luban.DataTarget;
using Luban.Defs;
using NLog;

namespace Luban.DataExporter.Builtin.SQLite;

[DataExporter("sqlite3")]
public class SQLiteExporter : DataExporterBase
{
    protected override void ExportCustom(List<DefTable> tables, OutputFileManifest manifest, IDataTarget dataTarget, GenerationContext ctx)
    {
        var dir = EnvManager.Current.GetOption($"{manifest.TargetName}", BuiltinOptionNames.OutputDataDir, true);
        var dbFilePath = Path.Join(dir, $"{EnvManager.Current.GetOption("", BuiltinOptionNames.DBFileName, true)}.db");
        //SqliteConnection connection = SQLiteUtil.CreateAndConnectToDB(dbFilePath);
        //try
        {
            List<Task> tasks = new List<Task>();
            foreach (var table in tables)
            {
                // tasks.Add(Task.Run(() =>
                // {
                    using (var conn = SQLiteUtil.CreateAndConnectToDB(dbFilePath))
                    {
                        try
                        {
                            using (var tran = conn.BeginTransaction())
                            {
                                ((SQLiteDataTarget)dataTarget).ExportTable(table, ctx.GetTableExportDataList(table), conn);
                                tran.Commit();
                            }
                        }
                        catch (Exception e)
                        {
                            var __e = e;
                        }
                        finally
                        {
                            conn.Close();
                        }
                        
                        
                    }
                // }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
