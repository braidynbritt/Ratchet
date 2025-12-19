using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public sealed class AssignmentNode : StatementNode
    {
        public string VariableName { get; }
        public ExpressionNode Expression { get; }

        public AssignmentNode(
            string variableName,
            ExpressionNode expression,
            int line,
            int column
        ) : base(line, column)
        {
            VariableName = variableName;
            Expression = expression;
        }
        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitAssignment( this );
    }
}
