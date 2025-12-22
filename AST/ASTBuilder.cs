using Antlr4.Runtime.Misc;
using Ratchet.Grammar;

namespace Ratchet.AST
{
    public sealed class ASTBuilder : RatchetBaseVisitor<ASTNode>
    {
        public override ASTNode VisitIntLiteral(RatchetParser.IntLiteralContext context)
        {
            var text = context.INT_LITERAL().GetText();
            var value = int.Parse(text);

            int line = context.start.Line;
            int column = context.start.Column;

            return new LiteralNode(value, line, column);
        }

        public override ASTNode VisitStringLiteral(RatchetParser.StringLiteralContext context)
        {
            var raw = context.GetText();

            int line = context.start.Line;
            int column = context.start.Column;

            var inner = raw.Length >= 2 ? raw.Substring(1, raw.Length - 2) : string.Empty;
            string Unescape(string s) => s
                .Replace("\\n", "\n")
                .Replace("\\r", "\r")
                .Replace("\\t", "\t")
                .Replace("\\\"", "\"")
                .Replace("\\\\", "\\");

            var value = Unescape(inner);

            return new LiteralNode(value, line, column);
        }

        public override ASTNode VisitTrueLiteral(RatchetParser.TrueLiteralContext context)
        {
            int line = context.start.Line;
            int column = context.start.Column;

            return new LiteralNode(true, line, column);
        }

        public override ASTNode VisitFalseLiteral(RatchetParser.FalseLiteralContext context)
        {
            int line = context.start.Line;
            int column = context.start.Column;

            return new LiteralNode(false, line, column);
        }

        public override ASTNode VisitIdentifier(RatchetParser.IdentifierContext context)
        {
            var variableName = context.IDENTIFIER().GetText();

            int line = context.start.Line;
            int column = context.start.Column;
            return new IdentifierNode(variableName, line, column);
        }

        public override ASTNode VisitUnary(RatchetParser.UnaryContext context)
        {
            var startText = context.GetText();

            var opChar = startText[0];
            var op = opChar switch
            {
                '!' => UnaryOperatorType.Not,
                '-' => UnaryOperatorType.Negative,
                _ => throw new NotImplementedException(),
            };
            ASTNode operandNode;
            if (context.unary() != null)
            {
                operandNode = Visit(context.unary());
            }
            else if (context.primary() != null)
            {
                operandNode = Visit(context.primary());
            }
            else
            {
                operandNode = Visit(context.GetChild(1));
            }

            return new UnaryOpNode(op, (ExpressionNode)operandNode, context.start.Line, context.start.Column);
        }

        public override ASTNode VisitAdditive(RatchetParser.AdditiveContext context)
        {
            var left = (ExpressionNode)Visit(context.multiplicative(0));
            
            for (int i = 1; i <= context.multiplicative().Length -1; i++)
            {
                BinaryOperatorType op;
                if(context.PLUS(i -1) != null)
                {
                    op = BinaryOperatorType.Addition;
                }
                else
                {
                    op = BinaryOperatorType.Subtraction;
                }

                var right = (ExpressionNode)Visit(context.multiplicative(i));
                left = new BinaryOpNode(op, left, right, context.start.Line, context.start.Column);
            }
            return left;
        }

        public override ASTNode VisitMultiplicative(RatchetParser.MultiplicativeContext context)
        {
            var left = (ExpressionNode)Visit(context.unary(0));
            for(int i = 1;i <= context.unary().Length - 1; i++)
            {
                BinaryOperatorType op;
                if(context.STAR(i - 1) != null)
                {
                    op = BinaryOperatorType.Multiplication;
                }
                else if (context.SLASH(i - 1) != null)
                {
                    op = BinaryOperatorType.Division;
                }
                else
                {
                    op = BinaryOperatorType.Modulo;
                }
                var right = (ExpressionNode)Visit(context.unary(i));
                left = new BinaryOpNode(op, left, right, context.start.Line, context.start.Column);
            }
            return left;
        }

        public override ASTNode VisitComparison(RatchetParser.ComparisonContext context)
        {
            var left = (ExpressionNode)Visit(context.additive(0));
            for (int i = 1; i <= context.additive().Length - 1; i++)
            {
                BinaryOperatorType op;
                if(context.LT(i - 1) != null)
                {
                    op = BinaryOperatorType.LessThan;
                }
                else if(context.LTE(i - 1) != null)
                {
                    op = BinaryOperatorType.LessThanOrEqual;
                }
                else if (context.GT(i - 1) != null)
                {
                    op = BinaryOperatorType.GreaterThan;
                }
                else
                {
                    op = BinaryOperatorType.GreaterThanOrEqual;
                }
                var right = (ExpressionNode)Visit(context.additive(i));
                left = new BinaryOpNode(op, left, right, context.start.Line, context.start.Column);
            }
            return left;
        }

        public override ASTNode VisitEquality(RatchetParser.EqualityContext context)
        {
            var left = (ExpressionNode)Visit(context.comparison(0));
            for (int i = 1; i < context.comparison().Length; i++)
            {
                BinaryOperatorType op;
                if (context.GetToken(RatchetParser.EQ, i - 1) != null)
                {
                    op = BinaryOperatorType.Equals;
                }
                else
                {
                    op = BinaryOperatorType.NotEquals;
                }
                var right = (ExpressionNode)Visit(context.comparison(i));
                left = new BinaryOpNode(op, left, right, context.start.Line, context.start.Column);
            }
            return left;
        }

        public override ASTNode VisitLogicalAnd(RatchetParser.LogicalAndContext context)
        {
            var left = (ExpressionNode)Visit(context.equality(0));
            for (int i = 1; i < context.equality().Length; i++)
            {
                var right = (ExpressionNode)Visit(context.equality(i));
                left = new BinaryOpNode(BinaryOperatorType.And, left, right, context.start.Line, context.start.Column);
            }
            return left;
        }

        public override ASTNode VisitLogicalOr([NotNull] RatchetParser.LogicalOrContext context)
        {
            var left = (ExpressionNode)Visit(context.logicalAnd(0));
            for (int i = 1; i < context.logicalAnd().Length; i++)
            {
                var right = (ExpressionNode)Visit(context.logicalAnd(i));
                left = new BinaryOpNode(BinaryOperatorType.Or, left, right, context.start.Line, context.start.Column);
            }
            return left;
        }

        public override ASTNode VisitAssignment(RatchetParser.AssignmentContext context)
        {
            var identifier = context.IDENTIFIER().GetText();
            var exprNode = (ExpressionNode)Visit(context.expr());
            int line = context.start.Line;
            var column = context.start.Column;
            return new AssignmentNode(identifier, exprNode, line, column);
        }

        public override ASTNode VisitFunctionCall(RatchetParser.FunctionCallContext context)
        {
            var identifier = context.IDENTIFIER().GetText();
            var argumentList = new List<ExpressionNode>();
            var argsContext = context.argumentList();
            if (argsContext != null)
            {
                foreach (var exprCtx in argsContext.expr())
                {
                    argumentList.Add((ExpressionNode)Visit(exprCtx));
                }
            }
            int line = context.start.Line;
            var column = context.start.Column;
            return new FunctionCallNode(identifier, argumentList, line, column);
        }

        public override ASTNode VisitExpr(RatchetParser.ExprContext context)
        {
            var expr = (ExpressionNode)Visit(context.logicalOr());
            int line = context.start.Line;
            int column = context.start.Column;
            return new ExpressionStatementNode(expr, line, column);
        }

        public override ASTNode VisitVarDecl(RatchetParser.VarDeclContext context)
        {
            var identifier = context.IDENTIFIER().GetText();

            var type = context.type() is not null
                ? (TypeNode)Visit(context.type())
                : null;

            var expression = context.expr() is not null
                ? (ExpressionNode)Visit(context.expr())
                : null;

            int line = context.start.Line;
            var column = context.start.Column;
            return new VarDeclNode(identifier, type, expression, line, column);
        }


        public override ASTNode VisitReturnStatement(RatchetParser.ReturnStatementContext context)
        {
            var expr = context.expr() is not null
                ? (ExpressionNode)Visit(context.expr())
                : null;
            int line = context.start.Line;
            int column = context.start.Column;
            return new ReturnNode(expr, line, column);
        }


        public override ASTNode VisitBlock(RatchetParser.BlockContext context)
        {
            var statementList = new List<StatementNode>();
            var statementContexts = context.statement();
            if (statementContexts != null)
            {
                foreach (var stmtCtx in statementContexts)
                {
                    var node = (StatementNode)Visit(stmtCtx);
                    statementList.Add(node);
                }
            }
            int line = context.start.Line;
            int column = context.start.Column;
            return new BlockNode(statementList, line, column);
        }

        public override ASTNode VisitIfStatement(RatchetParser.IfStatementContext context)
        {
            var condition = (ExpressionNode)Visit(context.expr());
            var trueBlock = (BlockNode)Visit(context.block());
            var elseBlock = (BlockNode)Visit(context.elseBlock());

            List<ElseIfNode>? elseIfNodes = null;
            var elifContexts = context.elseIf();
            if (elifContexts != null && elifContexts.Length > 0)
            {
                elseIfNodes = new List<ElseIfNode>();
                foreach (var elifCtx in elifContexts)
                {
                    elseIfNodes.Add((ElseIfNode)Visit(elifCtx));
                }
            }

            int line = context.start.Line;
            int column = context.start.Column;
            return new IfStatementNode(condition, trueBlock, elseBlock, elseIfNodes, line, column);
        }

        public override ASTNode VisitElseIf(RatchetParser.ElseIfContext context)
        {
            var condition = (ExpressionNode)Visit(context.expr());
            var trueBlock = (BlockNode)Visit(context.block());

            int line = context.start.Line;
            int column = context.start.Column;
            return new ElseIfNode(condition, trueBlock, line, column);
        }

        public override ASTNode VisitElseBlock(RatchetParser.ElseBlockContext context)
        {
            var blockCtx = context.block();
            if (blockCtx is null)
            {
                int line = context.start.Line;
                int column = context.start.Column;
                return new BlockNode(new List<StatementNode>(), line, column);
            }
            return Visit(blockCtx);
        }

        public override ASTNode VisitWhileStatement(RatchetParser.WhileStatementContext context)
        {
            var condition = (ExpressionNode)Visit(context.expr());
            var body = (BlockNode)Visit(context.block());

            int line = context.start.Line;
            int column = context.start.Column;
            return new WhileNode(condition, body, line, column);
        }

        public override ASTNode VisitFunctionDecl(RatchetParser.FunctionDeclContext context)
        {
            var identifier = context.IDENTIFIER().GetText();

            TypeNode? returnType = null;
            if (context.returnType() is not null)
            {
                returnType = (TypeNode)Visit(context.returnType());
            }

            List<VarDeclNode>? paramList = null;
            var paramListCtx = context.paramList();
            if (paramListCtx != null)
            {
                paramList = new List<VarDeclNode>();
                foreach (var paramCtx in paramListCtx.param())
                {
                    var pNode = (VarDeclNode)Visit(paramCtx);
                    paramList.Add(pNode);
                }
            }

            var blockCtx = context.block();
            var body = (BlockNode)Visit(blockCtx);

            int line = context.start.Line;
            int column = context.start.Column;
            return new FunctionDeclNode(identifier, paramList, returnType, body, line, column);
        }

        public override ASTNode VisitProgram(RatchetParser.ProgramContext context)
        {
            var FuncDecls = context.functionDecl();
            var functions = new List<FunctionDeclNode>();
            foreach (var func in FuncDecls)
            {
                functions.Add((FunctionDeclNode)Visit(func));
            }
            int line = context.start.Line;
            int column = context.start.Column;
            return new ProgramNode(functions, line, column);
        }
    }
}