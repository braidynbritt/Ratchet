namespace Ratchet.AST
{
    public sealed class ExpressionStatementNode : StatementNode
    {
        public ExpressionNode Expression { get; }

        public ExpressionStatementNode(
            ExpressionNode expression,
            int line,
            int column
        ) : base(line, column)
        {
            Expression = expression;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitExpressionStatement(this);
    }
}
