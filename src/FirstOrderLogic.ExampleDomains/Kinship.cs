using System.Linq;
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

    public static class KnowledgeBaseExtensions
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<IPerson>(); // ..or a different KB implementation - none implemented yet
        // kb.AddKinshipAxioms();
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of LinqToKB would be in allowing something like kb.Bind(myDomainAdapter); for runtime "constants"
        // kb.Ask(..my query..);
        //
        // Would this be better as a public read-only axioms collection and an IKnowledgeBase extension to tell multiple facts at once?
        // kb.Tell(KinshipKnowledge.Axioms);
        // ..could also gracefully provide theorems then, and also allows for axiom examination with a KB instance..
        public static void AddKinshipAxioms(this IKnowledgeBase<IPerson> knowledgeBase)
        {
            //// AXIOMS:

            // One's mother is one's female parent:
            knowledgeBase.Tell(d => d.All((m, c) => Iff(c.Mother == m, m.IsFemale && m.IsParent(c))));

            // Ones' husband is one's male spouse:
            knowledgeBase.Tell(d => d.All((w, h) => Iff(h.IsHusband(w), h.IsMale && h.IsSpouse(w))));

            // Male and female are disjoint categories:
            knowledgeBase.Tell(d => d.All(x => Iff(x.IsMale, !x.IsFemale)));

            // Parent and child are inverse relations:
            knowledgeBase.Tell(d => d.All((p, c) => Iff(p.IsParent(c), c.IsChild(p))));

            // A grandparent is a parent of one's parent:
            knowledgeBase.Tell(d => d.All((g, c) => Iff(g.IsGrandparent(c), d.Any(p => g.IsParent(p) && p.IsParent(c)))));

            // A sibling is another child of one's parents:
            knowledgeBase.Tell(d => d.All((x, y) => Iff(x.IsSibling(y), x != y && d.Any(p => p.IsParent(x) && p.IsParent(y)))));

            //// THEOREMS:

            //KnowledgeBase.Tell(d => d.All((x, y) => Iff(x.IsSibling(y), y.IsSibling(x)));
        }
    }
}
