using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingOperableSentenceFactory;

/// <summary>
/// <para>
/// The "crime" example from section 9.3 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
/// ..
/// IKnowledgeBase kb = .. // a knowledge base implementation
/// kb.Tell(Axioms);
/// var answer = kb.Ask(IsCriminal(West)); // should return true
/// </code>
/// </summary>
public static class CrimeDomain
{
    static CrimeDomain()
    {
        Axioms = new List<Sentence>()
        {
            // "... it is a crime for an American to sell weapons to hostile nations":
            // American(x) ∧ Weapon(y) ∧ Sells(x, y, z) ∧ Hostile(z) ⇒ Criminal(x).
            ForAll(X, Y, Z, If(IsAmerican(X) & IsWeapon(Y) & Sells(X, Y, Z) & IsHostile(Z), IsCriminal(X))),

            // "NoNo... has some missiles."
            // ∃x IsMissile(x) ∧ Owns(NoNo, x)
            ThereExists(X, IsMissile(X) & Owns(NoNo, X)),

            // "All of its missiles were sold to it by Colonel West":
            // Missile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, NoNo)
            ForAll(X, If(IsMissile(X) & Owns(NoNo, X), Sells(ColonelWest, X, NoNo))),

            // We will also need to know that missiles are weapons: 
            ForAll(X, If(IsMissile(X), IsWeapon(X))),

            // And we must know that an enemy of America counts as “hostile”:
            // Enemy(x, America) ⇒ Hostile(x)
            ForAll(X, If(IsEnemyOf(X, America), IsHostile(X))),

            // "West, who is American..": American(West)
            IsAmerican(ColonelWest),

            // "The country Nono, an enemy of America..": Enemy(NoNo, America).
            IsEnemyOf(NoNo, America),

        }.AsReadOnly();
    }

    /// <summary>
    /// Gets the fundamental axioms of the crime domain.
    /// </summary>
    public static IReadOnlyCollection<Sentence> Axioms { get; }

    public static OperableConstant America { get; } = new Constant(nameof(America));
    public static OperableConstant NoNo { get; } = new Constant(nameof(NoNo));
    public static OperableConstant ColonelWest { get; } = new Constant(nameof(ColonelWest));

    public static OperablePredicate IsAmerican(OperableTerm t) => new Predicate(nameof(IsAmerican), t);
    public static OperablePredicate IsHostile(OperableTerm t) => new Predicate(nameof(IsHostile), t);
    public static OperablePredicate IsCriminal(OperableTerm t) => new Predicate(nameof(IsCriminal), t);
    public static OperablePredicate IsWeapon(OperableTerm t) => new Predicate(nameof(IsWeapon), t);
    public static OperablePredicate IsMissile(OperableTerm t) => new Predicate(nameof(IsMissile), t);
    public static OperablePredicate Owns(OperableTerm owner, OperableTerm owned) => new Predicate(nameof(Owns), owner, owned);
    public static OperablePredicate Sells(OperableTerm seller, OperableTerm item, OperableTerm buyer) => new Predicate(nameof(Sells), seller, item, buyer);
    public static OperablePredicate IsEnemyOf(OperableTerm t, OperableTerm other) => new Predicate(nameof(IsEnemyOf), t, other);
}
