using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.TestProblems.GenericDomainSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.Normalisation;

public static partial class CNFConversionTests
{
    public static Test VariablesStandardisedAcrossSentences => TestThat
        .Given(() => new
        {
            // NB these normalise to just P(X) and Q(X) respectively
            CNFSentence1 = CNFConversion.ApplyTo(ForAll(X, P(X))),
            CNFSentence2 = CNFConversion.ApplyTo(ForAll(X, Q(X)))
        })
        .When(g => ((Predicate)g.CNFSentence1).Arguments[0].Equals(((Predicate)g.CNFSentence2).Arguments[0]))
        .ThenReturns((_, retVal) => retVal.Should().BeFalse("standardised variables from different sentences shouldn't be equal, even if the underlying identifier is the same"));

    // These behaviours might be nice, but we don't do them for now at least:
    ////public static Test NormalisationOfEquivalentSentences => TestThat
    ////    .GivenEachOf(() => new[]
    ////    {
    ////        new
    ////        {
    ////            // Order of operation for disjunctions doesn't matter
    ////            Sentence1 = Or(Or(P(), Q()), R()),
    ////            Sentence2 = Or(P(), Or(Q(), R()))
    ////        },
    ////        new
    ////        {
    ////            // More difficult, but MAYBE still useful - variable naming doesn't matter
    ////            Sentence1 = ForAll(X, P(X)),
    ////            Sentence2 = ForAll(Y, P(Y))
    ////        }
    ////    })
    ////    .When(g => new
    ////    {
    ////        CNFSentence1 = new CNFConversion().ApplyTo(g.Sentence1),
    ////        CNFSentence2 = new CNFConversion().ApplyTo(g.Sentence2)
    ////    })
    ////    .ThenReturns()
    ////    .And((_, retVal) => retVal.CNFSentence1.Should().Be(retVal.CNFSentence2));
}
