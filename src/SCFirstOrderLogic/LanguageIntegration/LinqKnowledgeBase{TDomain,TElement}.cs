// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.Inference;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.LanguageIntegration;

/// <summary>
/// An implementation of the <see cref="ILinqKnowledgeBase{TDomain, TElement}"/> interface that simply wraps an inner <see cref="IKnowledgeBase"/> instance.
/// </summary>
/// <typeparam name="TDomain">The type of the domain. The domain must be modelled as an <see cref="IEnumerable{T}"/>.</typeparam>
/// <typeparam name="TElement">The type that the sentences passed to this class refer to.</typeparam>
public class LinqKnowledgeBase<TDomain, TElement> : ILinqKnowledgeBase<TDomain, TElement>
    where TDomain : IEnumerable<TElement>
{
    private readonly IKnowledgeBase innerKnowledgeBase;

    /// <summary>
    /// Initialises a new instance of the <see cref="LinqKnowledgeBase{TDomain, TElement}"/> class.
    /// </summary>
    public LinqKnowledgeBase(IKnowledgeBase innerKnowledgeBase) => this.innerKnowledgeBase = innerKnowledgeBase;

    /// <inheritdoc/>
    public async Task<bool> AskAsync(Expression<Predicate<TDomain>> query, CancellationToken cancellationToken = default)
    {
        return await innerKnowledgeBase.AskAsync(SentenceFactory.Create<TDomain, TElement>(query), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task TellAsync(Expression<Predicate<TDomain>> sentence, CancellationToken cancellationToken = default)
    {
        await innerKnowledgeBase.TellAsync(SentenceFactory.Create<TDomain, TElement>(sentence), cancellationToken);
    }
}
