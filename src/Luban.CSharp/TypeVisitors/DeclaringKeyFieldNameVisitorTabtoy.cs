using Luban.Types;
using Luban.TypeVisitors;

namespace Luban.CSharp.TypeVisitors;

public class DeclaringKeyFieldNameVisitorTabtoy : DecoratorFuncVisitor<string, string, string>
{
    public static DeclaringKeyFieldNameVisitorTabtoy Ins { get; } = new();

    protected virtual ITypeFuncVisitor<string, string, string> UnderlyingVisitor => UnderlyingDeclaringKeyFieldNameVisitorTabtoy.Ins;

    public override string DoAccept(TType type, string fieldRoot, string filedName)
    {
        return type.IsNullable && !type.Apply(IsRawNullableTypeVisitor.Ins) ? (type.Apply(UnderlyingVisitor, fieldRoot, filedName) + "?") : type.Apply(UnderlyingVisitor, fieldRoot, filedName);
    }
    
}
