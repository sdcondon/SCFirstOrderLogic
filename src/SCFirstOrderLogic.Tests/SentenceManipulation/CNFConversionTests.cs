using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation
{
    public static partial class CNFConversionTests
    {
        private static Predicate A => new(nameof(A));
        private static Predicate B => new(nameof(B));
        private static Predicate C => new(nameof(C));

        private static Predicate D(Term d) => new(nameof(D), d);
        private static Predicate E(Term e) => new(nameof(E), e);

        public static Test VariablesStandardisedAcrossSentences => TestThat
            .Given(() => new
            {
                // NB these normalise to just D(X) and E(X) respectively
                CNFSentence1 = CNFConversion.ApplyTo(ForAll(X, D(X))),
                CNFSentence2 = CNFConversion.ApplyTo(ForAll(X, E(X)))
            })
            .When(g => ((Predicate)g.CNFSentence1).Arguments[0].Equals(((Predicate)g.CNFSentence2).Arguments[0]))
            .ThenReturns((_, retVal) => retVal.Should().BeFalse("standardised variables from different sentences shouldn't be equal, even if the underlying symbol is the same"));

        // These behaviours might be nice, but we don't do them for now at least:
        ////public static Test NormalisationOfEquivalentSentences => TestThat
        ////    .GivenEachOf(() => new[]
        ////    {
        ////        new
        ////        {
        ////            // Order of operation for disjunctions doesn't matter
        ////            Sentence1 = Or(Or(A, B), C),
        ////            Sentence2 = Or(A, Or(B, C))
        ////        },
        ////        new
        ////        {
        ////            // More difficult, but MAYBE still useful - variable naming doesn't matter
        ////            Sentence1 = ForAll(X, D(X)),
        ////            Sentence2 = ForAll(Y, D(Y))
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
}
