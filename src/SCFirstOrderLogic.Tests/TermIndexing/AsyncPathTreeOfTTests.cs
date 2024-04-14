using FluentAssertions;
using FlUnit;
using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing;

public static class AsyncPathTreeOfTTests
{
    private static readonly Constant C1 = new("C1");
    private static readonly Constant C2 = new("C2");

    private static Function F(params Term[] a) => new(nameof(F), a);

    public static Test ContainsAsyncBehaviour => TestThat
        .GivenEachOf(() => new TryGetExactAsyncTestCase<int>[]
        {
            new(
                Contents: new()
                {
                    [F(C1, C1)] = 1,
                    [F(C2, C2)] = 1
                },
                QueryTerm: F(C1, C1),
                ExpectedReturnValue: true),

            new(
                Contents: new()
                {
                    [F(C1, C1)] = 1,
                    [F(C2, C2)] = 1
                },
                QueryTerm: F(C1, C2),
                ExpectedReturnValue: false),
        })
        .WhenAsync(async tc =>
        {
            var tree = new AsyncPathTree<int>(new AsyncPathTreeDictionaryNode<int>(), tc.Contents);
            return (await tree.TryGetExactAsync(tc.QueryTerm)).isSucceeded;
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    private record TryGetExactAsyncTestCase<T>(Dictionary<Term, T> Contents, Term QueryTerm, bool ExpectedReturnValue);
}
