using FluentAssertions;
using FlUnit;
using static LinqToKB.FirstOrderLogic.Sentence;

namespace LinqToKB.FirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public static class CNFConversionTests
    {
        private static Predicate IsAnimal(Term term) => new Predicate("IsAnimal", term);
        private static Predicate Loves(Term term1, Term term2) => new Predicate("Loves", term1, term2);
        private static Variable X => new Variable("x");
        private static Variable Y => new Variable("y");

        // NB: bad dependencies here on naming of skolem functions and standardised variables used by implementation.
        // Possible to get rid of by configuring equivalence options. A challenge for another day..
        private static SkolemFunction F(Term term) => new SkolemFunction("Skolem1", term);
        private static SkolemFunction G(Term term) => new SkolemFunction("Skolem2", term);
        private static Variable StdX => new Variable("0:x");

        // Incidentally, its really annoying when the principal example given in textbooks is wrong..
        public static Test BasicBehaviour => TestThat
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
