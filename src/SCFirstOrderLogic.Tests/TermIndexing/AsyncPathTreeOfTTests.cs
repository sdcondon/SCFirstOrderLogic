using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.Specialised.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.TermIndexing;

public static class AsyncPathTreeOfTTests
{
    public static Test ContainsAsyncBehaviour => TestThat
        .GivenEachOf<TryGetExactAsyncTestCase<int>>(() =>
        [
            new(
                Contents: new()
                {
                    [F(C, C)] = 1,
                    [F(D, D)] = 1
                },
                QueryTerm: F(C, C),
                ExpectedReturnValue: true),

            new(
                Contents: new()
                {
                    [F(C, C)] = 1,
                    [F(D, D)] = 1
                },
                QueryTerm: F(C, D),
                ExpectedReturnValue: false),
        ])
        .WhenAsync(async tc =>
        {
            var tree = new AsyncPathTree<int>(new AsyncPathTreeDictionaryNode<int>(), tc.Contents);
            return (await tree.TryGetExactAsync(tc.QueryTerm)).isSucceeded;
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    private record TryGetExactAsyncTestCase<T>(Dictionary<Term, T> Contents, Term QueryTerm, bool ExpectedReturnValue);
}
