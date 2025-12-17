// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.Inference;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.LanguageIntegration;

/// <summary>
/// An implementation of the <see cref="ILinqKnowledgeBase{TElement}"/> interface that simply wraps an inner <see cref="IKnowledgeBase"/> instance.
/// </summary>
/// <typeparam name="TElement">
/// The type that the sentences passed to this class refer to.
/// </typeparam>
public class LinqKnowledgeBase<TElement> : ILinqKnowledgeBase<TElement>
{
    private readonly IKnowledgeBase innerKnowledgeBase;

    /// <summary>
    /// Initialises a new instance of the <see cref="LinqKnowledgeBase{TElement}"/> class.
    /// </summary>
    public LinqKnowledgeBase(IKnowledgeBase innerKnowledgeBase) => this.innerKnowledgeBase = innerKnowledgeBase;

    /// <inheritdoc/>
    public async Task<bool> AskAsync(Expression<Predicate<IEnumerable<TElement>>> query, CancellationToken cancellationToken = default)
    {
        return await innerKnowledgeBase.AskAsync(FormulaFactory.Create(query), cancellationToken);
    }

    /// <inheritdoc/>
    public async Task TellAsync(Expression<Predicate<IEnumerable<TElement>>> sentence, CancellationToken cancellationToken = default)
    {
        await innerKnowledgeBase.TellAsync(FormulaFactory.Create(sentence), cancellationToken);
    }
}
