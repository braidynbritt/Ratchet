namespace Ratchet.AST
{
    public sealed class ProgramNode : ASTNode
    {
        public IReadOnlyList<FunctionDeclNode> Functions { get; }

        public ProgramNode(
            List<FunctionDeclNode> functions,
            int line,
            int column
        ) : base(line, column)
        {
            Functions = functions;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitProgram( this );
            
    }
}
