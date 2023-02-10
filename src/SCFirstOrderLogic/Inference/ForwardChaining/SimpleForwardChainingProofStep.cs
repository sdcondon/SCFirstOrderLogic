using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// Container for information about an attempt to apply a specific rule from the knowledge base, given what we've already discerned.
    /// </summary>
    public class SimpleForwardChainingProofStep
    {
        internal SimpleForwardChainingProofStep(CNFDefiniteClause rule)
        {
            Rule = rule;
            KnownPredicates = Enumerable.Empty<Predicate>();
            Unifier = new VariableSubstitution();
        }

        /// <summary>
        /// Extends a proof step with an additional predicate and updated unifier.
        /// </summary>
        /// <param name="parent">The existing proof step.</param>
        /// <param name="additionalPredicate">The predicate to add.</param>
        /// <param name="updatedUnifier">The updated unifier.</param>
        internal SimpleForwardChainingProofStep(
            SimpleForwardChainingProofStep parent,
            Predicate additionalPredicate,
            VariableSubstitution updatedUnifier)
        {
            Rule = parent.Rule;
            KnownPredicates = parent.KnownPredicates.Append(additionalPredicate); // Hmm. Nesting.. Though we can probably realise it lazily, given the usage.
            Unifier = updatedUnifier;
        }

        /// <summary>
        /// The rule that was applied by this step.
        /// </summary>
        public CNFDefiniteClause Rule { get; }

        /// <summary>
        /// The known predicates that were used to make this step.
        /// </summary>
        public IEnumerable<Predicate> KnownPredicates { get; }

        /// <summary>
        /// The substitution that is applied to the rule's conjuncts to make them match the known predicates.
        /// </summary>
        public VariableSubstitution Unifier { get; }

        /// <summary>
        /// Gets the predicate that was inferred by this step by the application of the rule to the known unit clauses.
        /// </summary>
        public Predicate InferredPredicate => Unifier.ApplyTo(Rule.Consequent).Predicate;
    }
}
