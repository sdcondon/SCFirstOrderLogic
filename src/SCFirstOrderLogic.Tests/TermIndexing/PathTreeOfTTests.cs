using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing;

public static class PathTreeOfTTests
{
    private static readonly Constant C1 = new("C1");
    private static readonly Constant C2 = new("C2");

    private static Term F(params Term[] a) => new Function(nameof(F), a);

    public static Test ContainsBehaviour => TestThat
        .GivenEachOf(() => new TryGetExactTestCase<int>[]
        {
            new(
                Contents:
                [
                    KeyValuePair.Create(F(C1, C1), 1),
                    KeyValuePair.Create(F(C2, C2), 1)
                ],
                QueryTerm: F(C1, C1),
                ExpectedReturnValue: true),

            new(
                Contents:
                [
                    KeyValuePair.Create(F(C1, C1), 1),
                    KeyValuePair.Create(F(C2, C2), 1)
                ],
                QueryTerm: F(C2, C2),
                ExpectedReturnValue: true),

            new(
                Contents:
                [
                    KeyValuePair.Create(F(C1, C1), 1),
                    KeyValuePair.Create(F(C2, C2), 1)
                ],
                QueryTerm: F(C1, C2),
                ExpectedReturnValue: false),
        })
        .When(tc =>
        {
            var tree = new PathTree<int>(tc.Contents);
            return tree.TryGetExact(tc.QueryTerm, out _);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    private record TryGetExactTestCase<T>(KeyValuePair<Term, T>[] Contents, Term QueryTerm, bool ExpectedReturnValue);
}
