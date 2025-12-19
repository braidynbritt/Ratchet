namespace Ratchet.AST
{
    public sealed class BlockNode : StatementNode
    {
        public IReadOnlyList<StatementNode> StatementNodes { get; }

        public BlockNode(
            List<StatementNode> statementNodes,
            int line,
            int column
        ) : base(line, column)
        {
            StatementNodes = statementNodes; 
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitBlock(this);
    }
}
