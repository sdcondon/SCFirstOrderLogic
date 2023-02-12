namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Interface for types that implement resolution queuing logic for a <see cref="IResolutionStrategy"/>.
    /// Such logic might be a plain old queue, might be prioritised in some way, or may be something more complex.
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
