using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter8.UsingSentenceFactory;

/// <summary>
/// <para>
/// The natural numbers example domain from chapter 8 of Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.
/// </para>
/// <para>
/// Example usage:
/// </para>
/// <code>
/// IKnowledgeBase kb = .. // a knowledge base implementation
/// kb.Tell(NaturalNumbersDomain.Axioms);
/// kb.Tell(..facts about the specific problem..);
/// var answer = kb.Ask(..my query..);
/// </code>
/// </summary>
public static class NaturalNumbersDomain
{
    static NaturalNumbersDomain()
    {
        Axioms = new List<Sentence>()
        {
            ForAll(X, Not(AreEqual(Successor(X), Zero))),

            ForAll(X, Y, If(Not(AreEqual(X, Y)), Not(AreEqual(Successor(X), Successor(Y))))),

            ForAll(X, AreEqual(Add(Zero, X), X)),

            ForAll(X, Y, AreEqual(Add(Successor(X), Y), Add(Successor(Y), X))),

        }.AsReadOnly();
    }

    /// <summary>
    /// Gets the fundamental axioms of the natural numbers domain.
    /// </summary>
    public static IReadOnlyCollection<Sentence> Axioms { get; }

    public static Constant Zero { get; } = new Constant(nameof(Zero));

    public static Function Successor(Term t) => new Function(nameof(Successor), t);

    public static Function Add(Term t, Term other) => new Function(nameof(Add), t, other);
}
