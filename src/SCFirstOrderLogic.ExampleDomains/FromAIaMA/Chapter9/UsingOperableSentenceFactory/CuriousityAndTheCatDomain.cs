using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory;

/// <summary>
/// <para>
/// The "curiousity and the cat" example from section 9.5 of Artificial Intelligence: A Modern Approach,
/// Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CuriousityAndTheCatDomain;
/// ..
/// IKnowledgeBase kb = .. // a knowledge base implementation
/// kb.Tell(Axioms);
/// var answer = kb.Ask(Kills(Curiousity, Tuna)); // should return true
/// </code>
/// </summary>
public static class CuriousityAndTheCatDomain
{
    static CuriousityAndTheCatDomain()
    {
        Axioms = new List<Sentence>()
        {
            // Everyone who loves all animals is loved by someone.
            // ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
            ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X)))),

            // Anyone who kills an animal is loved by no one.
            // ∀x [∃z Animal(z) ∧ Kills(x, z)] ⇒ [∀y ¬Loves(y, x)]
            ForAll(X, If(
                ThereExists(Z, IsAnimal(Z) & Kills(X, Z)),
                ForAll(Y, !Loves(Y, X)))),

            // Jack loves all animals.
            // ∀x Animal(x) ⇒ Loves(Jack, x)
            ForAll(X, If(IsAnimal(X), Loves(Jack, X))),

            // Either Jack or Curiosity killed the cat, who is named Tuna.
            // Kills(Jack, Tuna) ∨ Kills(Curiosity, Tuna)
            // Cat(Tuna)
            Kills(Jack, Tuna) | Kills(Curiousity, Tuna),
            IsCat(Tuna),

            // Cats are animals.
            // ∀x Cat(x) ⇒ Animal(x)
            ForAll(X, If(IsCat(X), IsAnimal(X))),

        }.AsReadOnly();
    }

    /// <summary>
    /// Gets the fundamental axioms of the domain.
    /// </summary>
    public static IReadOnlyCollection<Sentence> Axioms { get; }

    public static OperableConstant Jack { get; } = new Constant(nameof(Jack));
    public static OperableConstant Tuna { get; } = new Constant(nameof(Tuna));
    public static OperableConstant Curiousity { get; } = new Constant(nameof(Curiousity));

    public static OperablePredicate IsAnimal(OperableTerm subject) => new Predicate(nameof(IsAnimal), subject);
    public static OperablePredicate IsCat(OperableTerm subject) => new Predicate(nameof(IsCat), subject);
    public static OperablePredicate Loves(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(Loves), subject, @object);
    public static OperablePredicate Kills(OperableTerm subject, OperableTerm @object) => new Predicate(nameof(Kills), subject, @object);
}
