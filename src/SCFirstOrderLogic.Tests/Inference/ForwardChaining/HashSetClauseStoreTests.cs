using FluentAssertions;
using FlUnit;
using System.Linq;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    public static class HashSetClauseStoreTests
    {
        public static Test Smoke => TestThat
            .Given(() =>
            {
                var store = new HashSetClauseStore();
                store.AddAsync(new CNFDefiniteClause(new Predicate("A"))).Wait();
                store.AddAsync(new CNFDefiniteClause(new Predicate("B"))).Wait();
                return store;
            })
            .When(store => store.ToArrayAsync().GetAwaiter().GetResult())
            .ThenReturns((_, a) => a.Should().BeEquivalentTo(new[] { new CNFClause(new Predicate("A")), new CNFClause(new Predicate("B")) }));

        public static Test Concurrency_AddDuringEnum => TestThat
            .Given(() =>
            {
                // we have a store that has had a couple of clauses added...
                var store = new HashSetClauseStore();
                store.AddAsync(new CNFDefiniteClause(new Predicate("A"))).Wait();
                store.AddAsync(new CNFDefiniteClause(new Predicate("B"))).Wait();

                // ..then has had an enumeration started but not completed..
                var enumerator = store.GetAsyncEnumerator();
                enumerator.MoveNextAsync().AsTask().Wait();

                // ..then has had another clause added.
                store.AddAsync(new CNFDefiniteClause(new Predicate("C"))).Wait();

                return new { store, enumerator };
            })
            .When(g => g.enumerator.MoveNextAsync().AsTask().Wait())
            .ThenReturns();
    }
}
