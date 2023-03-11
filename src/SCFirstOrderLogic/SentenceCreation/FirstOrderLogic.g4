grammar FirstOrderLogic;

singleSentence: sentence EOF;

sentenceList: (sentences+=sentence ';'?)* EOF;

sentence: '(' sentence ')'                                                                 # BracketedSentence
        | '[' sentence ']'                                                                 # BracketedSentence
        | ID '(' argumentList ')'                                                          # Predicate
        | term '=' term                                                                    # PredicateEquality
        | ('¬'|'!'|'not'|'NOT') sentence                                                   # Negation
        | sentence ('∨'|'|'|'or'|'OR') sentence                                           # Disjunction
        | sentence ('∧'|'&'|'and'|'AND') sentence                                         # Conjunction
        | sentence ('⇒'|'->'|'=>') sentence                                               # Implication
        | sentence ('⇔'|'<->'|'<=>') sentence                                             # Equivalence
        | ('∀'|'forall'|'FORALL'|'for-all'|'FOR-ALL') declarationList sentence            # UniversalQuantification
        | ('∃'|'exists'|'EXISTS'|'there-exists'|'THERE-EXISTS') declarationList sentence  # ExistentialQuantification
        ;

declarationList: (elements+=ID ','?)+;

argumentList: (elements+=term (',' elements+=term)*)?;

term: ID                       # VariableOrConstant
    | ID '(' argumentList ')'  # Function
    ;

ID: [a-zA-Z0-9]+;
WS: [ \r\t\n]+ -> skip;