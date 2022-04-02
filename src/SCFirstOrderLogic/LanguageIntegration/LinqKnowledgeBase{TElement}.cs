using SCFirstOrderLogic.Inference;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SCFirstOrderLogic.LanguageIntegration
{
    /// <typeparam name="TElement">
    /// The type that the sentences passed to this class refer to.
    /// </typeparam>
    public class LinqKnowledgeBase<TElement> : ILinqKnowledgeBase<TElement>
    {
        private readonly IKnowledgeBase innerKnowledgeBase;

        /// <summary>
        /// Initialises a new instance of the <see cref="LinqKnowledgeBase{TElement}"/> class.
        /// </summary>
        public LinqKnowledgeBase(IKnowledgeBase innerKnowledgeBase) => this.innerKnowledgeBase = innerKnowledgeBase;

        /// <inheritdoc/>
        public bool Ask(Expression<Predicate<IEnumerable<TElement>>> query)
        {
            return innerKnowledgeBase.Ask(SentenceFactory.Create(query));
        }

        /// <inheritdoc/>
        public void Tell(Expression<Predicate<IEnumerable<TElement>>> sentence)
        {
            innerKnowledgeBase.Tell(SentenceFactory.Create(sentence));
        }
    }
}
