using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using System.Text;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// Query implementation that doesn't recurse for conjuncts.
    /// <para/>
    /// Problematic because it doesn't recurse across conjuncts - meaning that we don't try out different following conjunct bindings
    /// for each possible binding for a given conjunct. No test case that articulates this at the time of writing (TODO! this explanation doesn't suffice..) -
    /// but note the extra binding (unused) that appears in some of the tests..
    /// </summary>
    public class SimpleBackwardChainingQuery_LimitedRecursion : IQuery
    {
        private readonly Predicate goal;
        private readonly IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol;
        private IEnumerable<Tree>? proofs;

        internal SimpleBackwardChainingQuery_LimitedRecursion(Predicate goal, IReadOnlyDictionary<object, List<CNFDefiniteClause>> clausesByConsequentSymbol)
        {
            this.goal = goal;
            this.clausesByConsequentSymbol = clausesByConsequentSymbol;
        }

        /// <inheritdoc />
        public bool IsComplete => proofs != null;

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
                var proofExplanation = new StringBuilder();

                var formatter = new SentenceFormatter();
                var cnfExplainer = new CNFExplainer(formatter);

                // Now build the explanation string.
                var proofStepsByPredicate = Tree.Flatten(proofs);
                var orderedPredicates = proofStepsByPredicate.Keys.ToList();

                for (var i = 0; i < proofStepsByPredicate.Count; i++)
                {
                    var predicate = orderedPredicates[i];
                    var proofSteps = proofStepsByPredicate[predicate];

                    string GetSource(Predicate predicate)
                    {
                        if (orderedPredicates.Contains(predicate))
                        {
                            return $"#{orderedPredicates.IndexOf(predicate):D2}";
                        }
                        else
                        {
                            return " KB";
                        }
                    }

                    proofExplanation.AppendLine($"#{i:D2}: {formatter.Format(predicate)}");

                    foreach (var proofStep in proofSteps)
                    {
                        proofExplanation.AppendLine($"     By Rule : {proofStep.Rule.Format(formatter)}");

                        foreach (var childPredicate in proofStep.SubTrees.Keys)
                        {
                            proofExplanation.AppendLine($"       From {GetSource(childPredicate)}: {formatter.Format(childPredicate)}");
                        }

                        proofExplanation.Append("       Using   : {");
                        proofExplanation.Append(string.Join(", ", proofStep.Unifier.Bindings.Select(s => $"{formatter.Format(s.Key)}/{formatter.Format(s.Value)}")));
                        proofExplanation.AppendLine("}");

                        // TODO: UGH, awful. More type fluidity.
                        var normalisationTermsToExplain = CNFExplainer.FindNormalisationTerms(
                            proofStep.SubTrees.Keys.Select(p => new CNFClause(new CNFLiteral[] { p }))
                                .Append(new CNFClause(new CNFLiteral[] { predicate })).ToArray());

                        foreach (var term in normalisationTermsToExplain) 
                        {
                            proofExplanation.AppendLine($"       ..where {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                        }

                        foreach (var term in proofStep.Unifier.Bindings.SelectMany(kvp => new[] { kvp.Key, kvp.Value }).Where(t => t is VariableReference vr && vr.Symbol is StandardisedVariableSymbol && !normalisationTermsToExplain.Contains(t)))
                        {
                            proofExplanation.AppendLine($"       ..where {formatter.Format(term)} is {cnfExplainer.ExplainNormalisationTerm(term)}");
                        }
                    }

                    proofExplanation.AppendLine();
                }

                return proofExplanation.ToString();
            }
        }

        /// <summary>
        /// Gets the proof trees (or rather, the roots of each) generated during execution of the query.
        /// </summary>
        public IEnumerable<Tree> Proofs => proofs ?? throw new InvalidOperationException("Query is not yet complete");

        /// <inheritdoc />
        public void Dispose()
        {
        }

        /// <inheritdoc />
        public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            proofs = VisitPredicate(goal, Path.Empty, new VariableSubstitution(), cancellationToken);
            return Task.FromResult(Result); // Would be nice to make this async-y at some point - even if just via task.run (but better via iterator method - though IsComplete then becomes a tricky concept..).
        }

        private IEnumerable<Tree> VisitPredicate(Predicate predicate, Path path, VariableSubstitution unifier, CancellationToken ct)
        {
            ct.ThrowIfCancellationRequested();

            List<Tree> trees = new();

            if (path.Contains(predicate))
            {
                return trees;
            }

            path = path.Prepend(predicate);

            if (clausesByConsequentSymbol.TryGetValue(predicate.Symbol, out var clausesWithMatchingConsequentSymbol))
            {
                foreach (var clause in clausesWithMatchingConsequentSymbol)
                {
                    if (LiteralUnifier.TryUpdate(clause.Consequent, predicate, unifier))
                    {
                        var subTrees = VisitRule(clause, path, unifier, ct);
                        if (subTrees != null)
                        {
                            trees.Add(new Tree(predicate, clause, unifier, subTrees));
                        }
                    }
                }
            }

            return trees;
        }

        private IReadOnlyDictionary<Predicate, IEnumerable<Tree>>? VisitRule(CNFDefiniteClause rule, Path path, VariableSubstitution unifier, CancellationToken ct)
        {
            unifier = new VariableSubstitution(unifier);
            var subTrees = new Dictionary<Predicate, IEnumerable<Tree>>();

            foreach (var conjunct in rule.Conjuncts)
            {
                var unifiedConjunct = unifier.ApplyTo(conjunct).Predicate;
                var outcome = VisitPredicate(unifiedConjunct, path, unifier, ct);

                if (!outcome.Any())
                {
                    return null; // means failure (as opposed to empty dictionary, which indicates that a target node is reached).
                }

                subTrees[unifiedConjunct] = outcome;
            }

            return subTrees;
        }

        public class Tree
        {
            internal Tree(Predicate consequent, CNFDefiniteClause rule, VariableSubstitution unifier, IReadOnlyDictionary<Predicate, IEnumerable<Tree>> subTreesByConjunct)
            {
                Consequent = consequent;
                Rule = rule ?? throw new ArgumentNullException(nameof(rule));
                Unifier = unifier;
                SubTrees = subTreesByConjunct ?? throw new ArgumentNullException(nameof(subTreesByConjunct));
            }

            /// <summary>
            /// Gets the consequent proved by the step.
            /// </summary>
            public Predicate Consequent { get; }

            /// <summary>
            /// Gets the root edge of the tree.
            /// </summary>
            public CNFDefiniteClause Rule { get; }

            /// <summary>
            /// Gets the unifier substitution applied to the consequent of the rule to give what we are looking to prove.
            /// </summary>
            public VariableSubstitution Unifier { get; }

            /// <summary>
            /// Gets the sub-trees that follow the root edge, keyed by node that they connect from. There will be more than one if the root edge actually represents a set of more than one coinjoined ("and") edges.
            /// </summary>
            public IReadOnlyDictionary<Predicate, IEnumerable<Tree>> SubTrees { get; }

            /// <summary>
            /// Flattens the tree out into a single mapping from the current node to the edge that should be followed to ultimately reach only target nodes.
            /// Intended to make trees easier to work with in certain situations (e.g. assertions in tests).
            /// <para/>
            /// Each node will occur at most once in the entire tree, so we can always safely do this.
            /// </summary>
            /// <returns>A mapping from the current node to the edge that should be followed to ultimately reach only target nodes.</returns>
            public static IReadOnlyDictionary<Predicate, IEnumerable<Tree>> Flatten(IEnumerable<Tree> trees)
            {
                var flattened = new Dictionary<Predicate, IEnumerable<Tree>>();

                void Visit(Predicate predicate, IEnumerable<Tree> trees)
                {
                    if (!flattened.ContainsKey(predicate))
                    {
                        flattened[predicate] = Enumerable.Empty<Tree>();
                    }

                    flattened[predicate] = flattened[predicate].Concat(trees);
                    foreach (var tree in trees)
                    {
                        foreach (var kvp in tree.SubTrees)
                        {
                            Visit(kvp.Key, kvp.Value);
                        }
                    }
                }

                foreach (var tree in trees)
                {
                    Visit(tree.Consequent, new[] { tree });
                }

                return flattened;
            }
        }

        private class Path
        {
            private Path(Predicate first, Path rest) => (First, Rest) = (first, rest);

            public static Path Empty { get; } = new Path(default, null);

            public Predicate First { get; }

            public Path Rest { get; }

            public Path Prepend(Predicate predicate) => new Path(predicate, this);

            public bool Contains(Predicate predicate) => (First?.Equals(predicate) ?? false) || (Rest?.Contains(predicate) ?? false);
        }
    }
}
