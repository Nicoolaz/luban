using Luban.DataVisitors;
using Luban.Types;

namespace Luban.Datas;

public class DUint : DType<uint>
{
    private const int POOL_SIZE = 128;
    private static readonly DUint[] s_pool = new DUint[POOL_SIZE];

    static DUint()
    {
        for (uint i = 0; i < POOL_SIZE; i++)
        {
            s_pool[i] = new DUint(i);
        }
    }

    public static DUint Default => s_pool[0];

    public static DUint ValueOf(uint x)
    {
        if (x >= 0 && x < POOL_SIZE)
        {
            return s_pool[x];
        }
        return new DUint(x);
    }

    public override string TypeName => "uint";

    private DUint(uint x) : base(x)
    {
    }

    public override void Apply<T>(IDataActionVisitor<T> visitor, T x)
    {
        visitor.Accept(this, x);
    }

    public override void Apply<T1, T2>(IDataActionVisitor<T1, T2> visitor, T1 x, T2 y)
    {
        visitor.Accept(this, x, y);
    }

    public override void Apply<T>(IDataActionVisitor2<T> visitor, TType type, T x)
    {
        visitor.Accept(this, type, x);
    }

    public override void Apply<T1, T2>(IDataActionVisitor2<T1, T2> visitor, TType type, T1 x, T2 y)
    {
        visitor.Accept(this, type, x, y);
    }

    public override TR Apply<TR>(IDataFuncVisitor<TR> visitor)
    {
        return visitor.Accept(this);
    }

    public override TR Apply<T, TR>(IDataFuncVisitor<T, TR> visitor, T x)
    {
        return visitor.Accept(this, x);
    }

    public override TR Apply<T1, T2, TR>(IDataFuncVisitor<T1, T2, TR> visitor, T1 x, T2 y)
    {
        return visitor.Accept(this, x, y);
    }

    public override bool Equals(object obj)
    {
        switch (obj)
        {
            case DInt dint:
                return this.Value == dint.Value;
            case DEnum denum:
                return this.Value == denum.Value;
            default:
                return false;
        }
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override int CompareTo(DType other)
    {
        if (other is DInt d)
        {
            return this.Value.CompareTo(d.Value);
        }
        throw new System.NotSupportedException();
    }
}
