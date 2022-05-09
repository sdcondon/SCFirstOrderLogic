using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;

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
            if ((x.Item1.IsUnitClause || x.Item2.IsUnitClause) && !(y.Item1.IsUnitClause || y.Item2.IsUnitClause))
            {
                return 1;
            }
            else if (!(x.Item1.IsUnitClause || x.Item2.IsUnitClause) && (y.Item1.IsUnitClause || y.Item2.IsUnitClause))
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
