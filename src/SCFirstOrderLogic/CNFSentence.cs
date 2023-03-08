using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a <see cref="Sentence"/> in conjunctive normal form (CNF).
    /// </summary>
    public class CNFSentence : IEquatable<CNFSentence>
    {
        private readonly CNFClause[] clauses;

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
            // TODO-BUG-ROBUSTNESS: Potential equality incorrectness on hash code collision.
            // TODO-BUG-ROBUSTNESS: No handling of being handed an enumerable containing dups.
            // One would hope that ImmutableSortedSet would deal with both. This is quite low-level code,
            // though - need to assess the options for performance cost.
            this.clauses = clauses.OrderBy(c => c.GetHashCode()).ToArray();
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
        // TODO-FEATURE: logically, this should be a set - IReadOnlySet<> or IImmutableSet<> would both be non-breaking.
        // Investigate perf impact of ImmutableSortedSet (sorted to facilitate quick equality comparison, hopefully)?
        public IReadOnlyCollection<CNFClause> Clauses => clauses;

        /// <summary>
        /// <para>
        /// Returns a string that represents the current object.
        /// </para>
        /// <para>
        /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to format the sentence.
        /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
        /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
        /// of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
        /// </para>
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString() => new SentenceFormatter().Format(this);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is CNFSentence sentence && Equals(sentence);

        /// <inheritdoc />
        /// <remarks>
        /// Sentences that contain exactly the same collection of clauses are considered equal.
        /// </remarks>
        public bool Equals(CNFSentence? other)
        {
            if (other == null || clauses.Length != other.clauses.Length)
            {
                return false;
            }

            for (int i = 0; i < clauses.Length; i++)
            {
                if (!clauses[i].Equals(other.clauses[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Sentences that contain exactly the same collection of clauses are considered equal.
        /// </remarks>
        public override int GetHashCode()
        {
            var hash = new HashCode();
            foreach (var clause in Clauses)
            {
                hash.Add(clause);
            }

            return hash.ToHashCode();
        }

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
