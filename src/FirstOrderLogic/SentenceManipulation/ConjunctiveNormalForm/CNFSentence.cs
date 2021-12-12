using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Representation of a <see cref="Sentence"/> in conjunctive normal form (CNF).
    /// </summary>
    /// <typeparam name="TModel">The type that the literals of this expression refer to.</typeparam>
    public class CNFSentence
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="CNFSentence"/> class, implicitly converting the provided sentence to CNF in the process.
        /// </summary>
        /// <param name="sentence">The sentence to (convert and) represent.</param>
        public CNFSentence(Sentence sentence)
        {
            Sentence = new CNFConversion().ApplyTo(sentence);
            var clauses = new List<CNFClause>();
            new ExpressionConstructor(clauses).ApplyTo(Sentence);
            Clauses = clauses.AsReadOnly();
        }

        /// <summary>
        /// Gets the actual <see cref="Sentence"/> that underlies this representation.
        /// </summary>
        public Sentence Sentence { get; }

        /// <summary>
        /// Gets the collection of clauses that comprise this CNF sentence.
        /// </summary>
        public IReadOnlyCollection<CNFClause> Clauses { get; }

        /// <summary>
        /// Sentence "Transformation" that constructs a set of <see cref="CNFClause"/> objects from a <see cref="Sentence"/> in CNF.
        /// </summary>
        private class ExpressionConstructor : SentenceTransformation
        {
            private readonly List<CNFClause> clauses;

            public ExpressionConstructor(List<CNFClause> clauses) => this.clauses = clauses;

            /// <inheritdoc />
            public override Sentence ApplyTo(Sentence sentence)
            {
                if (sentence is Conjunction)
                {
                    // The expression is already in CNF - so the root down until the individual clauses will all be Conjunctions - we just skip past those.
                    return base.ApplyTo(sentence);
                }
                else
                {
                    // We've hit a clause.
                    clauses.Add(new CNFClause(sentence));

                    // We don't need to look any further down the tree for the purposes of this class (though the CNFClause ctor, above,
                    // does so to figure out the details of the clause). So we can just return sentence rather than invoking base.ApplyTo. 
                    return sentence;
                }
            }
        }
    }
}
