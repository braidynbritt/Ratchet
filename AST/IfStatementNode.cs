namespace Ratchet.AST
{
    public sealed class IfStatementNode : StatementNode
    {
        public ExpressionNode Condition { get; }
        public BlockNode TrueBlock { get; }
        public BlockNode? FalseBlock { get; }
        public IReadOnlyList<ElseIfNode>? ElseIfNodes { get; }

        public IfStatementNode(
            ExpressionNode condition, 
            BlockNode trueBlock, 
            BlockNode? falseBlock, 
            List<ElseIfNode>? elseIfNodes, 
            int line, int column
        ) : base(line, column)
        {
            Condition = condition;
            TrueBlock = trueBlock;
            FalseBlock = falseBlock;
            ElseIfNodes = elseIfNodes;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitIf(this);
    }
}
