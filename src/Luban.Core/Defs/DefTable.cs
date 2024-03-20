using System.ComponentModel.Design;
using Luban.RawDefs;
using Luban.Types;
using Luban.TypeVisitors;
using Luban.Utils;
using Luban.Validator;

namespace Luban.Defs;

public enum IndexMode
{
    One,
    List,
    OneMainKey,
    ListMainKey
}

public record class IndexInfo
{
    public TType Type { get; private set; }
    public DefField IndexField{ get; private set; }
    public int IndexFieldIdIndex{ get; private set; }
    
    public List<IndexInfo> Childs { get; private set; }
    public bool IsUnionIndex => Childs.Count > 1;
    public bool IsListMode => Mode == IndexMode.List || Mode == IndexMode.ListMainKey;

    public bool IsMainKey => !IsUnionIndex && (Mode == IndexMode.ListMainKey || Mode == IndexMode.OneMainKey);
    
    public IndexMode Mode { get; private set; }

    public IndexInfo(TType type, DefField indexField, int index, IndexMode mode, List<IndexInfo> childs = null)
    {
        Type = type;
        IndexField = indexField;
        IndexFieldIdIndex = index;
        Mode = mode;
        this.Childs = new List<IndexInfo>();
        if (childs != null)
        {
            Childs.AddRange(childs);
        }
    }
}

public class DefTable : DefTypeBase
{
    private static readonly NLog.Logger s_logger = NLog.LogManager.GetCurrentClassLogger();

    public DefTable(RawTable b)
    {
        Name = b.Name;
        Namespace = b.Namespace;
        Index = b.Index;
        IndexMode = b.IndexMode;
        ValueType = b.ValueType;
        Mode = b.Mode;
        InputFiles = b.InputFiles;
        Groups = b.Groups;
        Comment = b.Comment;
        ReadSchemaFromFile = b.ReadSchemaFromFile;
        Tags = b.Tags;
        _outputFile = b.OutputFile;
    }

    public string Index { get; private set; }
    
    public string IndexMode { get; private set; }

    public string ValueType { get; }

    public TableMode Mode { get; }

    public bool ReadSchemaFromFile { get; }

    public bool IsSingletonTable => Mode == TableMode.ONE;

    public bool IsMapTable => Mode == TableMode.MAP;

    public bool IsListTable => Mode == TableMode.LIST;

    public bool IsExported { get; set; }

    public List<string> InputFiles { get; }

    private readonly string _outputFile;

    public TType KeyTType { get; private set; }

    public DefField IndexField { get; private set; }

    public int IndexFieldIdIndex { get; private set; }

    public TBean ValueTType { get; private set; }

    public TType Type { get; private set; }

    public bool IsUnionIndex { get; private set; }

    public bool MultiKey { get; private set; }

    public List<IndexInfo> IndexList { get; } = new();

    public List<ITableValidator> Validators { get; } = new();

    public string OutputDataFile => string.IsNullOrWhiteSpace(_outputFile) ? FullName.Replace('.', '_').ToLower() : _outputFile;
    
    public override void Compile()
    {
        var ass = Assembly;

        if ((ValueTType = (TBean)ass.CreateType(Namespace, ValueType, false)) == null)
        {
            throw new Exception($"table:'{FullName}' 的 value类型:'{ValueType}' 不存在");
        }

        ValueTType.DefBean.ConnectedTable = this;

        switch (Mode)
        {
            case TableMode.ONE:
            {
                IsUnionIndex = false;
                KeyTType = null;
                Type = ValueTType;
                break;
            }
            case TableMode.MAP:
            {
                IsUnionIndex = true;
                if (!string.IsNullOrWhiteSpace(Index))
                {
                    if (ValueTType.DefBean.TryGetField(Index, out var f, out var i))
                    {
                        IndexField = f;
                        IndexFieldIdIndex = i;
                    }
                    else
                    {
                        throw new Exception($"table:'{FullName}' index:'{Index}' 字段不存在");
                    }
                }
                else if (ValueTType.DefBean.HierarchyFields.Count == 0)
                {
                    throw new Exception($"table:'{FullName}' 必须定义至少一个字段");
                }
                else
                {
                    IndexField = ValueTType.DefBean.HierarchyFields[0];
                    Index = IndexField.Name;
                    IndexFieldIdIndex = 0;
                }
                KeyTType = IndexField.CType;
                Type = TMap.Create(false, null, KeyTType, ValueTType, false);
                this.IndexList.Add(new IndexInfo(KeyTType, IndexField, IndexFieldIdIndex, Defs.IndexMode.One));
                break;
            }
            case TableMode.LIST:
            {
                var indexs = Index.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s=> s.Trim()).ToList();//Index.Split('+', ',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList();
                var indexModes = IndexMode.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()).ToList();
                List<IndexInfo> childs = new List<IndexInfo>();
                Dictionary<string, string> mainTypes = new Dictionary<string, string>();
                for (int j = 0; j< indexs.Count; j++)
                {
                    var idx = indexs[j];
                    var fields = idx.Split('+').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s=>s.Trim()).ToList();
                    var mode = DefUtil.ConvertIndexMode(indexModes.Count > j ? indexModes[j] : "");
                    if (fields.Count > 1)
                    {
                        childs.Clear();
                        foreach (var field in fields)
                        {
                            if (ValueTType.DefBean.TryGetField(field, out var f, out var i))
                            {
                                if (f.CType.IsBean)
                                {
                                    throw new Exception($"table:'{FullName}' index:'{idx}' 联合主键内不支持嵌套联合主键");
                                }
                                childs.Add(new IndexInfo(f.CType, f, i, Defs.IndexMode.List));
                                
                                //this.IndexList.Add(new IndexInfo(f.CType, f, i, IndexType.One, mode));
                            }
                            else
                            {
                                throw new Exception($"table:'{FullName}' index:'{idx}' 字段不存在");
                            }
                        }

                        if (mode == Defs.IndexMode.ListMainKey || mode == Defs.IndexMode.OneMainKey)
                        {
                            throw new Exception($"table:'{FullName}' index: '{idx}' 联合索引不能作为MainKey");
                        }
                        this.IndexList.Add(new IndexInfo(childs[0].Type, childs[0].IndexField, childs[0].IndexFieldIdIndex, mode, childs));
                    }
                    else
                    {
                        
                        if (ValueTType.DefBean.TryGetField(idx, out var f, out var i))
                        {
                            if (IndexField == null)
                            {
                                IndexField = f;
                                IndexFieldIdIndex = i;
                            }

                            var info = new IndexInfo(f.CType, f, i, mode);
                            this.IndexList.Add(info);
                            if (info.IsMainKey)
                            {
                                if (f.CType.IsBean)
                                    throw new Exception($"table:'{FullName}' index: '{idx}'联合主键不能做为MainKey");
                                if (!mainTypes.TryAdd(f.CType.Apply(TypeNameVisitor.Ins), idx))
                                    throw new Exception($"table:'{FullName}' index: '{idx}' MainKey类型与‘{mainTypes[f.CType.Apply(TypeNameVisitor.Ins)]}’重复");
                            }
                        }
                        else
                        {
                            throw new Exception($"table:'{FullName}' index:'{idx}' 字段不存在");
                        }
                    }
                    
                }
                // 如果不是 union index, 每个key必须唯一，否则 (key1,..,key n)唯一
                //IsUnionIndex = IndexList.Count > 1 && !Index.Contains(',');
                IsUnionIndex = false;
                MultiKey = IndexList.Count > 1 && Index.Contains(',');
                break;
            }
            default:
                throw new Exception($"unknown mode:'{Mode}'");
        }

        foreach (var index in IndexList)
        {
            TType indexType = index.Type;
            string idxName = index.IndexField.Name;
            if (indexType.IsNullable)
            {
                throw new Exception($"table:'{FullName}' index:'{idxName}' 不能为 nullable类型");
            }
            if (!indexType.Apply(IsValidTableKeyTypeVisitor.Ins))
            {
                throw new Exception($"table:'{FullName}' index:'{idxName}' 的类型:'{index.IndexField.Type}' 不能作为index");
            }
        }
    }
}
