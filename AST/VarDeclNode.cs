namespace Ratchet.AST
{
    public sealed class VarDeclNode : StatementNode
    {
        public string VariableName { get; }
        public TypeNode? Type { get; }

        public ExpressionNode? Expression { get; }

        public VarDeclNode(
            string variableName,
            TypeNode? type, 
            ExpressionNode? expression,
            int line,
            int column
        ) : base(line, column)
        {
            VariableName = variableName;
            Type = type;
            Expression = expression;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitVarDecl( this );
    }
}
