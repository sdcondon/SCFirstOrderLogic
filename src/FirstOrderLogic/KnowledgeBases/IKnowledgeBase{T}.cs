using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace LinqToKB.FirstOrderLogic.KnowledgeBases
{
    /// <summary>
    /// A store of knowledge expressed as statements of propositional logic (in turn expressed as LINQ expressions).
    /// </summary>
    /// <typeparam name="TModel">
    /// The type that the sentences passed to this class refer to.
    /// </typeparam>
    public interface IKnowledgeBase<TModel>
    {
        /// <summary>
        /// Inform the knowledge base that a given sentence about the domain is true for all models that it will be asked about.
        /// </summary>
        /// <param name="sentence">The sentence that is always true.</param>
        public void Tell(Expression<Predicate<IEnumerable<TModel>>> sentence);

        /// <summary>
        /// Ask the knowledge base if a given sentence about the model must be true, given what it knows.
        /// </summary>
        /// <param name="query">The sentence to ask about.</param>
        /// <returns>True if the sentence is known to be true, false if it is known to be false or cannot be determined.</returns>
        public bool Ask(Expression<Predicate<IEnumerable<TModel>>> query);
        
        //// TODO-FEATURE: AskVars..
    }
}
