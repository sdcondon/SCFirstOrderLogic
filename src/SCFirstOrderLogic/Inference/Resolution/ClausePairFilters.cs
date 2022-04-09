using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Clause pair filters for use by <see cref="SimpleResolutionKnowledgeBase"/>.
    /// For context, see §9.5.6 ("Resolution Strategies") of 'Artifical Intelligence: A Modern Approach'.
    /// </summary>
    public class ClausePairFilters
    {
        /// <summary>
        /// "Filter" that doesn't actually filter out any pairings.
        /// </summary>
        public static Func<(CNFClause, CNFClause), bool> None { get; } = pair => true;

        /// <summary>
        /// Filter that requires that one of the clauses be a unit clause.
        /// </summary>
        public static Func<(CNFClause, CNFClause), bool> UnitResolution { get; } = pair => pair.Item1.Literals.Count == 1 || pair.Item2.Literals.Count == 1;
    }
}
