#if FALSE
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Basic implementation of <see cref="IUnifierStore"/> that just maintains all known sentences in a <see cref="List{T}"./>
    /// </summary>
    public class ListUnifierStore : IUnifierStore
    {
        /// <inheritdoc />
        public IAsyncEnumerable<IDictionary<VariableReference, Constant>> Fetch(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task StoreAsync(Sentence sentence)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
