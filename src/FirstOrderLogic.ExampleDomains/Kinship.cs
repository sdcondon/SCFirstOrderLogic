using LinqToKB.FirstOrderLogic.KnowledgeBases;
using System.Linq;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.ExampleDomains.Kinship
{
    public static class Kinship
    {
        static Kinship()
        {
            KnowledgeBase = null; // TODO!

            //// AXIOMS:

            // One's mother is one's female parent:
            KnowledgeBase.Tell(d => d.All((m, c) => Iff(c.Mother == m, m.IsFemale && m.IsParent(c))));

            // Ones' husband is one's male spouse:
            KnowledgeBase.Tell(d => d.All((w, h) => Iff(h.IsHusband(w), h.IsMale && h.IsSpouse(w))));

            // Male and female are disjoint categories:
            KnowledgeBase.Tell(d => d.All(x => Iff(x.IsMale, !x.IsFemale)));

            // Parent and child are inverse relations:
            KnowledgeBase.Tell(d => d.All((p, c) => Iff(p.IsParent(c), c.IsChild(p))));

            // A grandparent is a parent of one's parent:
            KnowledgeBase.Tell(d => d.All((g, c) => Iff(g.IsGrandparent(c), d.Any(p => g.IsParent(p) && p.IsParent(c)))));

            // A sibling is another child of one's parents:
            KnowledgeBase.Tell(d => d.All((x, y) => Iff(x.IsSibling(y), x != y && d.Any(p => p.IsParent(x) && p.IsParent(y)))));

            //// THEOREMS:

            //KnowledgeBase.Tell(d => d.All((x, y) => Iff(x.IsSibling(y), y.IsSibling(x)));
        }

        public static IKnowledgeBase<IPerson> KnowledgeBase { get; }

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
    }
}
