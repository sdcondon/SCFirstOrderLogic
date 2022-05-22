using SCFirstOrderLogic.SentenceManipulation;
using System;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Clause pair priority comparisons for use by <see cref="SimpleResolutionKnowledgeBase"/>.
    /// For context, see §9.5.6 ("Resolution Strategies") of 'Artifical Intelligence: A Modern Approach'.
    /// </summary>
    public class ClausePairPriorityComparisons
    {
        /// <summary>
        /// Naive comparison that orders clause pairs consistently but not according to any particular algorithm.
        /// </summary>
        public static Comparison<(CNFClause, CNFClause)> None { get; } = (x, y) =>
        {
            return x.GetHashCode().CompareTo(y.GetHashCode());
        };

        /// <summary>
        /// Comparison that gives priority to pairs where one of the clauses is a unit clause.
        /// <para/>
        /// NB: falls back on hash code comparison when not ordering because of unit clause presence. Given that
        /// some sentence things use reference equality (notably, symbols of standardised variables and Skolem functions),
        /// means that things can be ordered differently from one execution to the next. Not ideal..
        /// </summary>
        public static Comparison<(CNFClause, CNFClause)> UnitPreference { get; } =  (x, y) =>
        {
            var xHasUnitClause = x.Item1.IsUnitClause || x.Item2.IsUnitClause;
            var yHasUnitClause = y.Item1.IsUnitClause || y.Item2.IsUnitClause;

            if (xHasUnitClause && !yHasUnitClause)
            {
                return 1;
            }
            else if (!xHasUnitClause && yHasUnitClause)
            {
                return -1;
            }
            else
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            }
        };

        // Not mentioned in source material, but I figured it was a logical variant of unit preference.
        public static Comparison<(CNFClause, CNFClause)> TotalLiteralCountMinimisation { get; } = (x, y) =>
        {
            var xTotalClauseCount = x.Item1.Literals.Count + x.Item2.Literals.Count;
            var yTotalClauseCount = y.Item1.Literals.Count + y.Item2.Literals.Count;

            if (xTotalClauseCount < yTotalClauseCount)
            {
                return 1;
            }
            else if (xTotalClauseCount > yTotalClauseCount)
            {
                return -1;
            }
            else
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            }
        };
    }
}
