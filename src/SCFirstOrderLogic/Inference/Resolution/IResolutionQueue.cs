using System;
using System.Collections.Concurrent;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// <para>
    /// Interface for types that implement resolution queuing logic for a <see cref="IResolutionStrategy"/>.
    /// Such logic might be a plain old queue, might be prioritised in some way, or may be something altogether more complex.
    /// </para>
    /// <para>
    /// NB: I don't blame MS for not giving us a generic queue interface (after all, allowing for abstraction over
    /// queue implementation is a somewhat niche requirement), but obviously we'd use one instead of this
    /// if there was one. <see cref="IProducerConsumerCollection{T}"/> was briefly considered as an alternative, but
    /// given that it is fundamentally geared towards production and consumption happening on separate threads
    /// (and this is not how our resolution algorithm works), it was rejected in favour of this interface.
    /// </para>
    /// </summary>
    public interface IResolutionQueue
    {
        /// <summary>
        /// Gets a value indicating whether the queue is empty.
        /// </summary>
        bool IsEmpty { get; }

        /// <summary>
        /// Enqueues a resolution to be further explored.
        /// </summary>
        /// <param name="resolution">The resolution to enqueue.</param>
        void Enqueue(ClauseResolution resolution);

        /// <summary>
        /// Dequeues the next resolution for further exploration.
        /// </summary>
        /// <returns>The next resolution for further exploration.</returns>
        ClauseResolution Dequeue();
    }
}
