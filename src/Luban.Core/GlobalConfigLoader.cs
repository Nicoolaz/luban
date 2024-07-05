using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Luban.RawDefs;
using Luban.Schema;
using Luban.Utils;
using NLog.Targets;

namespace Luban;


internal class Group
{
    public List<string> Names { get; set; }

    public bool Default { get; set; }
}

public class SchemaFile
{
    public string FileName { get; set; }

    public string Type { get; set; }
}

internal class Target
{
    public string Name { get; set; }

    public string Manager { get; set; }

    public List<string> Groups { get; set; }

    public string TopModule { get; set; }
}

//YK Add 增加类型全局tag，用于配置默认tag
internal class TypeGlobalTags
{
    public string TypeName {  get; set; }

    public string Tags {  get; set; }
}

internal class LubanConf
{
    public List<Group> Groups { get; set; }

    public List<SchemaFile> SchemaFiles { get; set; }

    public string DataDir { get; set; }

    public List<Target> Targets { get; set; }

    public List<string> ExcelDefaultSchema { get; set; }

    // YK Add 增加全局Tag
    public List<TypeGlobalTags> GlobalTags { get; set; }

    public bool GroupsIsChar { get; set; }
}

[JsonSourceGenerationOptions(
    JsonSerializerDefaults.Web,
    AllowTrailingCommas = true,
    DefaultBufferSize = 10)]
[JsonSerializable(typeof(LubanConf))]
internal partial class LubanConfContext : JsonSerializerContext { }

public class GlobalConfigLoader : IConfigLoader
{
    
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    private string _curDir;

    public GlobalConfigLoader()
    {

    }

    public LubanConfig Load(string fileName)
    {
        s_logger.Debug("load config file:{}", fileName);
        _curDir = Directory.GetParent(fileName).FullName;

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
        };
        
        var globalConf = JsonSerializer.Deserialize(File.ReadAllText(fileName, Encoding.UTF8), typeof(LubanConf), LubanConfContext.Default) as LubanConf;

        var configFileName = Path.GetFileName(fileName);
        var dataInputDir = Path.Combine(_curDir, globalConf.DataDir);
        List<RawGroup> groups = globalConf.Groups.Select(g => new RawGroup() { Names = g.Names, IsDefault = g.Default }).ToList();
        List<RawTarget> targets = globalConf.Targets.Select(t => new RawTarget() { Name = t.Name, Manager = t.Manager, Groups = t.Groups, TopModule = t.TopModule }).ToList();

        List<SchemaFileInfo> importFiles = new();
        foreach (var schemaFile in globalConf.SchemaFiles)
        {
            if (string.IsNullOrEmpty(schemaFile.Type))
            {
                var fullPath = Path.Combine(_curDir, schemaFile.FileName);
                if (!Directory.Exists(fullPath))
                {
                    throw new Exception($"{configFileName} schemal 文件错误: 目录'{fullPath}'不存在");
                }
            }
            string fileOrDirectory = Path.Combine(_curDir, schemaFile.FileName);
            foreach (var subFile in FileUtil.GetFileOrDirectory(fileOrDirectory))
            {
                importFiles.Add(new SchemaFileInfo() { FileName = subFile, Type = schemaFile.Type });
            }
        }

        Dictionary<string, Dictionary<string, string>> GlobalTags = new Dictionary<string, Dictionary<string, string>>();
        if (globalConf.GlobalTags != null)
        {
            foreach(var TagInfo in globalConf.GlobalTags)
            {
                var tags = DefUtil.ParseAttrs(TagInfo.Tags);
                if(!GlobalTags.ContainsKey(TagInfo.TypeName))
                {
                    GlobalTags[TagInfo.TypeName.ToLower()] = tags;   
                }
                else
                {
                    s_logger.Warn($"GlobalTags配置中有重复的Type : {TagInfo.TypeName},会被忽略请检查Conf文件！！");
                }
            }
        }
        return new LubanConfig()
        {
            ConfigFileName = configFileName,
            InputDataDir = dataInputDir,
            Groups = groups,
            Targets = targets,
            Imports = importFiles,
            GroupsIsChar = globalConf.GroupsIsChar,
            ExcelDefaultSchema = globalConf.ExcelDefaultSchema ?? new List<string>(),
            GlobalTagsMap = GlobalTags,
        };
    }

}
