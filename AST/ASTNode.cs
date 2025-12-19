namespace Ratchet.AST
{
    public abstract class ASTNode
    {
        public int Line { get; }

        public int Column { get; }

        protected ASTNode(int line, int column) 
        {
            Line = line;
            Column = column;
        }

        public abstract T Accept<T>(IAstVisitor<T> visitor);
    }
}
