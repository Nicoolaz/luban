// YK Add File
namespace Luban.DataLoader.Builtin.Excel;
using System.Text;
using Luban.DataLoader.Builtin.Utils;

class DictReader 
{
    private List<Cell> _dataList;
    private Dictionary<string, Cell> _datas;
    private int _toIndex;
    private int _curIndex;

    public string LastReadDataInfo { get; }
    private int LastReadIndex { get; set; }

    public DictReader(List<Cell> datas, int fromIndex, int toIndex, string sep, string overrideDefault, string lastReadDataInfo)
    {
        LastReadDataInfo = lastReadDataInfo;
        if (string.IsNullOrWhiteSpace(sep))
        {

            this._dataList = datas;
            this._toIndex = toIndex;
            this._curIndex = fromIndex;
        }
        else
        {
            this._datas = new Dictionary<string, Cell>();
            List<Cell> pairs = new List<Cell>();
            Char[] seps = sep.ToCharArray();
            for (int i = fromIndex; i <= toIndex; i++)
            {
                var cell = datas[i];
                object d = cell.Value;
                if (!IsSkip(d))
                {
                    string s = (string)d;
                    pairs.AddRange(LoadDataUtil.SplitStringByAnySepChar(s, seps[0].ToString())
                        .Where(x => !IsSkip(x))
                        .Select(x => new Cell(cell.Row, cell.Column, x))
                        );
                    _dataList.AddRange(pairs);
                    foreach (var pair in pairs)
                    {
                        string sep2 = seps.Length >= 2 ? seps[1].ToString() : ":";
                        string[] key_value = ((string)pair.Value).Split(sep2, 2);
                        if (key_value.Length != 2)
                        {
                            continue;
                        }

                        _datas[key_value[0]] = new Cell(cell.Row, cell.Column, key_value[1]);
                    }
                    
                }
            }
        }

    }

    public DictReader(Cell cell, string sep, string lastReadDataInfo)
    {
        LastReadDataInfo = lastReadDataInfo;
        if (string.IsNullOrWhiteSpace(sep))
        {
            throw new FormatException($"DictReader read failed string = {cell.Value.ToString()}");
        }
        else
        {
            this._datas = new Dictionary<string, Cell>();
            object d = cell.Value;
            Char[] seps = sep.ToCharArray();
            List<Cell> pairs = new List<Cell>();
            if (!IsSkip(d))
            {
                string s = (string)d;
                pairs.AddRange(LoadDataUtil.SplitStringByAnySepChar(s, seps[0].ToString())
                    .Where(x => !IsSkip(x))
                    .Select(x => new Cell(cell.Row, cell.Column, x))
                );
                foreach (var pair in pairs)
                {
                    string sep2 = seps.Length >= 2 ? seps[1].ToString() : ":";
                    string[] key_value = ((string)pair.Value).Split(sep2, 2);
                    if (key_value.Length != 2)
                    {
                        throw new FormatException($"DictReader read failed string = {s}");
                    }

                    _datas[key_value[0]] = new Cell(cell.Row, cell.Column, key_value[1]);
                }
                    
            }
        }
    }
    
    
    public bool TryReadEOF()
    {
        return true;
    }
    
    public Cell this[string key]
    {
        get
        {
            return _datas.ContainsKey(key) ? _datas[key] : new Cell(0, 0, string.Empty);
        }
    }
    
    public object Read(bool notSkip = false)
    {
        //if (curIndex <= toIndex)
        //{
        //    return datas[curIndex++].Value;
        //}
        //else
        //{
        //    throw new Exception($"cell:{datas[curIndex - 1]} 无法读取到足够多的数据");
        //}
        return notSkip ? ReadMayNull() : ReadSkipNull();
    }
    
    public object ReadSkipNull()
    {
        while (_curIndex <= _toIndex)
        {
            var data = _dataList[_curIndex++];
            if (!IsSkip(data.Value))
            {
                LastReadIndex = _curIndex - 1;
                return data.Value;
            }
        }
        LastReadIndex = _curIndex - 1;
        throw new Exception($"cell:{_dataList[_curIndex - 1]} 缺少数据");
    }

    private object ReadMayNull()
    {
        return _curIndex <= _toIndex ? _dataList[LastReadIndex = _curIndex++].Value : null;
    }

    
    private bool IsSkip(object x)
    {
        return x == null || !(x is string) || (x is string s && string.IsNullOrEmpty(s));
    }

}
