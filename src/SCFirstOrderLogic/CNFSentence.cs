using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a <see cref="Sentence"/> in conjunctive normal form (CNF).
    /// </summary>
    public class CNFSentence
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFSentence"/> class from an enumerable of clauses.
        /// </summary>
        /// <param name="clauses">The set of clauses to be included in the sentence.</param>
        public CNFSentence(IEnumerable<CNFClause> clauses)
        {
            // NB #1: We *could* actually use an immutable type to stop unscrupulous users from making it mutable by casting,
            // but its a super low-level class and I'd so far I've erred on the side of using the simplest/smallest
            // implementation possible - that is, an array.
            // NB #2: Note that we order clauses - important to justifiably consider the sentence "normalised".
            // TODO-BUG-MAJOR: Potential equality bug on hash code collision..
            // One would hope that OrderedImmutableSet handles equality well.
            // Using a set would also deal with being handed something containing dups.
            Clauses = clauses.OrderBy(c => c.GetHashCode()).ToArray();
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="CNFSentence"/> class, implicitly converting the provided sentence to CNF in the process.
        /// </summary>
        /// <param name="sentence">The sentence to (convert and) represent.</param>
        public CNFSentence(Sentence sentence)
            : this(ConstructionVisitor.GetClauses(sentence))
        {
        }

        /// <summary>
        /// Gets the collection of clauses that comprise this CNF sentence.
        /// </summary>
        // TODO: logically, this should be a set - IReadOnlySet<> or IImmutableSet<> would both be non-breaking.
        // Investigate perf impact of ImmutableSortedSet?
        public IReadOnlyCollection<CNFClause> Clauses { get; }

        /// <summary>
        /// Sentence visitor that constructs a set of <see cref="CNFClause"/> objects from a <see cref="Sentence"/> in CNF.
        /// </summary>
        private class ConstructionVisitor : RecursiveSentenceVisitor
        {
            private readonly ICollection<CNFClause> clauses = new List<CNFClause>();

            public static IEnumerable<CNFClause> GetClauses(Sentence sentence)
            {
                var visitor = new ConstructionVisitor();
                visitor.Visit(CNFConversion.ApplyTo(sentence));
                return visitor.clauses;
            }

            /// <inheritdoc />
            public override void Visit(Sentence sentence)
            {
                if (sentence is Conjunction conjunction)
                {
                    // The expression is already in CNF - so the root down until the individual clauses will all be Conjunctions - we just skip past those.
                    Visit(conjunction);
                }
                else
                {
                    // We've hit a clause.
                    // Afterwards, we don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor that
                    // we invoke here does so to figure out the details of the clause). So we can just return rather than invoking base.Visit.
                    clauses.Add(new CNFClause(sentence));
                }
            }
        }
    }
}
