using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public sealed class FunctionDeclNode : ASTNode
    {
        public string FunctionName { get; }
        public List<VarDeclNode>? ParamList { get; }
        public TypeNode? Type { get; }
        public BlockNode Body { get; }
        
        public FunctionDeclNode(
            string functionName,
            List<VarDeclNode>? paramList,
            TypeNode? type,
            BlockNode body,
            int line,
            int column
        ) : base ( line, column )
        {
            FunctionName = functionName;
            ParamList = paramList;
            Type = type;
            Body = body;
        }

        public override T Accept<T>(IAstVisitor<T> visitor)
            => visitor.VisitFunctionDecl(this);
    }
}
