// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.LanguageIntegration;

/// <summary>
/// Helpful extension methods for <see cref="ILinqKnowledgeBase{TDomain, TElement}"/> instances.
/// </summary>
public static class ILinqKnowledgeBaseExtensions
{
    /// <summary>
    /// Inform a knowledge base that a given enumerable of expressions about the domain are all true for all models that it will be asked about.
    /// </summary>
    /// <param name="knowledgeBase">The knowledge base to tell.</param>
    /// <param name="sentences">The expressions that are always true.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    public static async Task Tell<TDomain, TElement>(this ILinqKnowledgeBase<TDomain, TElement> knowledgeBase, IEnumerable<Expression<Predicate<TDomain>>> sentences, CancellationToken cancellationToken)
        where TDomain : IEnumerable<TElement>
    {
        foreach (var sentence in sentences)
        {
            await knowledgeBase.TellAsync(sentence, cancellationToken);
        }
    }
}
