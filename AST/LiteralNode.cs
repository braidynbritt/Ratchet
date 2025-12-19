namespace Ratchet.AST
{
    public sealed class LiteralNode : ExpressionNode
    {
        public object Value { get; }

        public LiteralNode(
            object value,
            int line,
            int column
        ) : base(line, column)
        {
            Value = value;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitLiteral(this);
    }
}
