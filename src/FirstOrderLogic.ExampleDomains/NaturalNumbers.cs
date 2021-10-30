using System.Collections.Generic;
using System.Linq;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.ExampleDomains.NaturalNumbers
{
    public interface INaturalNumbers : IEnumerable<INaturalNumber>
    {
        INaturalNumber Zero { get; }
    }

    public interface INaturalNumber
    {
        INaturalNumber Successor { get; }

        INaturalNumber Add(INaturalNumber x);
    }

    public static class KnowledgeBaseExtensions
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase<INaturalNumbers, INaturalNumber>(); // ..or a different KB implementation - none implemented yet
        // kb.AddNaturalNumberAxioms();
        // kb.Tell(..facts about the specific problem..);
        // .. though the real value of LinqToKB would be in allowing something like kb.Bind(myDomainAdapter); for runtime "constants"
        // kb.Ask(..my query..);
        //
        // Would this be better as a public read-only axioms collection and an IKnowledgeBase extension to tell multiple facts at once?
        // i.e. kb.Tell(NaturalNumberKnowledge.Axioms); ..could also gracefully provide theorems then, and also allows for axiom
        // examination without a KB instance..

        public static void AddNaturalNumberAxioms(this IKnowledgeBase<INaturalNumbers, INaturalNumber> knowledgeBase)
        {
            knowledgeBase.Tell(d => d.All(x => x.Successor != d.Zero));
            knowledgeBase.Tell(d => d.All((x, y) => If(x != y, x.Successor != y.Successor)));
            knowledgeBase.Tell(d => d.All(x => d.Zero.Add(x) == x));
            knowledgeBase.Tell(d => d.All((x, y) => x.Successor.Add(y) == x.Add(y).Successor));
        }
    }
}
