using LinqToKB.FirstOrderLogic.KnowledgeBases;
using System.Collections.Generic;
using System.Linq;
using static LinqToKB.FirstOrderLogic.Operators;

namespace LinqToKB.FirstOrderLogic.ExampleDomains
{
    public static class NaturalNumbers
    {
        static NaturalNumbers()
        {
            KnowledgeBase = null; // TODO!

            KnowledgeBase.Tell(d => d.All(x => x.Successor != d.Zero));
            KnowledgeBase.Tell(d => d.All((x, y) => If(x != y, x.Successor != y.Successor)));
            KnowledgeBase.Tell(d => d.All(x => d.Zero.Add(x) == x));
            KnowledgeBase.Tell(d => d.All((x, y) => x.Successor.Add(y) == x.Add(y).Successor));
        }

        public static IKnowledgeBase<INaturalNumbers, INaturalNumber> KnowledgeBase { get; }

        public interface INaturalNumbers : IEnumerable<INaturalNumber>
        {
            INaturalNumber Zero { get; }
        }

        public interface INaturalNumber
        {
            INaturalNumber Successor { get; }

            INaturalNumber Add(INaturalNumber x);
        }
    }
}
