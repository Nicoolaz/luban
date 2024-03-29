using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.Unreal.TypeVisitors;

public class TypeNeedsExtraIncludeVisitor : AllFalseVisitor
{
    public static TypeNeedsExtraIncludeVisitor Ins { get; } = new TypeNeedsExtraIncludeVisitor();
    public override bool Accept(TBean type)
    {
        return true;
    }

    public override bool Accept(TEnum type)
    {
        return true;
    }

    public override bool Accept(TArray type)
    {
        return type.ElementType.Apply(this);
    }

    public override bool Accept(TList type)
    {
        return type.ElementType.Apply(this);
    }

    public override bool Accept(TMap type)
    {
        return type.KeyType.Apply(this) || type.ValueType.Apply(this);
    }

    public override bool Accept(TSet type)
    {
        return type.ElementType.Apply(this);
    }
}
