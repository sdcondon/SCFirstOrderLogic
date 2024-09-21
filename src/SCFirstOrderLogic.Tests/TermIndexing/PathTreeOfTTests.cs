using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.TermIndexing;

public static class PathTreeOfTTests
{
    public static Test ContainsBehaviour => TestThat
        .GivenEachOf<TryGetExactTestCase<int>>(() =>
        [
            new(
                Contents:
                [
                    KeyValuePair.Create<Term, int>(F(C, C), 1),
                    KeyValuePair.Create<Term, int>(F(D, D), 1)
                ],
                QueryTerm: F(C, C),
                ExpectedReturnValue: true),

            new(
                Contents:
                [
                    KeyValuePair.Create<Term, int>(F(C, C), 1),
                    KeyValuePair.Create<Term, int>(F(D, D), 1)
                ],
                QueryTerm: F(D, D),
                ExpectedReturnValue: true),

            new(
                Contents:
                [
                    KeyValuePair.Create<Term, int>(F(C, C), 1),
                    KeyValuePair.Create<Term, int>(F(D, D), 1)
                ],
                QueryTerm: F(C, D),
                ExpectedReturnValue: false),
        ])
        .When(tc =>
        {
            var tree = new PathTree<int>(tc.Contents);
            return tree.TryGetExact(tc.QueryTerm, out _);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    private record TryGetExactTestCase<T>(KeyValuePair<Term, T>[] Contents, Term QueryTerm, bool ExpectedReturnValue);
}
