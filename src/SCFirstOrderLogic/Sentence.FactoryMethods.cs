namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    public abstract partial class Sentence
    {
        //// Shorthand factory methods. Not sure if I like having these in the sentence class itself or not.
        //// Separating them into their own class would make it easier to include guidance (notably, how best
        //// to create your own properties for variables and constants and methods for functions and predicates).
        
        public static Sentence ForAll(VariableDeclaration variableDeclaration, Sentence sentence) =>
            new UniversalQuantification(variableDeclaration, sentence);

        public static Sentence ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Sentence sentence) => 
            new UniversalQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, sentence));

        public static Sentence ForAll(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Sentence sentence) => 
            new UniversalQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, new UniversalQuantification(variableDeclaration3, sentence)));

        public static Sentence ThereExists(VariableDeclaration variableDeclaration, Sentence sentence) => 
            new ExistentialQuantification(variableDeclaration, sentence);

        public static Sentence ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, Sentence sentence) => 
            new ExistentialQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, sentence));

        public static Sentence ThereExists(VariableDeclaration variableDeclaration1, VariableDeclaration variableDeclaration2, VariableDeclaration variableDeclaration3, Sentence sentence) =>
            new UniversalQuantification(variableDeclaration1, new UniversalQuantification(variableDeclaration2, new UniversalQuantification(variableDeclaration3, sentence)));

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="Conjunction"/>(s) of two (or more) operands.
        /// </summary>
        /// <param name="operand1">The first operand of the conjunction.</param>
        /// <param name="operand2">The second operand of the conjunction.</param>
        /// <param name="otherOperands">Any additional operands.</param>
        /// <returns>A new <see cref="Conjunction"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCFirstOrderLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence And(Sentence operand1, Sentence operand2, params Sentence[] otherOperands)
        {
            var conjunction = new Conjunction(operand1, operand2);

            foreach (var operand in otherOperands)
            {
                conjunction = new Conjunction(conjunction, operand);
            }

            return conjunction;
        }

        /// <summary>
        /// Shorthand factory method for a (tree of) new <see cref="Disjunction"/>(s) of two (or more) operands.
        /// </summary>
        /// <param name="operand1">The first operand of the disjunction.</param>
        /// <param name="operand2">The second operand of the disjunction.</param>
        /// <param name="otherOperands">Any additional operands.</param>
        /// <returns>A new <see cref="Disjunction"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCFirstOrderLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Or(Sentence operand1, Sentence operand2, params Sentence[] otherOperands)
        {
            var disjunction = new Disjunction(operand1, operand2);

            foreach (var operand in otherOperands)
            {
                disjunction = new Disjunction(disjunction, operand);
            }

            return disjunction;
        }

        /// <summary>
        /// Shorthand factory method for a new <see cref="Implication"/> instance.
        /// </summary>
        /// <param name="antecedent">The antecedent sentence of the implication.</param>
        /// <param name="consequent">The consequent sentence of the implication.</param>
        /// <returns>A new <see cref="Implication"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCFirstOrderLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence If(Sentence antecedent, Sentence consequent) => 
            new Implication(antecedent, consequent);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Equivalence"/> instance.
        /// </summary>
        /// <param name="left">The left-hand operand of the equivalence.</param>
        /// <param name="right">The right-hand operand of the equivalence.</param>
        /// <returns>A new <see cref="Equivalence"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCFirstOrderLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Iff(Sentence left, Sentence right) => 
            new Equivalence(left, right);

        /// <summary>
        /// Shorthand factory method for a new <see cref="Negation"/> instance.
        /// </summary>
        /// <param name="sentence">The negated sentence.</param>
        /// <returns>A new <see cref="Negation"/> instance.</returns>
        /// <remarks>
        /// Obviously, using 'using static SCFirstOrderLogic.Sentence;' can make use of these methods fairly succinct.
        /// Alternatively, see the <see cref="LanguageIntegration.SentenceFactory"/> class for another shorthand method of creating sentences.
        /// </remarks>
        public static Sentence Not(Sentence sentence) => 
            new Negation(sentence);

        public static Sentence AreEqual(Term left, Term right) =>
            new Equality(left, right);

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "A".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference A => new VariableReference(nameof(A));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "B".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference B => new VariableReference(nameof(B));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "C".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference C => new VariableReference(nameof(C));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "D".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference D => new VariableReference(nameof(D));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "E".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference E => new VariableReference(nameof(E));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "F".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference F => new VariableReference(nameof(F));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "G".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference G => new VariableReference(nameof(G));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "H".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference H => new VariableReference(nameof(H));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "I".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference I => new VariableReference(nameof(I));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "J".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference J => new VariableReference(nameof(J));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "K".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference K => new VariableReference(nameof(K));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "L".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference L => new VariableReference(nameof(L));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "M".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference M => new VariableReference(nameof(M));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "N".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference N => new VariableReference(nameof(N));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "O".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference O => new VariableReference(nameof(O));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "P".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference P => new VariableReference(nameof(P));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "Q".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference Q => new VariableReference(nameof(Q));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "R".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference R => new VariableReference(nameof(R));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "S".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference S => new VariableReference(nameof(S));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "T".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference T => new VariableReference(nameof(T));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "U".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference U => new VariableReference(nameof(U));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "V".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference V => new VariableReference(nameof(V));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "W".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference W => new VariableReference(nameof(W));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "X".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference X => new VariableReference(nameof(X));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "Y".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference Y => new VariableReference(nameof(Y));

        /// <summary>
        /// Gets a new <see cref="VariableReference"/> for a variable with the symbol "Z".
        /// </summary>
        /// <remarks>
        /// These properties are almost certainly a bad idea. They're handy in the example domains though, so I'm giving them the benefit of the doubt for now..
        /// </remarks>
        public static VariableReference Z => new VariableReference(nameof(Z));
    }
}
