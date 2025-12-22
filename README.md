# Ratchet

Ratchet is an experimental, minimal programming language implementation written in C# targeting .NET 10. The project includes an ANTLR4 grammar, an AST builder, and documentation for semantic rules and symbol table design. 
Ratchet is intended as a learning / research language for exploring parsing, AST construction, and semantic analysis.

## Features

- ANTLR4 grammar: `Grammar/Ratchet.g4`
- AST construction with visitor pattern (`AST/`)
- Semantic design notes and example programs in `Documentation/`
- Target: .NET 10

## Requirements

- .NET 10 SDK
- (Optional) ANTLR4 tooling if you regenerate the parser from the `.g4` grammar

## Build

From the repository root:
If you change `Grammar/Ratchet.g4`, regenerate the parser (using ANTLR4 tool) and then rebuild.

## Run

If there is an executable project that drives parsing/compilation, run it like:
Replace `<project-path>` with the path to the runnable project in the solution.


## Project layout

- `Grammar/` — ANTLR `.g4` grammar
- `AST/` — AST node types and the `ASTBuilder` visitor
- `Documentation/` — design notes: AST structure, symbol table and semantic rules, examples
- `obj/` — generated parser code (checked in or generated during build)

## Grammar notes

Ratchet follows conventional operator precedence via the grammar (unary, multiplicative, additive, comparison, equality, logical AND, logical OR). 
The grammar uses iterative rules (for example `additive : multiplicative ((PLUS|MINUS) multiplicative)*`) so generated parser contexts expose lists of children — visitors should fold those lists left‑associatively when building binary AST nodes.

Decisions reflected in the grammar and docs:
- Equality and comparison rules can be non‑associative or chaining depending on the chosen rule (`?` vs `*`). Keep grammar and visitor logic consistent.
- `primary` alternatives are labeled and map to `Visit...` methods in the visitor.