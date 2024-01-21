// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining;

/// <summary>
/// Implementation of <see cref="IClauseStore"/> that just uses an in-memory dictionary (keyed by consequent identifier) to store known clauses.
/// </summary>
public class DictionaryClauseStore : IClauseStore
{
    // NB: the inner dictionary here is intended more as a hash set - but system.collections.concurrent doesn't
    // offer a concurrent hash set (and I want strong concurrency support). Not worth adding a third-party package for
    // this though. Consumers to whom this matters will likely be considering creating their own clause store anyway.
    private readonly ConcurrentDictionary<object, ConcurrentDictionary<CNFDefiniteClause, byte>> clausesByConsequentPredicateId = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryClauseStore"/> class.
    /// </summary>
    public DictionaryClauseStore() { }

    /// <summary>
    /// <para>
    /// Initializes a new instance of the <see cref="DictionaryClauseStore"/> class that is pre-populated with some knowledge.
    /// </para>
    /// <para>
    /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
    /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
    /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
    /// </para>
    /// </summary>
    /// <param name="sentences">The initial content of the store.</param>
    public DictionaryClauseStore(IEnumerable<Sentence> sentences)
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

#pragma warning disable CS1998 // async lacks await.. Could add await Task.Yield() to silence this, but it is not worth the overhead.
    /// <inheritdoc />
    public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
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
        if (clausesByConsequentPredicateId.TryGetValue(goal.Identifier, out var clausesWithThisGoal))
        {
            foreach (var clause in clausesWithThisGoal.Keys)
            {
                // TODO-CODE-STINK: restandardisation doesn't belong here - the need to restandardise is due to the algorithm we use.
                // A query other than SimpleBackwardChain might not need this (if e.g. it had a different unifier instance for each step).
                // TODO*-BUG?: hmm, looks odd. we restandardise, THEN do a thing involving the constraint.. When could the constraint ever
                // kick in? Verify test coverage here..
                var restandardisedClause = clause.Restandardise();

                if (Unifier.TryUpdate(restandardisedClause.Consequent, goal, constraints, out var substitution))
                {
                    yield return (restandardisedClause, substitution);
                }
            }
        }
    }
#pragma warning restore CS1998
}
