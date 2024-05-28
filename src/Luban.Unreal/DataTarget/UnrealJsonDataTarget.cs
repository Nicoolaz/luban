using Luban.DataTarget;
using Luban.Defs;
using Luban.DataExporter.Builtin.Json;
using System.Text.Json;
using Luban.RawDefs;
using Luban.Unreal.TemplateExtensions;
using Luban.Utils;

namespace Luban.Unreal.DataTarget;

[DataTarget("unreal-json")]
public class UnrealJsonDataTarget : DataTargetBase
{
    protected override string DefaultOutputFileExt => "json";
    public override AggregationType AggregationType => AggregationType.Other;
    public static bool UseCompactJson => EnvManager.Current.GetBoolOptionOrDefault("json", "compact", true, false);

    protected virtual UnrealJsonDataVisitor ImplJsonDataVisitor => UnrealJsonDataVisitor.Ins;

    public void WriteAsArray(List<Record> datas, Utf8JsonWriter x, UnrealJsonDataVisitor jsonDataVisitor, DefTable table)
    {
        x.WriteStartArray();
        foreach (var d in datas)
        {
            d.Data.Apply(jsonDataVisitor, x);
        }
        
        x.WriteEndArray();
    }

    public override void ExportCustom(List<DefTable> tables, OutputFileManifest manifest, GenerationContext ctx)
    {
        var ss = new MemoryStream();
        var writer = new Utf8JsonWriter(ss,
            new JsonWriterOptions() { Indented = !UseCompactJson, SkipValidation = false, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping, });
        writer.WriteStartObject();
        writer.WritePropertyName(ConstStrings.UnrealImportGroupName);
        writer.WriteStartArray();
        var tasks = tables.Select(table => Task.Run(() =>
        {
            lock (writer)
            {
                WriteUnrealImportSetting(table, writer);
            }
            manifest.AddFile(ExportTable(table, ctx.GetTableExportDataList(table)));
        })).ToArray();
        Task.WaitAll(tasks);
        writer.WriteEndArray();
        writer.WriteEndObject();
        writer.Flush();
        manifest.AddFile(new OutputFile()
        {
            File = $"UnrealImportSettings.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss)
        });
    }

    private void WriteUnrealImportSetting(DefTable table, Utf8JsonWriter writer)
    {
        var outputDir = EnvManager.Current.GetOptionOrDefault("unreal-json", BuiltinOptionNames.OutputDataDir, true, "");
        outputDir = Path.GetFullPath(outputDir);
        writer.WriteStartObject();
        writer.WritePropertyName("GroupName");
        writer.WriteStringValue("LubanConfig");
        writer.WritePropertyName("DestinationPath");
        writer.WriteStringValue(EnvManager.Current.GetOptionOrDefault("", ConstStrings.UnrealImportDestinationParamName, true, "Game/Table/LubanConfig"));
        writer.WritePropertyName("FactoryName");
        writer.WriteStringValue("ReimportDataTableFactory");
        writer.WritePropertyName("Filenames");
        writer.WriteStartArray();
        writer.WriteStringValue(Path.Join(outputDir, $"{table.OutputDataFile}.{OutputFileExt}"));
        writer.WriteEndArray();
        writer.WritePropertyName("ImportSettings");
        writer.WriteStartObject();
        writer.WritePropertyName("ImportType");
        writer.WriteStringValue("ECSV_DataTable");
        writer.WritePropertyName("ImportRowStruct");
        writer.WriteStringValue(UnrealTemplateExtension.MakeCppNameWithoutPerfix(table.ValueTType.DefBean));
        writer.WriteEndObject();
        writer.WritePropertyName("bSkipReadOnly");
        writer.WriteStringValue("true");
        writer.WritePropertyName("bReplaceExisting");
        writer.WriteStringValue("true");
        writer.WriteEndObject();
    }
    public override OutputFile ExportTable(DefTable table, List<Record> records)
    {
        var ss = new MemoryStream();
        var jsonWriter = new Utf8JsonWriter(ss, new JsonWriterOptions()
        {
            Indented = !UseCompactJson,
            SkipValidation = false,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        });
        WriteAsArray(records, jsonWriter, ImplJsonDataVisitor, table);
        jsonWriter.Flush();
        return new OutputFile()
        {
            File = $"{table.OutputDataFile}.{OutputFileExt}",
            Content = DataUtil.StreamToBytes(ss),
        };
    }
}
