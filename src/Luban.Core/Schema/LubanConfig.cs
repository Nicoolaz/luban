using Luban.RawDefs;

namespace Luban.Schema;

public class LubanConfig
{
    public string ConfigFileName { get; set; }
    public List<RawGroup> Groups { get; set; }

    public List<RawTarget> Targets { get; set; }

    public List<SchemaFileInfo> Imports { get; set; }

    public bool GroupsIsChar;

    public List<string> ExcelDefaultSchema;

    //YK Add 增加全局Tag
    public Dictionary<string, Dictionary<string, string>> GlobalTagsMap;

    public string InputDataDir { get; set; }
}
