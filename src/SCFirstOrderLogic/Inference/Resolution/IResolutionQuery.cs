using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// An interface for representations of an individual query - for fine-grained step-by-step execution and examination.
    /// Probably needless complexity, given that we've only got a single resolution knowledge base - may disappear..
    /// </summary>
    public interface IResolutionQuery
    {
        /// <summary>
        /// Gets the (CNF representation of) the negation of the query.
        /// </summary>
        public CNFSentence NegatedQuery { get; }

        /// <summary>
        /// Gets a value indicating whether the query is complete.
        /// The result of completed queries is available via the <see cref="Result"/> property.
        /// Calling <see cref="NextStep"/> on a completed query will result in an <see cref="InvalidOperationException"/>.
        /// </summary>
        bool IsComplete { get; }

        /// <summary>
        /// Gets a value indicating the result of the query.
        /// True indicates that the query is known to be true.
        /// False indicates that the query is known to be false or cannot be determined.
        /// </summary>
        /// <exception cref="InvalidOperationException">The query is not yet complete.</exception>
        bool Result { get; }

        /// <summary>
        /// Gets a mapping from a clause to the two clauses from which it was inferred.
        /// </summary>
        IReadOnlyDictionary<CNFClause, (CNFClause, CNFClause, IReadOnlyDictionary<VariableReference, Term>)> Steps { get; }

        /// <summary>
        /// Executes the next step of the query.
        /// </summary>
        void NextStep();

        /// <summary>
        /// Continuously executes the next step of the query until it completes.
        /// </summary>
        /// <returns>the result of the query.</returns>
        bool Complete();
    }
}
