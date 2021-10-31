using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.ExampleDomains.Kinship
{
    public interface IPerson
    {
        //// Unary predicates:
        bool IsMale { get; }
        bool IsFemale { get; }

        //// Binary predicates:
        bool IsParent(IPerson person);
        bool IsSibling(IPerson person);
        bool IsBrother(IPerson person);
        bool IsSister(IPerson person);
        bool IsChild(IPerson person);
        bool IsDaughter(IPerson person);
        bool IsSon(IPerson person);
        bool IsSpouse(IPerson person);
        bool IsWife(IPerson person);
        bool IsHusband(IPerson person);
        bool IsGrandparent(IPerson person);
        bool IsGrandchild(IPerson person);
        bool IsCousin(IPerson person);
        bool IsAunt(IPerson person);
        bool IsUncle(IPerson person);

        //// Unary functions:
        IPerson Mother { get; }
        IPerson Father { get; }
    }

    public static class KinshipKnowledge
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<IPerson>(); // ..or a different KB implementation - none implemented yet
        // kb.Tell(KinshipKnowledge.Axioms);
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of LinqToKB would be in allowing something like kb.Bind(domainAdapter), where domainAdpater is an IEnumerable<IPerson>.. 
        // kb.Ask(..my query..);
        public static IReadOnlyCollection<Expression<Predicate<IEnumerable<IPerson>>>> Axioms { get; } = new List<Expression<Predicate<IEnumerable<IPerson>>>>()
        {
            // One's mother is one's female parent:
            d => d.All((m, c) => Iff(c.Mother == m, m.IsFemale && m.IsParent(c))),

            // Ones' husband is one's male spouse:
            d => d.All((w, h) => Iff(h.IsHusband(w), h.IsMale && h.IsSpouse(w))),

            // Male and female are disjoint categories:
            d => d.All(x => Iff(x.IsMale, !x.IsFemale)),

            // Parent and child are inverse relations:
            d => d.All((p, c) => Iff(p.IsParent(c), c.IsChild(p))),

            // A grandparent is a parent of one's parent:
            d => d.All((g, c) => Iff(g.IsGrandparent(c), d.Any(p => g.IsParent(p) && p.IsParent(c)))),

            // A sibling is another child of one's parents:
            d => d.All((x, y) => Iff(x.IsSibling(y), x != y && d.Any(p => p.IsParent(x) && p.IsParent(y)))),

        }.AsReadOnly();


        // Theorems are derivable from axioms, but might be useful for performance...
        public static IReadOnlyCollection<Expression<Predicate<IEnumerable<IPerson>>>> Theorems { get; } = new List<Expression<Predicate<IEnumerable<IPerson>>>>()
        {
            // Siblinghood is commutative:
            d => d.All((x, y) => Iff(x.IsSibling(y), y.IsSibling(x))),

        }.AsReadOnly();
    }
}
