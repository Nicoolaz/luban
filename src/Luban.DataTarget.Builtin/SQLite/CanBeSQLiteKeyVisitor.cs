using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.DataExporter.Builtin.SQLite;

public class CanBeSQLiteKeyVisitor : AllTrueVisitor
{
    public static new CanBeSQLiteKeyVisitor Ins { get; } = new CanBeSQLiteKeyVisitor();
    public override bool Accept(TBean type)
    {
        return false;
    }

    public override bool Accept(TArray type)
    {
        return false;
    }

    public override bool Accept(TMap type)
    {
        return false;
    }

    public override bool Accept(TDateTime type)
    {
        return false;
    }

    public override bool Accept(TSet type)
    {
        return false;
    }

    public override bool Accept(TList type)
    {
        return false;
    }
}
