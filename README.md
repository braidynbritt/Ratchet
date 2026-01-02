# Ratchet

Ratchet is an experimental, minimal programming language implementation written in C# targeting .NET 10. It uses ANTLR4 for parsing, constructs an AST, and performs a semantic analysis pass that includes symbol table management, parameter/return inference, and initialization checks.

## Key features

- Parser: ANTLR4 grammar in `Grammar/Ratchet.g4`.
- AST construction: `AST/ASTBuilder.cs` and typed AST nodes in `AST/`.
- Semantic analysis: `Semantic/SemanticAnalyzer.cs` with:
  - Scope and symbol management (`Semantic/Scope.cs`, `Symbols/`).
  - `FunctionSymbol` recording of parameter types and return type.
  - Parameter type inference from usages when parameters are untyped.
  - Function return inference: first return sets inferred return type when none declared.
  - Enforcement of declared return types and requirement that functions with a declared non-void return type actually return on required paths.
  - `Symbol.IsInitialized` tracking and use-before-initialization errors.
  - Basic type checking for literals, unary/binary operators, assignments and calls.
- Example programs: `Documentation/ExampleLanguage.txt`.
- Design docs: `Documentation/SemanticRules.txt`, `Documentation/SymbolTableDesign.txt`.

## Current semantics (short)

- Primitive types: `int`, `bool`, `string`, plus `void`.
- Variables:
  - Can be declared typed (`int: x = 5`) or untyped and inferred from initializer/assignment.
  - `Symbol.IsInitialized` is tracked; using an uninitialized variable yields a semantic error.
- Functions:
  - Function symbols (name, parameter types, return type) are created before analyzing bodies (supports recursion).
  - Parameters may be untyped; semantic pass attempts to infer their types from usage. Conflicts produce errors.
  - Return rules:
    - If a function has an explicit return type other than `void`, it must return a matching value on the required code paths (the analyzer checks that the function body guarantees a return on those paths).
    - If no return type is declared:
      - If the function contains at least one `return`, the first return sets the function return type and subsequent returns must match.
      - If the function contains no `return`, it defaults to returning `void`.
- Function calls:
  - Checked for existence, arity, and (when available) argument type compatibility. Untyped parameters may be inferred from call sites or body usage.

## Project layout

- `Grammar/` — ANTLR `.g4` grammar.
- `AST/` — AST node types and the `ASTBuilder` visitor.
- `Semantic/` — `SemanticAnalyzer`, `Scope` and related logic.
- `Symbols/` — `Symbol`, `FunctionSymbol`.
- `Documentation/` — design notes and examples.
- `obj/` — generated parser code (may be checked-in or regenerated during build).

## Build

Prerequisites:
- .NET 10 SDK.
- (Optional) ANTLR4 tooling to regenerate parser when `Grammar/Ratchet.g4` changes.

Build from repository root:

```
dotnet build
```

## License

MIT — see `LICENSE.txt`.