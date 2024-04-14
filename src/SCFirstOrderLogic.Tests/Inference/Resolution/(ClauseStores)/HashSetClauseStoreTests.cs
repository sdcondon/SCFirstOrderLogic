using FluentAssertions;
using FlUnit;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Resolution;

public static class HashSetClauseStoreTests
{
    public static Test Smoke => TestThat
        .GivenAsync(async () =>
        {
            var store = new HashSetClauseStore();
            await store.AddAsync(new CNFClause(new Predicate("A")));
            await store.AddAsync(new CNFClause(new Predicate("B")));
            return store;
        })
        .WhenAsync(async store => await store.ToArrayAsync())
        .ThenReturns((_, a) => a.Should().BeEquivalentTo(new[] { new CNFClause(new Predicate("A")), new CNFClause(new Predicate("B")) }));

    public static Test Concurrency_AddDuringEnum => TestThat
        .GivenAsync(async () =>
        {
            // we have a store that has had a couple of clauses added...
            var store = new HashSetClauseStore();
            await store.AddAsync(new CNFClause(new Predicate("A")));
            await store.AddAsync(new CNFClause(new Predicate("B")));

            // ..then has had an enumeration started but not completed..
            var enumerator = store.GetAsyncEnumerator();
            await enumerator.MoveNextAsync();

            // ..then has had another clause added.
            await store.AddAsync(new CNFClause(new Predicate("C")));

            return new { store, enumerator };
        })
        .WhenAsync(async g => await g.enumerator.MoveNextAsync())
        .ThenReturns();
}
