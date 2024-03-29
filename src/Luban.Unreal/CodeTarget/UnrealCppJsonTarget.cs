using Luban.CodeTarget;
using Luban.Defs;
using Scriban;
using Scriban.Runtime;
using Luban.Utils;
using Luban.Unreal;
using Luban.Unreal.TemplateExtensions;

namespace Luban.Unreal.CodeTarget;

[CodeTarget("unreal-cpp-json")]
public class UnrealCppJsonTarget : TemplateCodeTargetBase
{
    protected override IReadOnlySet<string> PreservedKeyWords => new HashSet<string>
    {
        // cpp preserved key words
        "alignas", "alignof", "and", "and_eq", "asm", "atomic_cancel", "atomic_commit", "atomic_noexcept",
        "auto", "bitand", "bitor", "bool", "break", "case", "catch", "char", "char8_t", "char16_t", "char32_t",
        "class", "compl", "concept", "const", "consteval", "constexpr", "constinit", "const_cast", "continue",
        "co_await", "co_return", "co_yield", "decltype", "default", "delete", "do", "double", "dynamic_cast",
        "else", "enum", "explicit", "export", "extern", "false", "float", "for", "friend", "goto", "if", "import", 
        "inline", "int", "long", "module", "mutable", "namespace", "new", "noexcept", "not", "not_eq", "nullptr",
        "operator", "or", "or_eq", "private", "protected", "public", "reflexpr", "register", "reinterpret_cast",
        "requires", "return", "short", "signed", "sizeof", "static", "static_assert", "static_cast", "struct",
        "switch", "synchronized", "template", "this", "thread_local", "throw", "true", "try", "typedef", "typeid",
        "typename", "union", "unsigned", "using", "virtual", "void", "volatile", "wchar_t", "while", "xor", "xor_eq",
        "Name"/* Name is a default variable for unreal data table */, "Default",
    };
    public override string FileHeader => CommonFileHeaders.AUTO_GENERATE_C_LIKE;
    protected override string FileSuffixName => ".cpp";

    public override void Handle(GenerationContext ctx, OutputFileManifest manifest)
    {
        //base.Handle(ctx, manifest);
        var tasks = new List<Task<OutputFile>>();
        foreach (var bean in ctx.ExportBeans)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBeanCpp(ctx, bean, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(UnrealTemplateExtension.MakeCppName(bean))}{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
            
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateBeanHeader(ctx, bean, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(UnrealTemplateExtension.MakeCppName(bean))}.h", Content = writer.ToResult(FileHeader) };
            }));
        }

        foreach (var @enum in ctx.ExportEnums)
        {
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnumCpp(ctx, @enum, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(UnrealTemplateExtension.MakeCppName(@enum))}{FileSuffixName}", Content = writer.ToResult(FileHeader) };
            }));
            
            tasks.Add(Task.Run(() =>
            {
                var writer = new CodeWriter();
                GenerateEnumHeader(ctx, @enum, writer);
                return new OutputFile() { File = $"{GetFileNameWithoutExtByTypeName(UnrealTemplateExtension.MakeCppName(@enum))}.h", Content = writer.ToResult(FileHeader) };
            }));
        }
        

        Task.WaitAll(tasks.ToArray());
        foreach (var task in tasks)
        {
            manifest.AddFile(task.Result);
        }
    }

    protected void GenerateBeanHeader(GenerationContext ctx, DefBean bean, CodeWriter writer)
    {
        var template = GetTemplate("bean_h");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", UnrealTemplateExtension.MakeCppName(bean)},
            { "__bean", bean },
            { "__this", bean },
            { "__export_fields", bean.ExportFields},
            { "__hierarchy_export_fields", bean.HierarchyExportFields},
            { "__parent_def_type", bean.ParentDefType},
            { "__code_style", CodeStyle},
            { "__api_name", EnvManager.Current.GetOptionOrDefault("", ConstStrings.APINameOption, true, ConstStrings.APIName)}
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    protected void GenerateBeanCpp(GenerationContext ctx, DefBean bean, CodeWriter writer)
    {
        var template = GetTemplate("bean_cpp");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", UnrealTemplateExtension.MakeCppName(bean)},
            { "__includeDir", EnvManager.Current.GetOptionOrDefault("", 
                ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix)},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    protected void GenerateEnumHeader(GenerationContext ctx, DefEnum @enum, CodeWriter writer)
    {
        var template = GetTemplate("enum_h");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", UnrealTemplateExtension.MakeCppName(@enum) },
            { "__enum", @enum },
            { "__this", @enum },
            { "__code_style", CodeStyle},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    protected void GenerateEnumCpp(GenerationContext ctx, DefEnum @enum, CodeWriter writer)
    {
        var template = GetTemplate("enum_cpp");
        var tplCtx = CreateTemplateContext(template);
        var extraEnvs = new ScriptObject
        {
            { "__ctx", ctx},
            { "__name", UnrealTemplateExtension.MakeCppName(@enum) },
            { "__includeDir", EnvManager.Current.GetOptionOrDefault("", 
                ConstStrings.PackageDirCfgName, true, ConstStrings.IncludePerfix)},
        };
        tplCtx.PushGlobal(extraEnvs);
        writer.Write(template.Render(tplCtx));
    }

    protected override void OnCreateTemplateContext(TemplateContext ctx)
    {
        ctx.PushGlobal(new UnrealTemplateExtension());
        //throw new NotImplementedException();
    }
}
