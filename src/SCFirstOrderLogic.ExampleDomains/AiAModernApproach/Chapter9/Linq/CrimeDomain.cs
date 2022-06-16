﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.Linq
{
    /// <summary>
    /// The "crime" example from section 9.3 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
    /// <para/>
    /// Example usage:
    /// <code>
    /// ILinqKnowledgeBase&lt;CrimeDomain.IDomain, CrimeDomain.IElement&gt; kb = .. // a LINQ knowledge base implementation
    /// kb.Tell(CrimeDomain.Axioms);
    /// var answer = kb.Ask(d => d.IsCriminal(d.West)); // should return true
    /// </code>
    /// </summary>
    public static class CrimeDomain
    {
        public static IReadOnlyCollection<Expression<Predicate<IDomain>>> Axioms { get; } = new List<Expression<Predicate<IDomain>>>()
        {
            // "... it is a crime for an American to sell weapons to hostile nations":
            // ∀x, y, z IsAmerican(x) ∧ IsWeapon(y) ∧ Sells(x, y, z) ∧ IsHostile(z) ⇒ IsCriminal(x).
            d => d.All((seller, item, buyer) => If(seller.IsAmerican && item.IsWeapon && seller.Sells(item, buyer) && buyer.IsHostile, seller.IsCriminal)),

            // "Nono... has some missiles."
            // ∃x IsMissile(x) ∧ Owns(Nono, x)
            d => d.Any(m => m.IsMissile && d.Nono.Owns(m)),

            // "All of its missiles were sold to it by Colonel West":
            // ∀x IsMissile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)
            d => d.All(x => If(x.IsMissile && d.Nono.Owns(x), d.West.Sells(x, d.Nono))),

            // We will also need to know that missiles are weapons: 
            // ∀x IsMissile(x) ⇒ IsWeapon(x)
            d => d.All(x => If(x.IsMissile, x.IsWeapon)),

            // And we must know that an enemy of America counts as “hostile”:
            // ∀x IsEnemyOf(x, America) ⇒ IsHostile(x)
            d => d.All(x => If(x.IsEnemyOf(d.America), x.IsHostile)),

            // "West, who is American.."
            // IsAmerican(West)
            d => d.West.IsAmerican,

            // "The country Nono, an enemy of America.."
            // IsEnemyOf(Nono, America).
            d => d.Nono.IsEnemyOf(d.America)

        }.AsReadOnly();

        public interface IDomain : IEnumerable<IElement>
        {
            IElement America { get; }
            IElement Nono { get; }
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
    }
}
