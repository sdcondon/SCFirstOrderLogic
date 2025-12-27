using SCFirstOrderLogic.Documentation.Types;
using SCFirstOrderLogic.FormulaManipulation.Normalisation;
using SCFirstOrderLogic.FormulaManipulation.Substitution;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SCFirstOrderLogic.Inference.Basic.ForwardChaining;

/// <summary>
/// Implementation of <see cref="IClauseStore"/> geared towards use in Blazor WASM, before multithreading support is added.
/// Hacky - adds in a bunch of Task.Delay(1)'s.
/// https://github.com/dotnet/aspnetcore/issues/17730
/// </summary>
// TODO-MAINTAINABILITY/PERFORMANCE: Remove as soon as possible, keep in sync with real implementation until then
public class BlazorWasmFCClauseStore : IKnowledgeBaseClauseStore
{
    // Yes, not actually a hash set, despite the name of the class. I want strong concurrency support,
    // but System.Collections.Concurrent doesn't include a hash set implementation. Using a ConcurrentDictionary
    // means some overhead (the values), but its not worth doing anything else for this basic implementation.
    // Might consider using a third-party package for an actual concurrent hash set at some point, but.. probably won't.
    // Consumers to whom this matters can always create their own implementation.
    private readonly ConcurrentDictionary<CNFDefiniteClause, byte> clauses = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorWasmFCClauseStore"/> class.
    /// </summary>
    public BlazorWasmFCClauseStore() { }

    /// <summary>
    /// <para>
    /// Initializes a new instance of the <see cref="BlazorWasmFCClauseStore"/> class that is pre-populated with some knowledge.
    /// </para>
    /// <para>
    /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
    /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
    /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
    /// </para>
    /// </summary>
    /// <param name="formulas">The initial content of the store.</param>
    public BlazorWasmFCClauseStore(IEnumerable<Formula> formulas)
    {
        foreach (var formula in formulas)
        {
            foreach (var clause in formula.ToCNF().Clauses)
            {
                if (!clause.IsDefiniteClause)
                {
                    throw new ArgumentException($"All forward chaining knowledge must be expressable as definite clauses. The normalisation of {formula} includes {clause}, which is not a definite clause");
                }

                clauses.TryAdd(new CNFDefiniteClause(clause), 0);
            }
        }
    }

    /// <inheritdoc/>
    public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(clauses.TryAdd(clause, 0));
    }

    /// <inheritdoc />
    public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await PeriodicYielder.PerhapsYield(cancellationToken);

        foreach (var clause in clauses.Keys)
        {
            cancellationToken.ThrowIfCancellationRequested();
            yield return clause;
        }
    }

    /// <inheritdoc />
    public Task<IQueryClauseStore> CreateQueryStoreAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IQueryClauseStore>(new QueryStore(clauses));
    }

    /// <summary>
    /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="HashSetClauseStore"/>.
    /// </summary>
    private class QueryStore(IEnumerable<KeyValuePair<CNFDefiniteClause, byte>> clauses) : IQueryClauseStore
    {
        private readonly ConcurrentDictionary<CNFDefiniteClause, byte> clauses = new(clauses);

        /// <inheritdoc />
        public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(clauses.TryAdd(clause, 0));
        }

        /// <inheritdoc />
        public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            await PeriodicYielder.PerhapsYield(cancellationToken);

            foreach (var clause in clauses.Keys)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return clause;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<CNFDefiniteClause> GetApplicableRules(
            IEnumerable<Predicate> facts,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            bool AnyConjunctUnifiesWithAnyFact(CNFDefiniteClause clause)
            {
                foreach (var conjunct in clause.Conjuncts)
                {
                    foreach (var fact in facts)
                    {
                        if (Unifier.TryCreate(conjunct, fact, out _))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            await foreach (var clause in this.WithCancellation(cancellationToken))
            {
                if (AnyConjunctUnifiesWithAnyFact(clause))
                {
                    yield return clause;
                }
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<(Predicate knownFact, VariableSubstitution unifier)> MatchWithKnownFacts(
            Predicate fact,
            VariableSubstitution constraints,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Here we just iterate through ALL known predicates trying to find something that unifies with the fact.
            // A better implementation would do some kind of indexing (or at least store facts and rules separately):
            await foreach (var knownClause in this.WithCancellation(cancellationToken))
            {
                if (knownClause.IsUnitClause)
                {
                    if (Unifier.TryUpdate(knownClause.Consequent, fact, constraints, out var unifier))
                    {
                        yield return (knownClause.Consequent, unifier);
                    }
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            //// Nothing to do..
        }
    }
}
