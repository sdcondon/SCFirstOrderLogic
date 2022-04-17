namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Sub-type of <see cref="IClauseStore"/> for implementations intended for storing the known clauses
    /// for a knowledge base as a whole. In particular, adds a method for creating a <see cref="IQueryClauseStore"/>
    /// for storing the intermediate clauses of an individual query.
    /// </summary>
    public interface IKnowledgeBaseClauseStore : IClauseStore
    {
        /// <summary>
        /// Creates essentially a (disposable) copy of the current store, for storing the intermediate clauses of a particular query.
        /// </summary>
        /// <returns>A new <see cref="IQueryClauseStore"/> instance.</returns>
        IQueryClauseStore CreateQueryClauseStore();
    }
}
