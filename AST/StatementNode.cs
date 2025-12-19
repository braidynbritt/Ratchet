using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public abstract class StatementNode : ASTNode
    {
        protected StatementNode(int line, int column) 
            : base(line, column) { }
    }
}
