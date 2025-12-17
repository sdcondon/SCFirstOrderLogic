// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference;

/// <summary>
/// A store of knowledge expressed as sentences of first-order logic.
/// </summary>
public interface IKnowledgeBase
{
    /// <summary>
    /// <para>
    /// Tells the knowledge base that a given <see cref="Formula"/> can be assumed to hold true when answering queries.
    /// </para>
    /// <para>
    /// NB: This is an asynchronous method ultimately because "real" knowledge bases will often need to do IO to store knowledge.
    /// </para>
    /// </summary>
    /// <param name="sentence">A sentence that can be assumed to hold true when answering queries.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    Task TellAsync(Formula sentence, CancellationToken cancellationToken = default);

    /// <summary>
    /// <para>
    /// Initiates a new query against the knowledge base.
    /// </para>
    /// <para>
    /// NB #1: We define an <see cref="IQuery"/> interface (as opposed to directly having an AskAsync method that returns
    /// a <see cref="Task{Boolean}"/>) so that it is easy for implementations to add additional behaviours - such as
    /// step-by-step execution (see <see cref="SteppableQuery{TStepResult}"/>) and result explanations. Note that 
    /// Ask(Async) extension methods do exist (which create queries and immediately execute them to completion).
    /// </para>
    /// <para>
    /// NB #2: This method itself is async (rather than just returning an <see cref="IQuery"/>) to allow for implementations
    /// that use IO to initialise the query before returning it (e.g. setting up external storage for intermediate clauses).
    /// We view this kind of initialisation as a significant enough milestone to be separately monitorable (rather than requiring
    /// it to just be folded into the query object's implementation). Note that a synchronous extension method does exist, though.
    /// </para>
    /// </summary>
    /// <param name="query">The query sentence.</param>
    /// <param name="cancellationToken">A cancellation token for the operation.</param>
    /// <returns>A task that returns an <see cref="IQuery"/> implementation that can be used to execute the query.</returns>
    Task<IQuery> CreateQueryAsync(Formula query, CancellationToken cancellationToken = default);
}
