// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution;

/// <summary>
/// <para>
/// A knowledge base that uses a very simple implementation of resolution to answer queries.
/// </para>
/// <para>
/// See §9.5 ("Resolution") of 'Artificial Intelligence: A Modern Approach' for a detailed explanation of resolution.
/// </para>
/// </summary>
public class ResolutionKnowledgeBase : IKnowledgeBase
{
    private readonly IResolutionStrategy strategy;

    /// <summary>
    /// Initialises a new instance of the <see cref="ResolutionKnowledgeBase"/> class.
    /// </summary>
    /// <param name="strategy">The resolution strategy to use.</param>
    public ResolutionKnowledgeBase(IResolutionStrategy strategy) => this.strategy = strategy;

    /// <inheritdoc />
    public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
    {
        foreach(var clause in sentence.ToCNF().Clauses)
        {
            await strategy.AddClauseAsync(clause, cancellationToken);
        }
    }

    /// <inheritdoc />
    async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken)
    {
        return await CreateQueryAsync(sentence, cancellationToken);
    }

    /// <summary>
    /// Initiates a new query against the knowledge base.
    /// </summary>
    /// <param name="sentence">The query sentence.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A task that returns an <see cref="ResolutionQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
    public Task<ResolutionQuery> CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken = default)
    {
        return ResolutionQuery.CreateAsync(sentence, strategy, cancellationToken);
    }

    /// <summary>
    /// Initiates a new query against the knowledge base.
    /// </summary>
    /// <param name="sentence">The query sentence.</param>
    /// <returns>An <see cref="ResolutionQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
    public ResolutionQuery CreateQuery(Sentence sentence)
    {
        return CreateQueryAsync(sentence).GetAwaiter().GetResult();
    }
}
