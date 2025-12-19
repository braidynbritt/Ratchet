namespace Ratchet.AST
{
    public sealed class TypeNode : ASTNode
    {
        public string Name { get; }

        public TypeNode(
            string name,
            int line,
            int column
        ) : base( line, column)
        {
            Name = name;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitType(this);
    }
}
