namespace Ratchet.AST
{
    public sealed class ReturnNode : StatementNode
    {
        public ExpressionNode? Expression { get; }

        public ReturnNode(
            ExpressionNode? expression,
            int line,
            int column
        ) : base( line, column )
        {
            Expression = expression;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitReturn( this );
    }
}
