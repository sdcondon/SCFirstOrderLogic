grammar FirstOrderLogic;

singleSentence: sentence EOF;
sentenceList: (sentences+=sentence SEMICOLON?)* EOF;

singleTerm: term EOF;
termList: (terms+=term SEMICOLON?)* EOF;

singleDeclarationList: (elements+=IDENTIFIER (COMMA elements+=IDENTIFIER)*)? EOF;

sentence: LPAREN sentence RPAREN                   # BracketedSentence
        | LBRACK sentence RBRACK                   # BracketedSentence
        | IDENTIFIER LPAREN argumentList RPAREN    # Predicate
        | term OP_EQUAL term                       # PredicateEquality
        | OP_NOT sentence                          # Negation
        | sentence OP_OR sentence                  # Disjunction
        | sentence OP_AND sentence                 # Conjunction
        | sentence OP_IMPLIES sentence             # Implication
        | sentence OP_EQUIV sentence               # Equivalence
        | OP_FORALL declarationList COMMA sentence # UniversalQuantification
        | OP_EXISTS declarationList COMMA sentence # ExistentialQuantification
        ;

declarationList: (elements+=IDENTIFIER (COMMA? elements+=IDENTIFIER)*)+;

argumentList: (elements+=term (COMMA elements+=term)*)?;

term: IDENTIFIER                            # VariableOrConstant
    | IDENTIFIER LPAREN argumentList RPAREN # Function
    ;

WS: [ \r\t\n]+ -> skip;

IDENTIFIER: [a-zA-Z0-9_]+;

LPAREN: '(';
RPAREN: ')';
LBRACK: '[';
RBRACK: ']';

COMMA: ',';
SEMICOLON: ';';

OP_EQUAL: '=';
OP_NOT: ('¬'|'!'|'not'|'NOT');
OP_OR: ('∨'|'|'|'or'|'OR');
OP_AND: ('∧'|'&'|'and'|'AND');
OP_IMPLIES: ('⇒'|'->'|'=>');
OP_EQUIV: ('⇔'|'<->'|'<=>');
OP_FORALL: ('∀'|'forall'|'FORALL'|'for-all'|'FOR-ALL');
OP_EXISTS: ('∃'|'exists'|'EXISTS'|'there-exists'|'THERE-EXISTS');
