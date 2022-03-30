using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Probably needless copmlexity..
    /// </summary>
    public interface IResolutionQuery
    {
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

        bool Complete();
    }
}
