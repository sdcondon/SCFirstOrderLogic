// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference;

/// <summary>
/// Handy base class for <see cref="IQuery"/> implementations that are executable step-by-step.
/// </summary>
/// <typeparam name="TStepResult">The type of the result of each step. This type should be a container for information on what happened during the step.</typeparam>
public abstract class SteppableQuery<TStepResult> : IQuery
{
    private int executeCount = 0;
    private bool? result = null;

    /// <inheritdoc/>
    public bool IsComplete => result.HasValue;

    /// <inheritdoc/>
    public bool Result
    {
        get
        {
            if (!result.HasValue)
            {
                throw new InvalidOperationException("Query is not yet complete");
            }

            return result.Value;
        }
    }

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
    // NB: While this is a rather low-level method, some cursory performance testing shows that using
    // ValueTask here tends to (slightly reduce GC pressure, sure, but) slow things down a bit overall.
    public abstract Task<TStepResult> NextStepAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        // ..while it might be nice to allow for other threads to just get the existing task back
        // if its already been started, the possibility of the cancellation token being different
        // makes it awkward. The complexity added by dealing with that simply isn't worth it.
        // (at a push could PERHAPS just throw if the CT is different - see CT equality remarks).
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

    /// <summary>
    /// Sets the result of the query and marks it as complete.
    /// </summary>
    /// <param name="result">The result of the query.</param>
    protected void SetResult(bool result)
    {
        this.result = result;
    }
}
