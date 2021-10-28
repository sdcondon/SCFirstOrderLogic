using LinqToKB.FirstOrderLogic.Sentences;
using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Basic implementation of <see cref="IUnifierStore{TDomain, TElement}"/> that just maintains all known sentences in a <see cref="List{T}"./>
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class ListUnifierStore<TDomain, TElement> : IUnifierStore<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <inheritdoc />
        public IEnumerable<IDictionary<Variable<TDomain, TElement>, Constant<TDomain, TElement>>> Fetch(Sentence<TDomain, TElement> sentence)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Store(Sentence<TDomain, TElement> sentence)
        {
            throw new NotImplementedException();
        }
    }
}
