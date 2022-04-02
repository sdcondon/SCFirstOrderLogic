using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Common clause pair filters for use by <see cref="SimpleResolutionKnowledgeBase"/>.
    /// For context, see §9.5.6 ("Resolution Strategies") of Artifical Intelligence: A Modern Approach.
    /// </summary>
    public class ClausePairFilters
    {
        /// <summary>
        /// Naive comparison that orders clause paris consistently but not according to any particular algorithm.
        /// </summary>
        public static Func<(CNFClause, CNFClause), bool> None { get; } = pair => true;

        /// <summary>
        /// Comparison that gives priority to pairs where one of the clauses is a unit clause.
        /// </summary>
        public static Func<(CNFClause, CNFClause), bool> UnitResolution { get; } = pair => pair.Item1.Literals.Count == 1 || pair.Item2.Literals.Count == 1;
    }
}
