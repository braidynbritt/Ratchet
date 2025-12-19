namespace Ratchet.AST
{
    public abstract class StatementNode : ASTNode
    {
        protected StatementNode(int line, int column) 
            : base(line, column) { }
    }
}
