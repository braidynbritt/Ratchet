namespace Ratchet.AST
{
    public enum BinaryOperatorType
    {
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Equals,
        NotEquals,
        LessThan,
        LessThanOrEqual,
        GreaterThan,
        GreaterThanOrEqual,
        And,
        Or
    }

    public sealed class BinaryOpNode : ExpressionNode
    {
        public BinaryOperatorType Operator { get; }
        public ExpressionNode Left { get; }
        public ExpressionNode Right { get; }

        public BinaryOpNode(
            BinaryOperatorType op,
            ExpressionNode left,
            ExpressionNode right,
            int line,
            int column
        ) : base(line, column)
        {
            Operator = op; 
            Left = left; 
            Right = right;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitBinaryOp( this );
    }
}
