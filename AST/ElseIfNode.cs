using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public sealed class ElseIfNode : ASTNode
    {
        public ExpressionNode Condition { get; }
        public BlockNode TrueBlock { get; }

        public ElseIfNode(
            
            ExpressionNode condition, 
            BlockNode trueBlock, 
            int line, 
            int column
        ) : base(line, column)
        {
            Condition = condition;
            TrueBlock = trueBlock;
        }
        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitElseIf(this);
    }
}
