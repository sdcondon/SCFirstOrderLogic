using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Sub-type of <see cref="CNFClause"/> that adds methods and properties appropriate for definite clauses.
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

        /// <summary>
        /// Constructs and returns a clause that is the same as this one, except for the
        /// fact that all variable declarations are replaced with new ones.
        /// </summary>
        /// <returns>
        /// A clause that is the same as this one, except for the fact that all variable
        /// references are replaced with new ones.
        /// </returns>
        public new CNFDefiniteClause Restandardise() => new CNFDefiniteClause(base.Restandardise());
    }
}
