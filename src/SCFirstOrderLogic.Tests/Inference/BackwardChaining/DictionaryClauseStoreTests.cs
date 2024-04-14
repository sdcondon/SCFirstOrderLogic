using FluentAssertions;
using FlUnit;
using System.Linq;

namespace SCFirstOrderLogic.Inference.BackwardChaining;

public static class DictionaryClauseStoreTests
{
    public static Test Smoke => TestThat
        .GivenAsync(async () =>
        {
            var store = new DictionaryClauseStore();
            await store.AddAsync(new CNFDefiniteClause(new Predicate("A")));
            await store.AddAsync(new CNFDefiniteClause(new Predicate("B")));
            return store;
        })
        .WhenAsync(async store => await store.ToArrayAsync())
        .ThenReturns((_, a) => a.Should().BeEquivalentTo(new[] { new CNFClause(new Predicate("A")), new CNFClause(new Predicate("B")) }));

    public static Test Concurrency_AddDuringEnum => TestThat
        .GivenAsync(async () =>
        {
            // we have a store that has had a couple of clauses added...
            var store = new DictionaryClauseStore();
            await store.AddAsync(new CNFDefiniteClause(new Predicate("A")));
            await store.AddAsync(new CNFDefiniteClause(new Predicate("B")));

            // ..then has had an enumeration started but not completed..
            var enumerator = store.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();

            // ..then has had another clause added.
            await store.AddAsync(new CNFDefiniteClause(new Predicate("C")));

            return new { store, enumerator };
        })
        .WhenAsync(async g => await g.enumerator.MoveNextAsync())
        .ThenReturns();
}
