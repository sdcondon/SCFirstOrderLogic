using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Helpful extension methods for <see cref="IKnowledgeBase{TDomain, TElement}"/> instances.
    /// </summary>
    public static class IKnowledgeBaseExtensions
    {
        /// <summary>
        /// Inform a knowledge base that a given enumerable of expressions about the domain are all true for all models that it will be asked about.
        /// </summary>
        /// <param name="sentence">The expressions that are always true.</param>
        public static void Tell<TDomain, TElement>(this IKnowledgeBase<TDomain, TElement> knowledgeBase, IEnumerable<Expression<Predicate<TDomain>>> sentences)
            where TDomain : IEnumerable<TElement>
        {
            foreach (var sentence in sentences)
            {
                knowledgeBase.Tell(sentence);
            }
        }
    }
}
