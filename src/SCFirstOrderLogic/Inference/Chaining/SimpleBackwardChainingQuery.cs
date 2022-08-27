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
        public bool IsComplete => proofs != null; // BAD - will be immediately true.

        /// <inheritdoc />
        public bool Result => proofs?.Any() ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Returns a human-readable explanation of the query result.
        /// </summary>
        /// <returns>A human-readable explanation of the query result</returns>
        public string ResultExplanation
        {
            get
            {
                var formatter = new SentenceFormatter();
                var stringBuilder = new StringBuilder();

                foreach (var proof in Proofs)
                {
                    stringBuilder.AppendLine(string.Join(", ", proof.Substitution.Bindings.Select(kvp => $"{formatter.Format(kvp.Key)}: {formatter.Format(kvp.Value)}")));
                }

                return stringBuilder.ToString();
            }
        }

        /// <summary>
        /// Gets the set of variable substitutions that can be made to satisfy the query.
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
            proofs = ProvePredicate(query, new Proof(new VariableSubstitution()));
            return Task.FromResult(Result);
        }

        private IEnumerable<Proof> ProvePredicate(Predicate goal, Proof proof)
        {
            if (clausesByConsequentSymbol.TryGetValue(goal.Symbol, out var clausesWithThisGoal))
            {
                foreach (var clause in clausesWithThisGoal)
                {
                    var clauseProof = new Proof(new VariableSubstitution(proof.Substitution));

                    if (LiteralUnifier.TryUpdateUnsafe(clause.Consequent, goal, clauseProof.Substitution))
                    {
                        foreach (var θ2 in ProvePredicates(clause.Conjuncts, clauseProof))
                        {
                            yield return θ2;
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
                foreach (var firstConjunctProof in ProvePredicate(proof.Substitution.ApplyTo(goals.First()).Predicate, proof))
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
            public Proof(VariableSubstitution substitution)
            {
                Substitution = substitution;
            }

            public VariableSubstitution Substitution { get; } 
        }

        ////public class Tree
        ////{
        ////    internal Tree(Predicate consequent, CNFDefiniteClause rule, VariableSubstitution unifier, IReadOnlyDictionary<Predicate, IEnumerable<Tree>> subTreesByConjunct)
        ////    {
        ////        Consequent = consequent;
        ////        Rule = rule ?? throw new ArgumentNullException(nameof(rule));
        ////        Unifier = unifier;
        ////        SubTrees = subTreesByConjunct ?? throw new ArgumentNullException(nameof(subTreesByConjunct));
        ////    }

        ////    /// <summary>
        ////    /// Gets the consequent proved by the step.
        ////    /// </summary>
        ////    public Predicate Consequent { get; }

        ////    /// <summary>
        ////    /// Gets the root edge of the tree.
        ////    /// </summary>
        ////    public CNFDefiniteClause Rule { get; }

        ////    /// <summary>
        ////    /// Gets the unifier substitution applied to the consequent of the rule to give what we are looking to prove.
        ////    /// </summary>
        ////    public VariableSubstitution Unifier { get; }

        ////    /// <summary>
        ////    /// Gets the sub-trees that follow the root edge, keyed by node that they connect from. There will be more than one if the root edge actually represents a set of more than one coinjoined ("and") edges.
        ////    /// </summary>
        ////    public IReadOnlyDictionary<Predicate, IEnumerable<Tree>> SubTrees { get; }

        ////    /// <summary>
        ////    /// Flattens the tree out into a single mapping from the current node to the edge that should be followed to ultimately reach only target nodes.
        ////    /// Intended to make trees easier to work with in certain situations (e.g. assertions in tests).
        ////    /// <para/>
        ////    /// Each node will occur at most once in the entire tree, so we can always safely do this.
        ////    /// </summary>
        ////    /// <returns>A mapping from the current node to the edge that should be followed to ultimately reach only target nodes.</returns>
        ////    public static IReadOnlyDictionary<Predicate, IEnumerable<Tree>> Flatten(IEnumerable<Tree> trees)
        ////    {
        ////        var flattened = new Dictionary<Predicate, IEnumerable<Tree>>();

        ////        void Visit(Predicate predicate, IEnumerable<Tree> trees)
        ////        {
        ////            if (!flattened.ContainsKey(predicate))
        ////            {
        ////                flattened[predicate] = Enumerable.Empty<Tree>();
        ////            }

        ////            flattened[predicate] = flattened[predicate].Concat(trees);
        ////            foreach (var tree in trees)
        ////            {
        ////                foreach (var kvp in tree.SubTrees)
        ////                {
        ////                    Visit(kvp.Key, kvp.Value);
        ////                }
        ////            }
        ////        }

        ////        foreach (var tree in trees)
        ////        {
        ////            Visit(tree.Consequent, new[] { tree });
        ////        }

        ////        return flattened;
        ////    }
        ////}

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
