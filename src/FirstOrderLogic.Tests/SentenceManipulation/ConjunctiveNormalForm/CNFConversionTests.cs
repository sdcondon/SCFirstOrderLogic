using FluentAssertions;
using FlUnit;
using LinqToKB.FirstOrderLogic.LanguageIntegration;
using System.Collections.Generic;
using System.Linq;
using static LinqToKB.FirstOrderLogic.LanguageIntegration.Operators;

namespace LinqToKB.FirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public static class CNFConversionTests
    {
        private interface IDomain
            : IEnumerable<IElement>
        {
        }

        private interface IElement
        {
            bool IsAnimal { get; }

            bool Loves(IElement other);
        }

        private static Predicate IsAnimal(Term term) => new MemberPredicate(
            typeof(IElement).GetProperty(nameof(IElement.IsAnimal)),
            new[] { term });

        private static Predicate Loves(Term term1, Term term2) => new MemberPredicate(
            typeof(IElement).GetMethod(nameof(IElement.Loves)),
            new[] { term1, term2 });

        // NB: bad dependencies on naming of skolem functions and standadised variables used by implementation here.
        // Possible to get rid of by configuring equivalence options. A challenge for another day..
        private static SkolemFunction F(Term term) => new SkolemFunction("Skolem1", term);

        private static SkolemFunction G(Term term) => new SkolemFunction("Skolem2", term);

        private static Variable X => new Variable("0:x");

        private static Variable Y1 => new Variable("1:y");

        private static Variable Y2 => new Variable("2:y");

        private static Negation Not(Sentence sentence) => new Negation(sentence);

        // Incidentally, its really annoying when the principal example given in textbooks is wrong..
        public static Test BasicBehaviour => TestThat
            // Given ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
            .Given(() => SentenceFactory.Create<IDomain, IElement>(d => d.All(x => If(d.All(y => If(y.IsAnimal, x.Loves(y))), d.Any(y => y.Loves(x))))))

            // When converted to CNF..
            .When(sentence => new CNFConversion().ApplyTo(sentence))

            // Then gives [Animal(F(x)) ∨ Loves(G(x), x)] ∧ [¬Loves(x, F(x)) ∨ Loves(G(x), x)]
            .ThenReturns((_, sentence) => sentence.Should().BeEquivalentTo(new Conjunction(
                new Disjunction(IsAnimal(F(X)), Loves(G(X), X)),
                new Disjunction(Not(Loves(X, F(X))), Loves(G(X), X)))));
    }
}
