using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static SCFirstOrderLogic.LanguageIntegration.Operators;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.Linq.Kinship
{
    public interface IPerson
    {
        //// Unary predicates:
        bool IsMale { get; }
        bool IsFemale { get; }

        //// Binary predicates:
        bool IsParentOf(IPerson person);
        bool IsSiblingOf(IPerson person);
        bool IsBrotherOf(IPerson person);
        bool IsSisterOf(IPerson person);
        bool IsChildOf(IPerson person);
        bool IsDaughterOf(IPerson person);
        bool IsSonOf(IPerson person);
        bool IsSpouseOf(IPerson person);
        bool IsWifeOf(IPerson person);
        bool IsHusbandOf(IPerson person);
        bool IsGrandparentOf(IPerson person);
        bool IsGrandchildOf(IPerson person);
        bool IsCousinOf(IPerson person);
        bool IsAuntOf(IPerson person);
        bool IsUncleOf(IPerson person);

        //// Unary functions:
        IPerson Mother { get; }
        IPerson Father { get; }
    }

    /// <summary>
    /// Container for fundamental knowledge about the kinship domain.
    /// </summary>
    public static class KinshipKnowledge
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<IPerson>(); // ..or a different KB implementation - none implemented yet
        // kb.Tell(KinshipKnowledge.Axioms);
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of language integration would be in allowing something like kb.Bind(domainAdapter, opts),
        //    where domainAdapter is an IEnumerable<IPerson>, to specify known constants in a way that easily integrates with.. other things
        // kb.Ask(..my query..);
        public static IReadOnlyCollection<Expression<Predicate<IEnumerable<IPerson>>>> Axioms { get; } = new List<Expression<Predicate<IEnumerable<IPerson>>>>()
        {
            // One's mother is one's female parent:
            d => d.All((m, c) => Iff(c.Mother == m, m.IsFemale && m.IsParentOf(c))),

            // Ones' husband is one's male spouse:
            d => d.All((w, h) => Iff(h.IsHusbandOf(w), h.IsMale && h.IsSpouseOf(w))),

            // Male and female are disjoint categories:
            d => d.All(x => Iff(x.IsMale, !x.IsFemale)),

            // Parent and child are inverse relations:
            d => d.All((p, c) => Iff(p.IsParentOf(c), c.IsChildOf(p))),

            // A grandparent is a parent of one's parent:
            d => d.All((g, c) => Iff(g.IsGrandparentOf(c), d.Any(p => g.IsParentOf(p) && p.IsParentOf(c)))),

            // A sibling is another child of one's parents:
            d => d.All((x, y) => Iff(x.IsSiblingOf(y), x != y && d.Any(p => p.IsParentOf(x) && p.IsParentOf(y)))),

        }.AsReadOnly();

        // Theorems are derivable from axioms, but might be useful for performance...
        public static IReadOnlyCollection<Expression<Predicate<IEnumerable<IPerson>>>> Theorems { get; } = new List<Expression<Predicate<IEnumerable<IPerson>>>>()
        {
            // Siblinghood is commutative:
            d => d.All((x, y) => Iff(x.IsSiblingOf(y), y.IsSiblingOf(x))),

        }.AsReadOnly();
    }
}
