using System.Collections.Generic;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8
{
    public static class NaturalNumbersDomain
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase(); // ..or a different KB implementation - none implemented yet
        // kb.Tell(NaturalNumberKnowledge.Axioms);
        // kb.Tell(..facts about the specific problem..);
        // kb.Ask(..my query..);
        public static IReadOnlyCollection<Sentence> Axioms { get; } = new List<Sentence>()
        {
            ForAll(X, Not(AreEqual(Successor(X), Zero))),
            ForAll(X, Y, If(Not(AreEqual(X, Y)), Not(AreEqual(Successor(X), Successor(Y))))),
            ForAll(X, AreEqual(Add(Zero, X), X)),
            ForAll(X, Y, AreEqual(Add(Successor(X), Y), Add(Successor(Y), X))),

        }.AsReadOnly();

        public static Constant Zero { get; } = new Constant(nameof(Zero));

        public static Function Successor(Term t) => new Function(nameof(Successor), t);

        public static Function Add(Term t, Term other) => new Function(nameof(Add), t, other);
    }
}
