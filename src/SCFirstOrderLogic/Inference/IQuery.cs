using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// <para>
    /// An interface for representations of an individual query. 
    /// </para>
    /// <para>
    /// We define our own interface (instead of just using a <see cref="Task{Boolean}"/>) so that it is
    /// easy for implementations to add additional behaviours - such as step-by-step execution 
    /// (see <see cref="SteppableQuery{TStepResult}"/>) and result explanations. However, note the existence
    /// of the <see cref="IQueryExtensions.GetAwaiter(IQuery)"/> extension method - so that query instances
    /// can be awaited directly.
    /// </para>
    /// </summary>
    public interface IQuery : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the query is complete.
        /// The result of completed queries is available via the <see cref="Result"/> property.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Gets a value indicating the result of the query.
        /// True indicates that the query is known to be true.
        /// False indicates that the query is known to be false or cannot be determined.
        /// </summary>
        /// <exception cref="InvalidOperationException">The query is not yet complete.</exception>
        bool Result { get; }

        /// <summary>
        /// <para>
        /// Executes the query to completion.
        /// </para>
        /// <para>
        /// NB: This is an asynchronous method ultimately because "real" knowledge bases will often need to do IO to retrieve knowledge.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task, the result of which is the result of the query.</returns>
        Task<bool> ExecuteAsync(CancellationToken cancellationToken = default);
    }
}
