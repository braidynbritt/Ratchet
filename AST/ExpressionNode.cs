using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public abstract class ExpressionNode : ASTNode
    {
        protected ExpressionNode(int line, int column)
            : base(line, column) { }
    }
}
