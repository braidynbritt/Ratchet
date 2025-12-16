grammar Ratchet;

FUNC    : 'func';
IF      : 'if';
ELSE    : 'else';
WHILE   : 'while';
RETURN  : 'return';

INT     : 'int';
BOOL    : 'bool';
STRING  : 'string';
VOID    : 'void';

TRUE    : 'true';
FALSE   : 'false';

PLUS        : '+';
MINUS       : '-';
STAR        : '*';
SLASH       : '/';
PERCENT     : '%';

EQ          : '==';
NEQ         : '!=';
LT          : '<';
LTE         : '<=';
GT          : '>';
GTE         : '>=';

AND         : '&';
OR          : '|';
NOT         : '!';

ASSIGN      : '=';

LPAREN      : '(';
RPAREN      : ')';
LBRACE      : '{';
RBRACE      : '}';
COLON       : ':';
COMMA       : ',';
SEMI        : ';';

INT_LITERAL
    : [0-9]+
    ;

STRING_LITERAL
    : '"' ( '\\' . | ~["\\] )* '"'
    | '\'' ( '\\' . | ~['\\] )* '\''
    ;

IDENTIFIER
    : [a-zA-Z_][a-zA-Z0-9_]*
    ;

WS
    : [ \t\r\n]+ -> skip
    ;

LINE_COMMENT
    : '//' ~[\r\n]* -> skip
    ;

BLOCK_COMMENT
    : '/*' .*? '*/' -> skip
    ;

program
    : functionDecl* EOF
    ;

functionDecl
    : FUNC returnType? IDENTIFIER LPAREN paramList? RPAREN block
    ;

returnType
    : type COLON
    ;

paramList
    : param (COMMA param)*
    ;

param
    : type? IDENTIFIER
    ;

type
    : INT
    | BOOL
    | STRING
    | VOID
    ;

statement
    : varDecl SEMI
    | assignment SEMI
    | expr SEMI
    | ifStatement
    | whileStatement
    | returnStatement SEMI
    | block
    ;

varDecl
    : type COLON IDENTIFIER ASSIGN expr
    ;

assignment
    : IDENTIFIER ASSIGN expr
    ;

returnStatement
    : RETURN expr?
    ;

block
    : LBRACE statement* RBRACE
    ;

ifStatement
    : IF expr block elseIf* elseBlock?
    ;

elseIf
    : ELSE IF expr block
    ;

elseBlock
    : ELSE block
    ;

whileStatement
    : WHILE expr block
    ;

expr
    : logicalOr
    ;

logicalOr
    : logicalAnd (OR logicalAnd)*
    ;

logicalAnd
    : equality (AND equality)*
    ;

equality
    : comparison ((EQ | NEQ) comparison)?
    ;

comparison
    : additive ((LT | LTE | GT | GTE) additive)*
    ;

additive
    : multiplicative ((PLUS | MINUS) multiplicative)*
    ;

multiplicative
    : unary ((STAR | SLASH | PERCENT) unary)*
    ;

unary
    : (NOT | MINUS) unary
    | primary
    ;

primary
    : INT_LITERAL
    | STRING_LITERAL
    | TRUE
    | FALSE
    | IDENTIFIER
    | functionCall
    | LPAREN expr RPAREN
    ;

functionCall
    : IDENTIFIER LPAREN argumentList? RPAREN
    ;

argumentList
    : expr (COMMA expr)*
    ;
