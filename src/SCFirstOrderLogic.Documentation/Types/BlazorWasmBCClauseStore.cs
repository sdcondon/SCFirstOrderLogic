using SCFirstOrderLogic.Documentation.Types;
using SCFirstOrderLogic.FormulaManipulation.Normalisation;
using SCFirstOrderLogic.FormulaManipulation.VariableManipulation;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace SCFirstOrderLogic.Inference.Basic.BackwardChaining;

/// <summary>
/// Implementation of <see cref="IClauseStore"/> geared towards use in Blazor WASM, before multithreading support is added.
/// Hacky - adds in a bunch of Task.Delay(1)'s.
/// https://github.com/dotnet/aspnetcore/issues/17730
/// </summary>
// TODO-MAINTAINABILITY/PERFORMANCE: Remove as soon as possible, keep in sync with real implementation until then
public class BlazorWasmBCClauseStore : IClauseStore
{
    // NB: the inner dictionary here is intended more as a hash set - but system.collections.concurrent doesn't
    // offer a concurrent hash set (and I want strong concurrency support). Not worth adding a third-party package for
    // this though. Consumers to whom this matters will likely be considering creating their own clause store anyway.
    private readonly ConcurrentDictionary<object, ConcurrentDictionary<CNFDefiniteClause, byte>> clausesByConsequentPredicateId = new();

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
        if (!clausesByConsequentPredicateId.TryGetValue(clause.Consequent.Identifier, out var clausesWithThisConsequentPredicateId))
        {
            clausesWithThisConsequentPredicateId = clausesByConsequentPredicateId[clause.Consequent.Identifier] = new ConcurrentDictionary<CNFDefiniteClause, byte>();
        }

        return Task.FromResult(clausesWithThisConsequentPredicateId.TryAdd(clause, 0));
    }

    /// <inheritdoc />
    public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        await PeriodicYielder.PerhapsYield(cancellationToken);

        foreach (var clauseList in clausesByConsequentPredicateId.Values)
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
        await PeriodicYielder.PerhapsYield(cancellationToken);

        if (clausesByConsequentPredicateId.TryGetValue(goal.Identifier, out var clausesWithThisGoal))
        {
            foreach (var clause in clausesWithThisGoal.Keys)
            {
                var restandardisedClause = clause.Restandardise();

                if (Unifier.TryUpdate(restandardisedClause.Consequent, goal, constraints, out var substitution))
                {
                    yield return (restandardisedClause, substitution);
                }
            }
        }
    }
}
