using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public sealed class IdentifierNode : ExpressionNode
    {
        public string VariableName { get; }

        public IdentifierNode(
            string variableName, 
            int line, 
            int column 
        ) : base(line, column) 
        { 
            VariableName = variableName; 
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitIdentifier(this);
    }
}
