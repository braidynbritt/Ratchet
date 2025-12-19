using System;
using System.Collections.Generic;
using System.Text;

namespace Ratchet.AST
{
    public interface IAstVisitor<T>
    {
        //Top-Level
        T VisitProgram(ProgramNode node);
        T VisitFunctionDecl(FunctionDeclNode node);
        T VisitType(TypeNode node);

        //Expressions
        T VisitBinaryOp(BinaryOpNode node);
        T VisitUnaryOp(UnaryOpNode node);
        T VisitLiteral(LiteralNode node);
        T VisitIdentifier(IdentifierNode node);
        T VisitFunctionCall(FunctionCallNode node);

        //Statements
        T VisitAssignment(AssignmentNode node);
        T VisitVarDecl(VarDeclNode node);
        T VisitIf(IfStatementNode node);
        T VisitElseIf(ElseIfNode node);
        T VisitWhile(WhileNode node);
        T VisitReturn(ReturnNode node);
        T VisitBlock(BlockNode node);
        T VisitExpressionStatement(ExpressionStatementNode node);
    }
}
