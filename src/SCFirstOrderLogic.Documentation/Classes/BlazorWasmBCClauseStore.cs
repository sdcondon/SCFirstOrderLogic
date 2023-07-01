using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// Implementation of <see cref="IClauseStore"/> geared towards use in Blazor WASM, before multithreading support is added in v8.
    /// Hacky - adds in a bunch of Task.Delay(1)'s.
    /// https://stackoverflow.com/questions/71287775/how-to-correctly-create-an-async-method-in-blazor
    /// </summary>
    public class BlazorWasmBCClauseStore : IClauseStore
    {
        // NB: the inner dictionary here is intended more as a hash set - but system.collections.concurrent doesn't
        // offer a concurrent hash set (and I want strong concurrency support). Not worth adding a third-party package for
        // this though. Consumers to whom this matters will likely be considering creating their own clause store anyway.
        private readonly ConcurrentDictionary<object, ConcurrentDictionary<CNFDefiniteClause, byte>> clausesByConsequentSymbol = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="BlazorWasmBCClauseStore"/> class.
        /// </summary>
        public BlazorWasmBCClauseStore() { }

        /// <summary>
        /// <para>
        /// Initializes a new instance of the <see cref="BlazorWasmBCClauseStore"/> class that is pre-populated with some knowledge.
        /// </para>
        /// <para>
        /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
        /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
        /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
        /// </para>
        /// </summary>
        /// <param name="sentences">The initial content of the store.</param>
        public BlazorWasmBCClauseStore(IEnumerable<Sentence> sentences)
        {
            foreach (var sentence in sentences)
            {
                foreach (var clause in sentence.ToCNF().Clauses)
                {
                    if (!clause.IsDefiniteClause)
                    {
                        throw new ArgumentException($"All forward chaining knowledge must be expressable as definite clauses. The normalisation of {sentence} includes {clause}, which is not a definite clause");
                    }

                    AddAsync(new CNFDefiniteClause(clause)).GetAwaiter().GetResult();
                }
            }
        }

        /// <inheritdoc/>
        public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
        {
            if (!clausesByConsequentSymbol.TryGetValue(clause.Consequent.Symbol, out var clausesWithThisConsequentSymbol))
            {
                clausesWithThisConsequentSymbol = clausesByConsequentSymbol[clause.Consequent.Symbol] = new ConcurrentDictionary<CNFDefiniteClause, byte>();
            }

            return Task.FromResult(clausesWithThisConsequentSymbol.TryAdd(clause, 0));
        }

        /// <inheritdoc />
        public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);

            foreach (var clauseList in clausesByConsequentSymbol.Values)
            {
                foreach (var clause in clauseList.Keys)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return clause;
                }
            }
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<(CNFDefiniteClause Clause, VariableSubstitution Substitution)> GetClauseApplications(
            Predicate goal,
            VariableSubstitution constraints,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            await Task.Delay(1, cancellationToken);

            if (clausesByConsequentSymbol.TryGetValue(goal.Symbol, out var clausesWithThisGoal))
            {
                foreach (var clause in clausesWithThisGoal.Keys)
                {
                    // TODO-CODE-STINK: restandardisation doesn't belong here - the need to restandardise is due to the algorithm we use.
                    // A query other than SimpleBackwardChain might not need this (if e.g. it had a different unifier instance for each step).
                    // TODO*-BUG?: hmm, looks odd. we restandardise, THEN do a thing involving the constraint.. When could the constraint ever
                    // kick in? Verify test coverage here..
                    var restandardisedClause = clause.Restandardise();
                    var substitution = new VariableSubstitution(constraints);

                    if (Unifier.TryUpdate(restandardisedClause.Consequent, goal, substitution))
                    {
                        yield return (restandardisedClause, substitution);
                    }
                }
            }
        }
    }
}
