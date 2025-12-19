namespace Ratchet.AST
{
    public sealed class FunctionCallNode : ExpressionNode
    {
        public string FunctionName { get; }
        public IReadOnlyList<ExpressionNode> ArgumentList { get; }

        public FunctionCallNode(
            string functionName,
            List<ExpressionNode> argumentList,
            int line,
            int column
        ) : base( line, column)
        {
            FunctionName = functionName;
            ArgumentList = argumentList;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitFunctionCall( this );
    }
}
