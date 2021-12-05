using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
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

        private static Predicate IsAnimal(Term term)
        {
            return new MemberPredicate(
                typeof(IElement).GetProperty(nameof(IElement.IsAnimal)).GetMethod,
                new[] { term });
        }

        private static Predicate Loves(Term term1, Term term2)
        {
            return new MemberPredicate(
                typeof(IElement).GetMethod(nameof(IElement.Loves)),
                new[] { term1, term2 });
        }

        private static SkolemFunction F(Term term)
        {
            return new SkolemFunction("F", new[] { term });
        }

        private static SkolemFunction G(Term term)
        {
            return new SkolemFunction("G", new[] { term });
        }

        private static Variable X => new Variable("x");

        private static Variable Y => new Variable("y");

        private static Negation Not(Sentence sentence) => new Negation(sentence);

        public static Test BasicBehaviour => TestThat
            // Given ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
            .Given(() => Sentence.Create<IDomain, IElement>(d => d.All(x => If(d.All(y => If(y.IsAnimal, x.Loves(y))), d.Any(y => y.Loves(x))))))

            // When converted to CNF..
            .When(sentence => new CNFConversion().ApplyTo(sentence))

            // Then gives [Animal(F(x)) ∨ Loves(G(z), x)] ∧ [¬Loves(x, F(x)) ∨ Loves(G(z), x)] 
            .Then((_, sentence) => sentence.Should().BeEquivalentTo(new Conjunction(
                new Disjunction(IsAnimal(F(X)), Loves(X, Y)),
                new Disjunction(Not(Loves(X, F(X))), Loves(G(X), X)))), "CNF is as expected");
    }
}
