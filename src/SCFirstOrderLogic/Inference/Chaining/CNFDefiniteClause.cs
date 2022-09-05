using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Sub-type of <see cref="CNFClause"/> that adds methods and properties appropriate for definite clauses.
    /// <para/>
    /// A useful class, no doubt, and one that at some point might get "promoted" to live in the SentenceManipulation namespace
    /// alongside the other CNF representation types. HOWEVER, not 100% sure I'm happy with it just yet, so leaving it here for now.
    /// </summary>
    public class CNFDefiniteClause : CNFClause
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CNFDefiniteClause"/> that is a copy of an existing <see cref="CNFClause"/>.
        /// </summary>
        /// <param name="definiteClause">The definite clause.</param>
        public CNFDefiniteClause(CNFClause definiteClause)
            : base(definiteClause.Literals)
        {
            if (!definiteClause.IsDefiniteClause)
            {
                throw new ArgumentException("Provided clause must be a definite clause", nameof(definiteClause));
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CNFDefiniteClause"/> that is just a unit clause.
        /// </summary>
        /// <param name="predicate">The sole predicate of the unit clause.</param>
        public CNFDefiniteClause(Predicate predicate)
            : base(predicate)
        {
        }

        /// <summary>
        /// Gets the consequent of this clause (that is, the Q in P₁ ∧ P₂ ∧ .. ∧ Pₙ ⇒ Q).
        /// </summary>
        public Predicate Consequent => Literals.Single(l => l.IsPositive).Predicate;

        /// <summary>
        /// Gets the conjuncts that combine to form the antecedent of this clause (that is, the P₁, .. Pₙ in P₁ ∧ P₂ ∧ .. ∧ Pₙ ⇒ Q).
        /// </summary>
        public IEnumerable<Predicate> Conjuncts => Literals.Where(l => l.IsNegated).Select(l => l.Predicate);

        // TODO: Messy. Add a Format(CNFDefiniteClause) method to SentenceFormatter. BUT only if we end up moving this class into SentenceManipulation..
        // Otherwise, do something else..
        public string Format(SentenceFormatter formatter)
        {
            if (IsUnitClause)
            {
                return formatter.Format(Consequent);
            }
            else
            {
                return $"{string.Join(" ∧ ", Conjuncts.Select(c => formatter.Format(c)))} ⇒ {formatter.Format(Consequent)}";
            }
        }
    }
}
