using System.Collections.Generic;

namespace SCFirstOrderLogic.TermIndexing
{
    /// <summary>
    /// Interface for path tree node types whose descendents represent all of the values of a particular function parameter
    /// (or, for the root node, all values of the root element of the term) that occur within the terms stored in a path tree.
    /// </summary>
    public interface IPathTreeParameterNode<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        IReadOnlyDictionary<IPathTreeArgumentNodeKey, IPathTreeArgumentNode<TValue>> Children { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        IPathTreeArgumentNode<TValue> GetOrAddChild(IPathTreeArgumentNodeKey key);
    }
}
