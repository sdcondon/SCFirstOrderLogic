using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a sentence of first order logic.
    /// </summary>
    public abstract class Sentence
    {
        // TODO.. proper visitor pattern probably useful for transformations and others..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);

        //// Shorthand factory methods. Not sure if I like these here or not. Separating them into their own class would make it easier to include guidance
        //// (notably, how best to create your own properties for variables and constants and methods for functions and predicates).
        public static Sentence ForAll(VariableDeclaration variableDeclaration, Sentence sentence) => new UniversalQuantification(variableDeclaration, sentence);
        public static Sentence ThereExists(VariableDeclaration variableDeclaration, Sentence sentence) => new ExistentialQuantification(variableDeclaration, sentence);
        public static Sentence And(Sentence left, Sentence right) => new Conjunction(left, right);
        public static Sentence Or(Sentence left, Sentence right) => new Disjunction(left, right);
        public static Sentence If(Sentence antecedent, Sentence consequent) => new Implication(antecedent, consequent);
        public static Sentence Iff(Sentence left, Sentence right) => new Equivalence(left, right);
        public static Sentence Not(Sentence sentence) => new Negation(sentence);
        public static Sentence Equals(Term left, Term right) => new Equality(left, right);

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // Just for now..
    }
}
