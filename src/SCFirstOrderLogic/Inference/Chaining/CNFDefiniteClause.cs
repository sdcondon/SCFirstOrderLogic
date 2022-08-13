using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Decorator type that adds methods and properties appropriate for definite clauses.
    /// <para/>
    /// A useful class, no doubt, and one that at some point might get "promoted" to live in the SentenceManipulation namespace
    /// alongside the other CNF representation types. HOWEVER, not quite sure how I want to deal with it just yet, so leaving it
    /// here for now.
    /// </summary>
    public class CNFDefiniteClause : CNFClause
    {
        private readonly CNFClause clause;

        /// <summary>
        /// Initializes a new instance of the <see cref="CNFDefiniteClause"/> that refers to a given existing definite <see cref="CNFClause"/>.
        /// </summary>
        /// <param name="definiteClause">The definite clause.</param>
        public CNFDefiniteClause(CNFClause definiteClause)
            : base(definiteClause.Literals)
        {
            if (!definiteClause.IsDefiniteClause)
            {
                throw new ArgumentException("Provided clause must be a definite clause", nameof(definiteClause));
            }

            this.clause = definiteClause;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CNFDefiniteClause"/> that is just a unit clause.
        /// </summary>
        /// <param name="predicate">The predicate of the unit clause.</param>
        public CNFDefiniteClause(Predicate predicate)
            : base(predicate)
        {
            this.clause = new CNFClause(new CNFLiteral[] { predicate });
        }

        /// <summary>
        /// Gets the consequent of this clause (that is, the Q in P₁ ∧ P₂ ∧ .. ∧ Pₙ ⇒ Q).
        /// </summary>
        public Predicate Consequent => clause.Literals.Single(l => l.IsPositive).Predicate;

        /// <summary>
        /// Gets the conjuncts that combine to form the antecedent of this clause (that is, the P₁, .. Pₙ in P₁ ∧ P₂ ∧ .. ∧ Pₙ ⇒ Q).
        /// </summary>
        public IEnumerable<Predicate> Conjuncts => clause.Literals.Where(l => l.IsNegated).Select(l => l.Predicate);

        /// <summary>
        /// Gets a value indicating whether this is a unit clause (that is, is just a predicate).
        /// </summary>
        public bool IsUnitClause => clause.IsUnitClause;

        /// <summary>
        /// Checks whether this clause unifies with any of an enumeration of other definite clauses.
        /// <para/>
        /// NB: this logic is not specific to definite clauses - so perhaps belongs elsewhere?
        /// </summary>
        /// <param name="clauses">The clauses to check for unification with.</param>
        /// <returns>True if this clause unifies with any of the provided clauses; otherwise false.</returns>
        public bool UnifiesWithAnyOf(IEnumerable<CNFDefiniteClause> clauses)
        {
            return clauses.Any(c => TryUnify(c, this, out var _));
        }

        // NB: not specific to definite clauses - so perhaps belongs elsewhere?
        private static bool TryUnify(CNFDefiniteClause clause1, CNFDefiniteClause clause2, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
        {
            if (clause1.clause.Literals.Count != clause2.clause.Literals.Count)
            {
                unifier = null;
                return false;
            }

            unifier = new VariableSubstitution();

            foreach (var (literal1, literal2) in clause1.clause.Literals.Zip(clause2.clause.Literals))
            {
                if (!LiteralUnifier.TryUpdate(literal1, literal2, unifier))
                {
                    unifier = null;
                    return false;
                }
            }

            return true;
        }
    }
}
