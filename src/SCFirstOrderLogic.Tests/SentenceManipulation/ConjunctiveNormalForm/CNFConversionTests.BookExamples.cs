using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public static partial class CNFConversionTests
    {
        private static Predicate IsAnimal(Term term) => new("IsAnimal", term);
        private static Predicate Loves(Term term1, Term term2) => new("Loves", term1, term2);
        private static VariableDeclaration X => new("x");
        private static VariableDeclaration Y => new("y");

        // NB: bad dependencies here on naming of skolem functions and standardised variables used by implementation.
        // Possible to get rid of by configuring equivalence options. A challenge for another day..
        private static SkolemFunction F(Term term) => new("Skolem1", term);
        private static SkolemFunction G(Term term) => new("Skolem2", term);
        private static VariableReference StdX => new("0:x");

        // Incidentally, its really annoying when the principal example given in textbooks is wrong..
        public static Test BookExample => TestThat
            // Given ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
            .Given(() => ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X)))))
            // When converted to CNF..
            .When(sentence => new CNFConversion().ApplyTo(sentence))
            // Then gives [Animal(F(x)) ∨ Loves(G(x), x)] ∧ [¬Loves(x, F(x)) ∨ Loves(G(x), x)]
            .ThenReturns((_, sentence) => sentence.Should().Be(And(
                Or(IsAnimal(F(StdX)), Loves(G(StdX), StdX)),
                Or(Not(Loves(StdX, F(StdX))), Loves(G(StdX), StdX)))));
    }
}
