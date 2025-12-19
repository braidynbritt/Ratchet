namespace Ratchet.AST
{
    public sealed class WhileNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public BlockNode Body { get; }

        public WhileNode(
            ExpressionNode condition,
            BlockNode body,
            int line,
            int column
        ) : base ( line, column )
        {
            Condition = condition;
            Body = body;
        }
        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitWhile( this );
    }
}
