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
    /// <para/>
    /// Problematic because it doesn't recurse across conjuncts - meaning that we don't try out different following conjunct bindings
    /// for each possible solution for a given conjunct. No test case that articulates this at the time of writing -
    /// but note the extra binding (unused) that appears in some of the tests..
    /// </summary>
    public class SimpleBackwardChainingQuery : IQuery
    {
        private readonly Predicate query;
        private readonly IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol;

        private IEnumerable<Proof>? proofs;

        internal SimpleBackwardChainingQuery(Predicate query, IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol)
        {
            this.query = query;
            this.clausesByConsequentSymbol = clausesByConsequentSymbol;
        }

        /// <inheritdoc />
        public bool IsComplete => proofs != null; // TODO: BAD - will be immediately true.

        /// <inheritdoc />
        public bool Result => proofs?.Any() ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets a human-readable explanation of the query result.
        /// </summary>
        public string ResultExplanation
        {
            get
            {
                // Don't bother lazy.. Won't be critical path - not worth the complexity hit. Might revisit.
                var formatter = new SentenceFormatter();
                var cnfExplainer = new CNFExplainer(formatter);
                var resultExplanation = new StringBuilder();

                var proofNum = 1;
                foreach (var proof in Proofs)
                {
                    resultExplanation.AppendLine($"--- PROOF #{proofNum++}");
                    resultExplanation.AppendLine();

                    // Now build the explanation string.
                    var proofStepsByPredicate = proof.Steps;
                    var orderedPredicates = proofStepsByPredicate.Keys.ToList();

                    for (var i = 0; i < orderedPredicates.Count; i++)
                    {
                        var predicate = orderedPredicates[i];
                        var proofStep = proofStepsByPredicate[predicate];

                        // Consequent:
                        resultExplanation.AppendLine($"Step #{i:D2}: {formatter.Format(predicate)}");

                        // Rule applied:
                        resultExplanation.AppendLine($"     By Rule : {proofStep.Format(formatter)}");

                        // Conjuncts used:
                        foreach (var childPredicate in proofStep.Conjuncts)
                        {
                            var unifiedChildPredicate = proof.GetUnified(childPredicate);
                            resultExplanation.AppendLine($"     And Step #{orderedPredicates.IndexOf(unifiedChildPredicate):D2}: {formatter.Format(unifiedChildPredicate)}");
                        }

                        resultExplanation.AppendLine();
                    }

                    // Output the unifier:
                    resultExplanation.Append("Using: {");
                    resultExplanation.Append(string.Join(", ", proof.Unifier.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
                    resultExplanation.AppendLine("}");
                    resultExplanation.AppendLine();

                    // Explain all the normalisation terms (standardised variables and skolem functions)
                    resultExplanation.AppendLine("Where:");
                    var normalisationTermsToExplain = new HashSet<Term>();

                    foreach (var term in CNFExplainer.FindNormalisationTerms(proof.Steps.Keys.Select(p => new CNFClause(new CNFLiteral[] { p })).ToArray()))
                    {
                        normalisationTermsToExplain.Add(term);
                    }

                    foreach (var term in proof.Unifier.Bindings.SelectMany(kvp => new[] { kvp.Key, kvp.Value }).Where(t => t is VariableReference vr && vr.Symbol is StandardisedVariableSymbol))
                    {
                        normalisationTermsToExplain.Add(term);
                    }

                    foreach (var term in normalisationTermsToExplain)
                    {
                        resultExplanation.AppendLine($"       {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                    }

                    resultExplanation.AppendLine();
                }

                return resultExplanation.ToString();
            }
        }

        /// <summary>
        /// Gets the set of proofs of the query..
        /// Result will be empty if and only if the query returned a negative result.
        /// </summary>
        public IEnumerable<Proof> Proofs => proofs ?? throw new InvalidOperationException("Query is not yet complete");

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            proofs = ProvePredicate(query, new Proof());
            return Task.FromResult(Result);
        }

        private IEnumerable<Proof> ProvePredicate(Predicate goal, Proof parentProof)
        {
            if (clausesByConsequentSymbol.TryGetValue(goal.Symbol, out var clausesWithThisGoal))
            {
                foreach (var clause in clausesWithThisGoal)
                {
                    var restandardisedClause = clause.Restandardize();
                    var clauseProofPrototype = new Proof(parentProof);

                    if (LiteralUnifier.TryUpdate(restandardisedClause.Consequent, goal, clauseProofPrototype.Unifier))
                    {
                        foreach (var clauseProof in ProvePredicates(restandardisedClause.Conjuncts, clauseProofPrototype))
                        {
                            clauseProof.AddStep(clauseProof.ApplyUnifierTo(goal), restandardisedClause);
                            yield return clauseProof;
                        }
                    }
                }
            }
        }

        private IEnumerable<Proof> ProvePredicates(IEnumerable<Predicate> goals, Proof proof)
        {
            if (!goals.Any())
            {
                yield return proof;
            }
            else
            {
                foreach (var firstConjunctProof in ProvePredicate(proof.ApplyUnifierTo(goals.First()), proof))
                {
                    foreach (var restOfConjunctsProof in ProvePredicates(goals.Skip(1), firstConjunctProof))
                    {
                        yield return restOfConjunctsProof;
                    }
                }
            }
        }

        public class Proof
        {
            private readonly Dictionary<Predicate, CNFDefiniteClause> steps;

            internal Proof()
            {
                Unifier = new VariableSubstitution();
                steps = new();
            }

            internal Proof(Proof parent)
            {
                Unifier = new VariableSubstitution(parent.Unifier);
                steps = parent.Steps.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }

            public VariableSubstitution Unifier { get; }

            public IReadOnlyDictionary<Predicate, CNFDefiniteClause> Steps => steps;

            public Predicate GetUnified(Predicate predicate)
            {
                // NB: the algorithm builds up the unifier step-by-step, so that it can (will probably) result in many
                // terms being bound indirectly. E.g. variable A is bound to variable B, which is in turn bound to constant C.
                // There's no sense in making the algorithm itself more complex (and thus slower) than it needs to be to streamline
                // this, but for the readability of explanations, we make sure to follw the chain to the end by applying the
                // unifier until it doesn't make any more changes. The "full story" remains viewable in the unifier.. 
                var newPredicate = ApplyUnifierTo(predicate);
                while (!newPredicate.Equals(predicate))
                {
                    predicate = newPredicate;
                    newPredicate = ApplyUnifierTo(predicate);
                }

                return predicate;
            }

            internal Predicate ApplyUnifierTo(Predicate predicate) => Unifier.ApplyTo(predicate).Predicate;

            internal void AddStep(Predicate predicate, CNFDefiniteClause rule)
            {
                steps[predicate] = rule;
            }
        }

        ////private class Path
        ////{
        ////    private Path(Predicate first, Path rest) => (First, Rest) = (first, rest);

        ////    public static Path Empty { get; } = new Path(default, null);

        ////    public Predicate First { get; }

        ////    public Path Rest { get; }

        ////    public Path Prepend(Predicate predicate) => new Path(predicate, this);

        ////    public bool Contains(Predicate predicate) => (First?.Equals(predicate) ?? false) || (Rest?.Contains(predicate) ?? false);
        ////}
    }
}
