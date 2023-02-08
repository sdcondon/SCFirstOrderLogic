namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Interface for types that implement a resolution strategy that can
    /// be utilised by <see cref="SimpleResolutionKnowledgeBase"/> instances.
    /// </summary>
    public interface ISimpleResolutionStrategy
    {
        /// <summary>
        /// Gets the clause store to use.
        /// </summary>
        IKnowledgeBaseClauseStore ClauseStore { get; }

        /// <summary>
        /// Creates a resolution queue for use by a particular query.
        /// </summary>
        /// <returns>A resolution queue for use by a particular query.</returns>
        ISimpleResolutionQueue MakeResolutionQueue();
    }
}
