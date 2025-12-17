grammar FirstOrderLogic;

singleFormula
    : formula EOF
    ;

formulaList
    : (formulas+=formula SEMICOLON?)* EOF
    ;

singleTerm
    : term EOF
    ;

termList
    : (terms+=term SEMICOLON?)* EOF
    ;

singleDeclarationList
    : (elements+=IDENTIFIER (COMMA elements+=IDENTIFIER)*)? EOF
    ;

formula
    : LPAREN formula RPAREN                   # BracketedFormula
    | LBRACK formula RBRACK                   # BracketedFormula
    | IDENTIFIER LPAREN argumentList RPAREN   # Predicate
    | term OP_EQUAL term                      # PredicateEquality
    | OP_NOT formula                          # Negation
    | formula OP_OR formula                   # Disjunction
    | formula OP_AND formula                  # Conjunction
    | formula OP_IMPLIES formula              # Implication
    | formula OP_EQUIV formula                # Equivalence
    | OP_FORALL declarationList COMMA formula # UniversalQuantification
    | OP_EXISTS declarationList COMMA formula # ExistentialQuantification
    ;

declarationList
    : (elements+=IDENTIFIER (COMMA? elements+=IDENTIFIER)*)+
    ;

argumentList
    : (elements+=term (COMMA elements+=term)*)?
    ;

term
    : IDENTIFIER                            # VariableOrConstant
    | IDENTIFIER LPAREN argumentList RPAREN # Function
    ;

LPAREN
    : '('
    ;

RPAREN
    : ')'
    ;

LBRACK
    : '['
    ;

RBRACK
    : ']'
    ;

COMMA
    : ','
    ;

SEMICOLON
    : ';'
    ;

OP_EQUAL
    : '='
    ;

OP_NOT
    : '¬'
    | '!'
    | 'not'
    | 'NOT'
    ;

OP_OR
    : '∨'
    | '|'
    | 'or'
    | 'OR'
    ;

OP_AND
    : '∧'
    | '&' 
    | 'and'
    | 'AND'
    ;

OP_IMPLIES
    : '⇒'
    | '->'
    | '=>'
    ;

OP_EQUIV
    : '⇔'
    | '<->'
    | '<=>'
    ;

OP_FORALL
    : '∀'
    | 'forall'
    | 'FORALL'
    | 'for-all'
    | 'FOR-ALL'
    ;

OP_EXISTS
    : '∃'
    | 'exists'
    | 'EXISTS'
    | 'there-exists'
    | 'THERE-EXISTS'
    ;

IDENTIFIER
    : [a-zA-Z0-9_]+
    ;

WHITESPACE
    : [ \r\t\n]+ -> skip
    ;
