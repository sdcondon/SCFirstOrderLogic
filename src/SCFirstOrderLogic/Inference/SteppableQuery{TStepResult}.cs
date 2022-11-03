using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Base class for <see cref="IQuery"/> implementations that are executable step-by-step.
    /// </summary>
    /// <typeparam name="TStepResult">The type of the result of each step. This type should be a container for information on what happened during the step.</typeparam>
    public abstract class SteppableQuery<TStepResult> : IQuery
    {
        /// <inheritdoc />
        public abstract bool IsComplete { get; }

        /// <inheritdoc />
        public abstract bool Result { get; }

        /// <summary>
        /// Executes the next step of the query.
        /// <para/>
        /// Calling <see cref="NextStepAsync"/> on a completed query should result in an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A container for information on what happened during the step.</returns>
        // TODO-BREAKING: Should this use ValueTask? Investigate me.
        public abstract Task<TStepResult> NextStepAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            while (!IsComplete)
            {
                await NextStepAsync(cancellationToken);
            }

            return Result;
        }

        /// <inheritdoc />
        public abstract void Dispose();
    }
}
