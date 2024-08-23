using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SCFirstOrderLogic.Inference.Basic.Resolution;

/// <summary>
/// Implementation of <see cref="IClauseStore"/> geared towwards use in Blazor WASM, before multithreading support is added in v8.
/// Hacky - adds in a bunch of Task.Delay(1)'s.
/// https://stackoverflow.com/questions/71287775/how-to-correctly-create-an-async-method-in-blazor
/// </summary>
public class BlazorWasmResClauseStore : IKnowledgeBaseClauseStore
{
    // Yes, not actually a hash set, despite the name of the class. I want strong concurrency support,
    // but System.Collections.Concurrent doesn't include a hash set implementation. Using a ConcurrentDictionary
    // means some overhead (the values), but its not worth doing anything else for this basic implementation.
    // Might consider using a third-party package for an actual concurrent hash set at some point, but.. probably won't.
    // Consumers to whom this matters can always create their own implementation.
    private readonly ConcurrentDictionary<CNFClause, byte> clauses = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorWasmResClauseStore"/> class.
    /// </summary>
    public BlazorWasmResClauseStore() { }

    /// <summary>
    /// <para>
    /// Initializes a new instance of the <see cref="BlazorWasmResClauseStore"/> class that is pre-populated with some knowledge.
    /// </para>
    /// <para>
    /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
    /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
    /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
    /// </para>
    /// </summary>
    /// <param name="sentences">The initial content of the store.</param>
    public BlazorWasmResClauseStore(IEnumerable<Sentence> sentences)
    {
        foreach (var sentence in sentences)
        {
            foreach (var clause in sentence.ToCNF().Clauses)
            {
                clauses.TryAdd(clause, 0);
            }
        }
    }

    /// <inheritdoc />
    public Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(clauses.TryAdd(clause, 0));
    }

    /// <inheritdoc />
    public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await Task.Delay(1, cancellationToken);

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
    private class QueryStore : IQueryClauseStore
    {
        private readonly ConcurrentDictionary<CNFClause, byte> clauses = new();

        public QueryStore(IEnumerable<KeyValuePair<CNFClause, byte>> clauses) => this.clauses = new(clauses);

        /// <inheritdoc />
        public Task<bool> AddAsync(CNFClause clause, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(clauses.TryAdd(clause, 0));
        }

        /// <inheritdoc />
        public async IAsyncEnumerator<CNFClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);

            foreach (var clause in clauses.Keys)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return clause;
            }
        }

        /// <inheritdoc />
        public async IAsyncEnumerable<ClauseResolution> FindResolutions(
            CNFClause clause,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await foreach (var otherClause in this.WithCancellation(cancellationToken))
            {
                foreach (var resolution in ClauseResolution.Resolve(clause, otherClause))
                {
                    yield return resolution;
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
