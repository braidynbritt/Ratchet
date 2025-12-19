namespace Ratchet.AST
{
    public enum UnaryOperatorType
    {
        Not,
        Negative
    }

    public sealed class UnaryOpNode: ExpressionNode
    {
        public UnaryOperatorType Operator { get; }
        public ExpressionNode Operand { get; }

        public UnaryOpNode(
            UnaryOperatorType op,
            ExpressionNode operand,
            int line,
            int column
        ) : base(line, column)
        {
            Operator = op;
            Operand = operand;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitUnaryOp(this);
    }
}
