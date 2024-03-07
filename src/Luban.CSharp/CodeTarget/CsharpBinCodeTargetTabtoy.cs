using Luban.CSharp.TemplateExtensions;
using Scriban;
using Luban.CodeTarget;
using Luban.Defs;
using Luban.Utils;
using Scriban.Runtime;

namespace Luban.CSharp.CodeTarget;

[CodeTarget("cs-bin-tabtoy")]
public class CsharpBinCodeTargetTabtoy : CsharpCodeTargetBase
{
    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        var tasks = new List<Task<OutputFile>>();
        tasks.Add(Task.Run(() =>
        {
            var writer = new CodeWriter();
            GenerateTables(ctx, ctx.Tables.Where(t=> ctx.Assembly.NeedExport(t.Groups)).ToList(), writer);
            return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(ctx.Target.Manager)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
        }));

        foreach (var table in ctx.ExportTables)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateTable(ctx, table, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(table.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBean(ctx, bean, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(bean.FullTypeName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnum(ctx, @enum, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(@enum.FullName)}.{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
        }

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }
    
    public override void GenerateBean(GenerationContext ctx, DefBean bean, CodeWriter writer)
    {
        var template = GetTemplate("bean");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__top_module", ctx.Target.TopModule },
            { "__manager_name", ctx.Target.Manager },
            { "__manager_name_with_top_module", TypeUtil.MakeFullName(ctx.TopModule, ctx.Target.Manager) },
            { "__name", bean.Name },
            {"__type_name", bean.TypeName},
            { "__namespace", bean.Namespace },
            { "__namespace_with_top_module", bean.NamespaceWithTopModule },
            { "__full_name_with_top_module", bean.FullNameWithTopModule },
            { "__bean", bean },
            { "__this", bean },
            {"__export_fields", bean.ExportFields},
            {"__hierarchy_export_fields", bean.HierarchyExportFields},
            {"__parent_def_type", bean.ParentDefType},
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        base.OnCreateTemplateContext(ctx);
        ctx.PushGlobal(new CsharpBinTemplateExtension());
    }
}
