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

        private static Predicate<IDomain, IElement> IsAnimal(Term<IDomain, IElement> term)
        {
            return new Predicate<IDomain, IElement>(
                typeof(IElement).GetProperty(nameof(IElement.IsAnimal)).GetMethod,
                new[] { term });
        }

        private static Predicate<IDomain, IElement> Loves(Term<IDomain, IElement> term1, Term<IDomain, IElement> term2)
        {
            return new Predicate<IDomain, IElement>(
                typeof(IElement).GetMethod(nameof(IElement.Loves)),
                new[] { term1, term2 });
        }

        private static SkolemFunction<IDomain, IElement> F(Term<IDomain, IElement> term)
        {
            return new SkolemFunction<IDomain, IElement>("F", new[] { term });
        }

        private static SkolemFunction<IDomain, IElement> G(Term<IDomain, IElement> term)
        {
            return new SkolemFunction<IDomain, IElement>("G", new[] { term });
        }

        private static Variable<IDomain, IElement> X => new Variable<IDomain, IElement>("x");

        private static Variable<IDomain, IElement> Y => new Variable<IDomain, IElement>("y");

        private static Negation<IDomain, IElement> Not(Sentence<IDomain, IElement> sentence) => new Negation<IDomain, IElement>(sentence);

        public static Test BasicBehaviour => TestThat
            // Given ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
            .Given(() => Sentence.Create<IDomain, IElement>(d => d.All(x => If(d.All(y => If(y.IsAnimal, x.Loves(y))), d.Any(y => y.Loves(x))))))

            // When converted to CNF..
            .When(sentence => new CNFConversion<IDomain, IElement>().ApplyTo(sentence))

            // Then gives [Animal(F(x)) ∨ Loves(G(z), x)] ∧ [¬Loves(x, F(x)) ∨ Loves(G(z), x)] 
            .Then((_, sentence) => sentence.Should().BeEquivalentTo(new Conjunction<IDomain, IElement>(
                new Disjunction<IDomain, IElement>(IsAnimal(F(X)), Loves(X, Y)),
                new Disjunction<IDomain, IElement>(Not(Loves(X, F(X))), Loves(G(X), X)))), "CNF is as expected");
    }
}
