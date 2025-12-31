using Ratchet.AST;
using System.Data.Common;

namespace Ratchet.Semantic
{
    /*
        This pass defines symbols, resolves indentifiers, and enforces scope rules
    */
    public class SemanticAnalyzer : IAstVisitor<object>
    {
        private Scope currentScope;
        private List<ErrorType> semanticErrors;

        private FunctionSymbol? currentFunctionSymbol;

        public object VisitType(TypeNode node) { return null; }
        public object VisitLiteral(LiteralNode node) { return null; }

        public object VisitProgram(ProgramNode node) 
        {
            currentScope = new Scope();
            semanticErrors = new List<ErrorType>();

            foreach(var function in node.Functions)
            {
                function.Accept(this);
            }
            return null;     
        }
        public object VisitFunctionDecl(FunctionDeclNode node)
        {           
            var paramTypes = node.ParamList?.Select(p => p.Type?.Name).ToList() ?? new List<string?>();

            if (currentScope.ExistsInCurrentScope(node.FunctionName))
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Function {node.FunctionName} already defined in this scope.", node.Line, node.Column));
            }
            else
            {
                currentScope.DefineFunction(node.FunctionName, paramTypes, node.Type?.Name);
            }

            var functionSymbol = currentScope.GetFunction(node.FunctionName);

            var previousFunction = currentFunctionSymbol;
            currentFunctionSymbol = functionSymbol;

            Scope newScope = new Scope { ParentScope = currentScope };
            currentScope = newScope;

            if (node.ParamList != null)
            {
                foreach (var param in node.ParamList)
                {
                    if (currentScope.ExistsInCurrentScope(param.VariableName))
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Parameter {param.VariableName} already defined in this scope.", param.Line, param.Column));
                    }
                    else
                    {
                        currentScope.Define(param.VariableName, param.Type?.Name, param.Type?.Name == null);
                    }
                }
            }

            node.Body.Accept(this);

            if (functionSymbol != null && node.ParamList != null)
            {
                for (int i = 0; i < node.ParamList.Count; i++)
                {
                    var paramName = node.ParamList[i].VariableName;
                    var paramSym = currentScope.GetSymbol(paramName);
                    var inferred = paramSym?.TypeName;

                    if (functionSymbol.ParamTypes.Count > i)
                    {
                        var declared = functionSymbol.ParamTypes[i];
                        if (declared != null && inferred != null && declared != inferred)
                        {
                            semanticErrors.Add(new ErrorType($"Semantic Error: Parameter '{paramName}' declared as '{declared}' but inferred as '{inferred}' in function '{node.FunctionName}'.", node.Line, node.Column));
                        }
                        else if (declared == null && inferred != null)
                        {
                            functionSymbol.ParamTypes[i] = inferred;
                        }
                    }
                }
            }

            currentScope = currentScope.ParentScope;
            currentFunctionSymbol = previousFunction;

            return null; 
        }
        public object VisitBinaryOp(BinaryOpNode node) 
        { 
            if (node.Left == null || node.Right == null)
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Binary operator '{node.Operator}' missing operand(s).", node.Line, node.Column));
                return null;
            }

            // Cache inferred types
            var leftType = InferExpressionType(node.Left);
            var rightType = InferExpressionType(node.Right);

            if (leftType != rightType)
            {
                semanticErrors.Add(new ErrorType(
                    $"Semantic Error: Binary operator '{node.Operator}' operands type mismatch: left is '{leftType ?? "unknown"}', right is '{rightType ?? "unknown"}'.",
                    node.Line, node.Column));
            }

            node.Left.Accept(this);
            node.Right.Accept(this);

            if ((node.Operator == BinaryOperatorType.Division || node.Operator == BinaryOperatorType.Modulo)
                && node.Right is LiteralNode rightLit
                && ((rightLit.Value is int i && i == 0) || (rightLit.Value is long l && l == 0L)))
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Division or modulo by zero.", node.Line, node.Column));
            }

            if (node.Operator == BinaryOperatorType.Addition ||
                node.Operator == BinaryOperatorType.Subtraction ||
                node.Operator == BinaryOperatorType.Multiplication ||
                node.Operator == BinaryOperatorType.Division ||
                node.Operator == BinaryOperatorType.Modulo)
            {
                if (leftType != "int" || rightType != "int")
                {
                    semanticErrors.Add(new ErrorType($"Semantic Error: Arithmetic operator '{node.Operator}' requires 'int' operands.", node.Line, node.Column));
                }
            }
            else if (node.Operator == BinaryOperatorType.LessThan ||
                     node.Operator == BinaryOperatorType.LessThanOrEqual ||
                     node.Operator == BinaryOperatorType.GreaterThan ||
                     node.Operator == BinaryOperatorType.GreaterThanOrEqual)
            {
                if (leftType != "int" || rightType != "int")
                {
                    semanticErrors.Add(new ErrorType($"Semantic Error: Comparison operator '{node.Operator}' requires 'int' operands.", node.Line, node.Column));
                }
            }
            else if (node.Operator == BinaryOperatorType.And || node.Operator == BinaryOperatorType.Or)
            {
                if (leftType != "bool" || rightType != "bool")
                {
                    semanticErrors.Add(new ErrorType($"Semantic Error: Logical operator '{node.Operator}' requires 'bool' operands.", node.Line, node.Column));
                }
            }

            return null; 
        }
        public object VisitUnaryOp(UnaryOpNode node) 
        { 
            if (node.Operand == null)
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Unary operator '{node.Operator}' missing operand.", node.Line, node.Column));
                return null;
            }

            node.Operand.Accept(this);

            string? operandType = InferExpressionType(node.Operand);

            if(node.Operator == UnaryOperatorType.Negative)
            {
                if(operandType != "int")
                {
                    semanticErrors.Add(new ErrorType($"Semantic Error: Unary '-' operator requires 'int' operand, got '{operandType ?? "unknown"}'.", node.Line, node.Column));
                }
            }
            else if(node.Operator == UnaryOperatorType.Not)
            {
                if (operandType != "bool")
                {
                    semanticErrors.Add(new ErrorType($"Semantic Error: Unary '!' operator requires 'bool' operand, got '{operandType ?? "unknown"}'.", node.Line, node.Column));
                }
            }

            return null; 
        }
        public object VisitIdentifier(IdentifierNode node) 
        { 
            if (!currentScope.ExistsInAnyScope(node.VariableName))
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Variable {node.VariableName} not defined in this scope.", node.Line, node.Column));
            }
            return null; 
        }
        public object VisitFunctionCall(FunctionCallNode node)
        {
            var func = currentScope.GetFunction(node.FunctionName);
            if (func == null)
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Function {node.FunctionName} not defined in this scope.", node.Line, node.Column));
            }
            else
            {
                if (func.ParamTypes.Count != node.ArgumentList.Count)
                {
                    semanticErrors.Add(new ErrorType($"Semantic Error: Function {node.FunctionName} expects {func.ParamTypes.Count} args, got {node.ArgumentList.Count}.", node.Line, node.Column));
                }

                for (int i = 0; i < Math.Min(func.ParamTypes.Count, node.ArgumentList.Count); i++)
                {
                    var arg = node.ArgumentList[i];
                    var argType = InferExpressionType(arg);
                    var expected = func.ParamTypes[i];

                    if (expected != null && argType != null && expected != argType)
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Argument {i+1} of {node.FunctionName} expects {expected}, got {argType}.", arg.Line, arg.Column));
                    }
                    else if (expected == null && argType != null)
                    {
                        func.ParamTypes[i] = argType;
                    }
                    arg.Accept(this);
                }
            }

            if (func == null)
            {
                foreach (var arg in node.ArgumentList) arg.Accept(this);
            }

            return null; 
        }
        public object VisitAssignment(AssignmentNode node) 
        { 
            var rhs = node.Expression;
            string? inferredType = InferExpressionType(rhs);
           
            if (!currentScope.ExistsInAnyScope(node.VariableName))
            {
                currentScope.Define(node.VariableName, inferredType, inferredType == null);
                node.Expression.Accept(this);
            }
            else
            {
                var symbol = currentScope.GetSymbol(node.VariableName);
                if (!string.IsNullOrEmpty(symbol.TypeName))
                {
                    if (inferredType != null && inferredType != symbol.TypeName)
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Type mismatch in assignment to variable {node.VariableName}. Expected {symbol.TypeName}, got {inferredType}.", node.Line, node.Column));
                    }
                }
                else
                {
                    if(inferredType != null)
                    {
                        symbol.TypeName = inferredType;
                    }
                }
            }
            return null; 
        }
        public object VisitVarDecl(VarDeclNode node) 
        {
            if (currentScope.ExistsInCurrentScope(node.VariableName))
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: Variable {node.VariableName} already defined in this scope.", node.Line, node.Column));
            }
            else
            {
                currentScope.Define(node.VariableName);
            }
            node.Expression?.Accept( this );

            return null; 
        
        }
        public object VisitIf(IfStatementNode node)
        {
            node.Condition.Accept(this);

            string? inferredType = InferExpressionType(node.Condition);
            if (inferredType != "bool")
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: If condition must be boolean, got {inferredType ?? "unknown"}.", node.Condition.Line, node.Condition.Column));
            }

            node.TrueBlock.Accept(this);

            if (node.FalseBlock != null)
            {
                node.FalseBlock.Accept(this);
            }
            if (node.ElseIfNodes != null)
            {
                foreach (var elseIf in node.ElseIfNodes)
                {
                    elseIf.Accept(this);
                }
            }

            return null;
        }
        public object VisitElseIf(ElseIfNode node) 
        {
            node.Condition.Accept(this);

            string? inferredType = InferExpressionType(node.Condition);
            if (inferredType != "bool")
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: If condition must be boolean, got {inferredType ?? "unknown"}.", node.Condition.Line, node.Condition.Column));
            }

            node.TrueBlock.Accept(this);

            return null;
        }
        public object VisitWhile(WhileNode node) 
        {
            node.Condition.Accept(this);

            string? inferredType = InferExpressionType(node.Condition);
            if (inferredType != "bool")
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: If condition must be boolean, got {inferredType ?? "unknown"}.", node.Condition.Line, node.Condition.Column));
            }

            node.Body.Accept(this);
            return null; 
        }
        public object VisitReturn(ReturnNode node) 
        {
            if (currentFunctionSymbol == null)
            {
                semanticErrors.Add(new ErrorType($"Semantic Error: 'return' used outside of a function.", node.Line, node.Column));
                node.Expression?.Accept(this);
                return null;
            }

            string? returnedType = null;
            if (node.Expression != null)
            {
                returnedType = InferExpressionType(node.Expression);
                node.Expression.Accept(this);
            }
            else
            {
                returnedType = "void";
            }

            var declared = currentFunctionSymbol.ReturnType;
            if (!string.IsNullOrEmpty(declared))
            {
                if (declared == "void")
                {
                    if (node.Expression != null)
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Returning a value from void function '{currentFunctionSymbol.Name}'.", node.Line, node.Column));
                    }
                }
                else
                {
                    if (returnedType == "void")
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Function '{currentFunctionSymbol.Name}' must return '{declared}' but return has no value.", node.Line, node.Column));
                    }
                    else if (returnedType != null && returnedType != declared)
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Return type mismatch in function '{currentFunctionSymbol.Name}'. Expected '{declared}', got '{returnedType}'.", node.Line, node.Column));
                    }
                }
            }
            else
            {
                if (currentFunctionSymbol.ReturnType == null)
                {
                    currentFunctionSymbol.ReturnType = returnedType;
                }
                else
                {
                    var inferred = currentFunctionSymbol.ReturnType;
                    if (inferred != returnedType && returnedType != null)
                    {
                        semanticErrors.Add(new ErrorType($"Semantic Error: Inconsistent return types in function '{currentFunctionSymbol.Name}'. Expected '{inferred ?? "unknown"}', got '{returnedType}'.", node.Line, node.Column));
                    }
                }
            }

            return null;
        }
        public object VisitBlock(BlockNode node) 
        {
            Scope newScope = new Scope { ParentScope = currentScope };

            currentScope = newScope;
            foreach (var stmt in node.StatementNodes)
            {
                stmt.Accept(this);
            }

            currentScope = currentScope.ParentScope;
            return null; 
        }
        public object VisitExpressionStatement(ExpressionStatementNode node) 
        { 
            node.Expression.Accept(this);
            return null; 
        }

        private string? InferExpressionType(object expr)
        {
            if (expr is LiteralNode lit)
            {
                return lit.Value switch
                {
                    int => "int",
                    long => "int",
                    string => "string",
                    bool => "bool",
                    _ => null
                };
            }

            if (expr is IdentifierNode id)
            {
                var sym = currentScope.GetSymbol(id.VariableName);
                return sym?.TypeName;
            }

            if (expr is BinaryOpNode bin)
            {
                switch (bin.Operator)
                {
                    case BinaryOperatorType.Addition:
                    case BinaryOperatorType.Subtraction:
                    case BinaryOperatorType.Multiplication:
                    case BinaryOperatorType.Division:
                    case BinaryOperatorType.Modulo:
                        return "int";
                    case BinaryOperatorType.Equals:
                    case BinaryOperatorType.NotEquals:
                    case BinaryOperatorType.LessThan:
                    case BinaryOperatorType.LessThanOrEqual:
                    case BinaryOperatorType.GreaterThan:
                    case BinaryOperatorType.GreaterThanOrEqual:
                    case BinaryOperatorType.And:
                    case BinaryOperatorType.Or:
                        return "bool";
                    default:
                        return null;
                }
            }

            if (expr is UnaryOpNode un)
            {
                return un.Operator switch
                {
                    UnaryOperatorType.Negative => "int",
                    UnaryOperatorType.Not => "bool",
                    _ => null
                };
            }

            return null;
        }
    }
}
