#if FALSE
using SCFirstOrderLogic.SentenceManipulation;
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
        private readonly List<CNFClause> clauses = new List<CNFClause>();

        /// <inheritdoc />
        public async Task StoreAsync(Sentence sentence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IAsyncEnumerable<IDictionary<VariableReference, Constant>> Fetch(Sentence sentence)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
