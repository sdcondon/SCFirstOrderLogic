using FluentAssertions;
using FlUnit;
using System;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public static partial class CNFConversionTests
    {
        private static Predicate A => new(nameof(A));
        private static Predicate B => new(nameof(B));
        private static Predicate C => new(nameof(C));

        // This behaviour would probably be nice, but we don't do it for now at least:
        ////public static Test NormaliseOrderOfOperationsInSentenceProperty => TestThat
        ////    .Given(() => new
        ////    {
        ////        Sentence1 = Or(Or(A, B), C),
        ////        Sentence2 = Or(A, Or(B, C))
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
