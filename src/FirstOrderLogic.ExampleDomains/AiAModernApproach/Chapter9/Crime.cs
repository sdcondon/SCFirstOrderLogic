using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.Crime
{
    //// The Crime example from section 9.3 of 
    //// Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.

    public interface IDomain : IEnumerable<IElement>
    {
        IElement America { get; }
        IElement Nono { get; }
        IElement M1 { get; }
        IElement West { get; }
    }

    public interface IElement
    {
        bool IsAmerican { get; }
        bool IsHostile { get; }
        bool IsCriminal { get; }
        bool IsWeapon { get; }
        bool IsMissile { get; }
        bool Owns(IElement item);
        bool Sells(IElement item, IElement buyer);
        bool IsEnemyOf(IElement other);
    }

    /// <summary>
    /// Container for fundamental knowledge about the kinship domain.
    /// </summary>
    public static class CrimeKnowledge
    {
        public static IReadOnlyCollection<Expression<Predicate<IDomain>>> Axioms { get; } = new List<Expression<Predicate<IDomain>>>()
        {
            // "... it is a crime for an American to sell weapons to hostile nations":
            // American(x) ∧ Weapon(y) ∧ Sells(x, y, z) ∧ Hostile(z) ⇒ Criminal(x).
            d => d.All((seller, item, buyer) => If(seller.IsAmerican && item.IsWeapon && seller.Sells(item, buyer) && buyer.IsHostile, seller.IsCriminal)),

            // "Nono... has some missiles."
            // (NB: could also be specified as d => d.Any(m => m.IsMissile && d.Nono.Owns(m)), but the book does Existential Instantiation for us, so we do the same here..)
            d => d.Nono.Owns(d.M1),
            d => d.M1.IsMissile,

            // "All of its missiles were sold to it by Colonel West":
            // Missile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)
            d => d.All(x => If(x.IsMissile && d.Nono.Owns(x), d.West.Sells(x, d.Nono))),

            // We will also need to know that missiles are weapons: 
            d => d.All(x => If(x.IsMissile, x.IsWeapon)),

            // And we must know that an enemy of America counts as “hostile”:
            // Enemy(x, America) ⇒ Hostile(x)
            d => d.All(x => If(x.IsEnemyOf(d.America), x.IsHostile)),

            // "West, who is American..": American(West)
            d => d.West.IsAmerican,

            // "The country Nono, an enemy of America..": Enemy(Nono, America).
            d => d.Nono.IsEnemyOf(d.America)

        }.AsReadOnly();
    }
}
