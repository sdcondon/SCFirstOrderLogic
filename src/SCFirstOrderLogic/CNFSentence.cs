using System.Collections.Generic;
using System.Linq;
using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a <see cref="Sentence"/> in conjunctive normal form (CNF).
    /// </summary>
    public sealed class CNFSentence
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFSentence"/> class, implicitly converting the provided sentence to CNF in the process.
        /// </summary>
        /// <param name="sentence">The sentence to (convert and) represent.</param>
        internal CNFSentence(Sentence sentence)
        {
            var cnfSentence = CNFConversion.ApplyTo(sentence);
            var clauses = new List<CNFClause>();
            new CNFClauseFinder(clauses).Visit(cnfSentence);
            // BUG: Potential equality bug on hash code collision..
            Clauses = clauses.OrderBy(c => c.GetHashCode()).ToArray();
        }

        /// <summary>
        /// Gets the collection of clauses that comprise this CNF sentence.
        /// </summary>
        public IReadOnlyCollection<CNFClause> Clauses { get; }

        /// <summary>
        /// Defines the (implicit) conversion of a <see cref="Sentence"/> instance to a <see cref="CNFSentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to convert.</param>
        /// <remarks>
        /// I'm still not 100% happy with exactly how the CNFSentence / Sentence dichotomy is handled. Almost all of the time
        /// we'll be wanting to deal with CNF - but the "raw" sentence tree structure still has value.. This conversion
        /// operator helps, but there's almost certainly more that could be done.
        /// </remarks>
        public static implicit operator CNFSentence(Sentence sentence) => new(sentence);

        /// <summary>
        /// Sentence visitor that constructs a set of <see cref="CNFClause"/> objects from a <see cref="Sentence"/> in CNF.
        /// </summary>
        private class CNFClauseFinder : RecursiveSentenceVisitor
        {
            private readonly ICollection<CNFClause> clauses;

            public CNFClauseFinder(ICollection<CNFClause> clauses) => this.clauses = clauses;

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
