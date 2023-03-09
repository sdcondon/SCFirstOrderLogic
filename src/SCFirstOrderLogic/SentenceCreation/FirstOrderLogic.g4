grammar FirstOrderLogic;

singleSentence: sentence EOF;

sentenceList: (sentences+=sentence ';'?)* EOF;

sentence: '(' sentence ')'                                   # BracketedSentence
        | '[' sentence ']'                                   # BracketedSentence
        | ID '(' termList ')'                                # Predicate
        | term '=' term                                      # PredicateEquality
        | ('¬'|'NOT') sentence                               # Negation
        | sentence ('∨'|'OR') sentence                      # Disjunction
        | sentence ('∧'|'AND') sentence                     # Conjunction
        | sentence ('⇒'|'->'|'=>') sentence                 # Implication
        | sentence ('⇔'|'<->'|'<=>') sentence               # Equivalence
        | ('∀'|'FOR-ALL') identifierList ','? sentence      # UniversalQuantification
        | ('∃'|'THERE-EXISTS') identifierList ','? sentence # ExistentialQuantification
        ;

identifierList: elements+=ID (','? elements+=ID)*;

termList: (elements+=term (',' elements+=term)*)?;

term: ID                   # VariableOrConstant
    | ID '(' termList ')'  # Function
    ;

ID: [a-zA-Z0-9]+;
WS: [ \r\t\n]+ -> skip;