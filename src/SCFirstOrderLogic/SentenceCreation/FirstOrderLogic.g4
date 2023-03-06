grammar FirstOrderLogic;

sentence: ('∀'|'FOR-ALL') identifierList ','? sentence      # UniversalQuantification
        | ('∃'|'THERE-EXISTS') identifierList ','? sentence # ExistentialQuantification
        | sentence ('∧'|'AND') sentence                     # Conjunction
        | sentence ('∨'|'OR') sentence                      # Disjunction
        | sentence ('⇒'|'->'|'=>') sentence                 # Implication
        | sentence ('⇔'|'<->'|'<=>') sentence               # Equivalence
        | ('¬'|'NOT') sentence                               # Negation
        | ID '(' termList ')'                                # Predicate
        | term '=' term                                      # PredicateEquality
        | '(' sentence ')'                                   # BracketedSentence
        | '[' sentence ']'                                   # BracketedSentence
        ;

identifierList: elements+=ID (','? elements+=ID)*;

termList: (elements+=term)? (',' elements+=term)*;

term: ID                   # VariableOrConstant
    | ID '(' termList ')'  # Function
    ;

ID: [a-zA-Z0-9]+;
WS: [ \r\t\n]+ -> skip;