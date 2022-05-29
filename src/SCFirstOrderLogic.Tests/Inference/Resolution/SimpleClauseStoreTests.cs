using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceManipulation;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static class SimpleClauseStoreTests
    {
        public static Test Smoke => TestThat
            .Given(() =>
            {
                var store = new SimpleClauseStore();
                store.AddAsync(new CNFClause(new Predicate("A"))).Wait();
                store.AddAsync(new CNFClause(new Predicate("B"))).Wait();
                return store;
            })
            .When(store => store.ToArrayAsync().Result)
            .ThenReturns((_, a) => a.Should().BeEquivalentTo(new[] { new CNFClause(new Predicate("A")), new CNFClause(new Predicate("B")) }));

        public static Test Concurrency_AddDuringEnum => TestThat
            .Given(() =>
            {
                // we have a store that has had a couple of clauses added...
                var store = new SimpleClauseStore();
                store.AddAsync(new CNFClause(new Predicate("A"))).Wait();
                store.AddAsync(new CNFClause(new Predicate("B"))).Wait();

                // ..then has had an enumeration strarted but not completed..
                var enumerator = store.GetAsyncEnumerator();
                enumerator.MoveNextAsync().AsTask().Wait();

                // ..then has had another clause added.
                store.AddAsync(new CNFClause(new Predicate("C"))).Wait();

                return new { store, enumerator };
            })
            .When(g => g.enumerator.MoveNextAsync().AsTask().Wait())
            .ThenReturns();
    }
}
