using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Query implementation used by <see cref="SimpleBackwardChainingKnowledgeBase"/>.
    /// </summary>
    public class SimpleBackwardChainingQuery : IQuery
    {
        private readonly Predicate query;
        private readonly IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol;
        ////private readonly Dictionary<Predicate, ProofStep> proof = new();

        private IEnumerable<VariableSubstitution>? substitutions;

        internal SimpleBackwardChainingQuery(Predicate query, IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol)
        {
            this.query = query;
            this.clausesByConsequentSymbol = clausesByConsequentSymbol;
        }

        /// <inheritdoc />
        public bool IsComplete => substitutions != null;

        /// <inheritdoc />
        public bool Result => substitutions?.Any() ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets a human-readable explanation of the query result.
        /// </summary>
        public string ResultExplanation
        {
            get
            {
                // Don't bother lazy.. Won't be critical path - not worth the complexity hit. Might revisit.
                var formatter = new SentenceFormatter();
                var stringBuilder = new StringBuilder();

                foreach (var substitution in Substitutions)
                {
                    stringBuilder.AppendLine(string.Join(", ", substitution.Bindings.Select(kvp => $"{formatter.Format(kvp.Key)}: {formatter.Format(kvp.Value)}")));
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets the proof tree generated during execution of the query.
        /// </summary>
        ////public IReadOnlyDictionary<Predicate, ProofStep> Proof => proof;

        /// <summary>
        /// Gets the set of variable substitutions that can be made to satisfy the query.
        /// Result will be empty if and only if the query returned a negative result.
        /// </summary>
        public IEnumerable<VariableSubstitution> Substitutions => substitutions ?? throw new InvalidOperationException("Query is not yet complete");

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            substitutions = VisitPredicate(query, new VariableSubstitution());
            return Task.Run(() => Result, cancellationToken);
        }

        /// <summary>
        /// Attempt to verify the truth of a predicate by (recursively) attempting to verify the truth of at least one of the clauses for which it is a consequent -
        /// making only variable substitutions that do not conflict with substitutions already made.
        /// </summary>
        /// <param name="goal">The predicate in question.</param>
        /// <param name="unifier">The current unifier.</param>
        /// <returns></returns>
        private IEnumerable<VariableSubstitution> VisitPredicate(Predicate goal, VariableSubstitution unifier)
        {
            if (clausesByConsequentSymbol.TryGetValue(goal.Symbol, out var clausesWithThisGoal))
            {
                foreach (var clause in clausesWithThisGoal)
                {
                    var updatedUnifier = new VariableSubstitution(unifier);

                    if (LiteralUnifier.TryUpdate(clause.Consequent, goal, updatedUnifier))
                    {
                        foreach (var result in VisitConjuncts(clause.Conjuncts, updatedUnifier))
                        {
                            yield return result;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to verify the truth of all a set of conjoined predicates in a particular clause by (recursively) attempting to verify each conjunct -
        /// making only variable substitutions that do not conflict with substitutions already made.
        /// </summary>
        /// <param name="conjuncts">The conjuncts in question.</param>
        /// <param name="unifier">The current unifier.</param>
        /// <returns></returns>
        private IEnumerable<VariableSubstitution> VisitConjuncts(IEnumerable<Predicate> conjuncts, VariableSubstitution unifier)
        {
            if (!conjuncts.Any())
            {
                yield return unifier;
            }
            else
            {
                foreach (var firstPredicateResult in VisitPredicate(unifier.ApplyTo(conjuncts.First()).Predicate, unifier))
                {
                    foreach (var result in VisitConjuncts(conjuncts.Skip(1), firstPredicateResult))
                    {
                        yield return result;
                    }
                }
            }
        }

        /// <summary>
        /// Container for an attempt to apply a specific rule from the knowledge base, given what we've already discerned.
        /// </summary>
        ////public class ProofStep
        ////{
        ////    internal ProofStep(Predicate consequent)
        ////    {
        ////        //Rule = rule;
        ////        KnownPredicates = Enumerable.Empty<Predicate>();
        ////        Unifier = new VariableSubstitution();
        ////    }

        ////    /// <summary>
        ////    /// Extends a proof step with an additional 
        ////    /// </summary>
        ////    /// <param name="parent">The existing proof step.</param>
        ////    /// <param name="predicate">The predicate to add.</param>
        ////    /// <param name="unifier">The updated unifier.</param>
        ////    internal ProofStep(ProofStep parent, Predicate predicate, VariableSubstitution unifier)
        ////    {
        ////        Rule = parent.Rule;
        ////        KnownPredicates = parent.KnownPredicates.Append(predicate); // Hmm. Nesting.. Though we can probably realise it lazily, given the usage.
        ////        Unifier = unifier;
        ////    }

        ////    /// <summary>
        ////    /// The rule that was applied by this step.
        ////    /// </summary>
        ////    public CNFDefiniteClause Rule { get; }

        ////    /// <summary>
        ////    /// The known unit clauses that were used to make this step.
        ////    /// </summary>
        ////    public IEnumerable<Predicate> KnownPredicates { get; }

        ////    /// <summary>
        ////    /// The substitution that is applied to the rules conjuncts to make the match the known predicates.
        ////    /// <para/>
        ////    /// TODO: Make VariableSubstitution readonly, and add MutableVariableSubstitution.
        ////    /// </summary>
        ////    public VariableSubstitution Unifier { get; }

        ////    /// <summary>
        ////    /// Gets the predicate that was inferred by this step by the application of the rule to the known unit clauses.
        ////    /// </summary>
        ////    public Predicate Consequent { get; }
        ////}
    }
}
