using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingLanguageIntegration;

/// <summary>
/// <para>
/// The "curiousity and the cat" example from section 9.5 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// ILinqKnowledgeBase&lt;CuriousityAndTheCatDomain.IDomain, CuriousityAndTheCatDomain.IElement&gt; kb = .. // a LINQ knowledge base implementation
/// kb.Tell(CuriousityAndTheCatDomain.Axioms);
/// var answer = kb.Ask(d => d.Kills(d.Curiousity, d.Tuna)); // should return true
/// </code>
/// </summary>
public static class CuriousityAndTheCatDomain
{
    /// <summary>
    /// Gets the fundamental axioms of the domain.
    /// </summary>
    public static IReadOnlyCollection<Expression<Predicate<IDomain>>> Axioms { get; } = new List<Expression<Predicate<IDomain>>>()
    {
        // Everyone who loves all animals is loved by someone.
        // ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
        d => d.All(x => If(
            d.All(y => If(y.IsAnimal, x.Loves(y))),
            d.Any(y => y.Loves(x)))),

        // Anyone who kills an animal is loved by no one.
        // ∀x [∃z Animal(z) ∧ Kills(x, z)] ⇒ [∀y ¬Loves(y, x)]
        d => d.All(x => If(
            d.Any(z => z.IsAnimal && x.Kills(z)),
            d.All(y => !y.Loves(x)))),

        // Jack loves all animals.
        // ∀x Animal(x) ⇒ Loves(Jack, x)
        d => d.All(x => If(x.IsAnimal, d.Jack.Loves(x))),

        // Either Jack or Curiosity killed the cat, who is named Tuna.
        // Kills(Jack, Tuna) ∨ Kills(Curiosity, Tuna)
        // Cat(Tuna)
        d => d.Jack.Kills(d.Tuna) || d.Curiousity.Kills(d.Tuna),
        d => d.Tuna.IsCat,

        // Cats are animals.
        // ∀x Cat(x) ⇒ Animal(x)
        d => d.All(x => If(x.IsCat, x.IsAnimal))

    }.AsReadOnly();

    public interface IDomain : IEnumerable<IElement>
    {
        IElement Jack { get; }
        IElement Tuna { get; }
        IElement Curiousity { get; }
    }

    public interface IElement
    {
        bool IsAnimal { get; }
        bool IsCat { get; }
        bool Loves(IElement other);
        bool Kills(IElement other);
    }
}
