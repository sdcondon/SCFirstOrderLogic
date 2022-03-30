#if FALSE
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.Inference.UnifierStorage
{
    /// <summary>
    /// Basic implementation of <see cref="IUnifierStore"/> that just maintains all known sentences in a <see cref="List{T}"./>
    /// </summary>
    public class ListUnifierStore : IUnifierStore
    {
        /// <inheritdoc />
        public IEnumerable<IDictionary<VariableReference, Constant>> Fetch(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Store(Sentence sentence)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
