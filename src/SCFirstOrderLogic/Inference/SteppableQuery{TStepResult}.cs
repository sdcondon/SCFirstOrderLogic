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
        private int executeCount = 0;

        /// <inheritdoc />
        public abstract bool IsComplete { get; }

        /// <inheritdoc />
        public abstract bool Result { get; }

        /// <summary>
        /// <para>
        /// Executes the next step of the query.
        /// </para>
        /// <para>
        /// Calling <see cref="NextStepAsync"/> on a completed query should result in an <see cref="InvalidOperationException"/>.
        /// </para>
        /// </summary>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A container for information on what happened during the step.</returns>
        // TODO-BREAKING-V4?: Should this use ValueTask to reduce GC load? Investigate me. Yeah, high-perf isn't the point of this package,
        // but given that this is an abstraction, its constraining what people *could* achieve with it. So worth a look at least.
        public abstract Task<TStepResult> NextStepAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc />
        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // ..while it might be nice to allow for other threads to just get the existing task back
            // if its already been started, the possibility of the cancellation token being different
            // makes it awkward. The complexity added by dealing with that simply isn't worth it.
            // So, we just throw if the query is already in progress. Messing about with a query from
            // multiple threads is fairly unlikely anyway (as opposed to wanting an individual query to
            // parallelise itself - which is definitely something I want to look at).
            if (Interlocked.Exchange(ref executeCount, 1) == 1)
            {
                throw new InvalidOperationException("Query execution has already begun via a prior ExecuteAsync invocation");
            }

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
