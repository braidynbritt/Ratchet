using System;
using System.Collections.Generic;
using System.Text;

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
    }
}
