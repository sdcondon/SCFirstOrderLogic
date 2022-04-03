using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System.Collections.Generic;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Clause pair priority comparers for use by <see cref="SimpleResolutionKnowledgeBase"/>.
    /// For context, see §9.5.6 ("Resolution Strategies") of Artifical Intelligence: A Modern Approach.
    /// </summary>
    public class ClausePairPriorityComparers
    {
        /// <summary>
        /// Naive comparison that orders clause pairs consistently but not according to any particular algorithm.
        /// </summary>
        public static IComparer<(CNFClause, CNFClause)> None { get; } = Comparer<(CNFClause, CNFClause)>.Create((x, y) =>
        {
            return x.GetHashCode().CompareTo(y.GetHashCode());
        });

        /// <summary>
        /// Comparison that gives priority to pairs where one of the clauses is a unit clause.
        /// </summary>
        public static IComparer<(CNFClause, CNFClause)> UnitPreference { get; } = Comparer<(CNFClause, CNFClause)>.Create((x, y) =>
        {
            if (x.Item1.Literals.Count == 1 || x.Item2.Literals.Count == 1 && !(y.Item1.Literals.Count == 1 || y.Item2.Literals.Count == 1))
            {
                return 1;
            }
            else if (!(x.Item1.Literals.Count == 1 || x.Item2.Literals.Count == 1) && y.Item1.Literals.Count == 1 || y.Item2.Literals.Count == 1)
            {
                return -1;
            }
            else
            {
                return x.GetHashCode().CompareTo(y.GetHashCode());
            }
        });
    }
}
