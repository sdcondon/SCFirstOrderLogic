using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Base class for <see cref="IQuery"/> implementations that are executable step-by-step.
    /// </summary>
    public abstract class SteppableQuery : IQuery
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
        public abstract Task NextStepAsync(CancellationToken cancellationToken = default);

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
