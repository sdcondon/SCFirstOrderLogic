grammar FirstOrderLogic;

singleSentence: sentence EOF;

sentenceList: (sentences+=sentence ';'?)* EOF;

sentence: '(' sentence ')'                                   # BracketedSentence
        | '[' sentence ']'                                   # BracketedSentence
        | ID '(' argumentList ')'                            # Predicate
        | term '=' term                                      # PredicateEquality
        | ('¬'|'NOT') sentence                               # Negation
        | sentence ('∨'|'OR') sentence                      # Disjunction
        | sentence ('∧'|'AND') sentence                     # Conjunction
        | sentence ('⇒'|'->'|'=>') sentence                 # Implication
        | sentence ('⇔'|'<->'|'<=>') sentence               # Equivalence
        | ('∀'|'FOR-ALL') declarationList sentence          # UniversalQuantification
        | ('∃'|'THERE-EXISTS') declarationList sentence     # ExistentialQuantification
        ;

declarationList: (elements+=ID ','?)+;

argumentList: (elements+=term (',' elements+=term)*)?;

term: ID                       # VariableOrConstant
    | ID '(' argumentList ')'  # Function
    ;

ID: [a-zA-Z0-9]+;
WS: [ \r\t\n]+ -> skip;