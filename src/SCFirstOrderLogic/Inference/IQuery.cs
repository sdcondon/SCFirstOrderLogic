using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// An interface for representations of an individual query - for fine-grained step-by-step execution and examination.
    /// </summary>
    public interface IQuery : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether the query is complete.
        /// The result of completed queries is available via the <see cref="Result"/> property.
        /// Calling <see cref="NextStep"/> on a completed query will result in an <see cref="InvalidOperationException"/>.
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
        /// Executes the next step of the query.
        /// <para/>
        /// NB: This is an asynchronous method ultimately because "real" knowledge bases will often need to do IO to retrieve knowledge.
        /// </summary>
        Task NextStepAsync(CancellationToken cancellationToken = default);
    }
}
