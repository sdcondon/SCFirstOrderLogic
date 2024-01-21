// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SCFirstOrderLogic.Inference.ForwardChaining;

/// <summary>
/// Sub-type of <see cref="IClauseStore"/> for types intended for storing the intermediate clauses of a particular query.
/// Notably, is <see cref="IDisposable"/> so that any unmanaged resources can be promptly tidied away once the query is complete.
/// </summary>
public interface IQueryClauseStore : IClauseStore, IDisposable
{
    /// <summary>
    /// Gets all of the rules whose conjuncts contain at least one that unifies with at least one of a given set of facts.
    /// </summary>
    /// <param name="facts">The facts.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>An async enumerable of applicable rules.</returns>
    IAsyncEnumerable<CNFDefiniteClause> GetApplicableRules(IEnumerable<Predicate> facts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds all known facts that unify with a given fact.
    /// </summary>
    /// <param name="fact">The fact to check for.</param>
    /// <param name="constraints">The variable values that must be respected.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>An async enumerable of fact-unifier pairs - one for each known fact that unifies with the given fact.</returns>
    IAsyncEnumerable<(Predicate knownFact, VariableSubstitution unifier)> MatchWithKnownFacts(Predicate fact, VariableSubstitution constraints, CancellationToken cancellationToken = default);
}
