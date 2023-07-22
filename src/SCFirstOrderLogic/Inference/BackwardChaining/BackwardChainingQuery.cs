// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic;
using SCFirstOrderLogic.InternalUtilities;
using SCFirstOrderLogic.SentenceFormatting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    /// <summary>
    /// An implementation of <see cref="IQuery"/> that uses a (very basic) depth-first backward chaining algorithm.
    /// Used by <see cref="BackwardChainingKnowledgeBase"/>.
    /// </summary>
    public class BackwardChainingQuery : IQuery
    {
        private readonly Predicate queryGoal;
        private readonly IClauseStore clauseStore;

        private int executeCount = 0;
        private List<BackwardChainingProof>? proofs;

        internal BackwardChainingQuery(Predicate queryGoal, IClauseStore clauseStore)
        {
            this.queryGoal = queryGoal;
            this.clauseStore = clauseStore;
        }

        /// <inheritdoc />
        public bool IsComplete => proofs != null;

        /// <inheritdoc />
        public bool Result => proofs?.Any() ?? throw new InvalidOperationException("Query is not yet complete");

        /// <summary>
        /// Gets a human-readable explanation of the query result.
        /// </summary>
        public string ResultExplanation => GetResultExplanation(new SentenceFormatter());

        /// <summary>
        /// Gets a list of proofs of the query.
        /// Result will be empty if and only if the query returned a negative result.
        /// </summary>
        public IReadOnlyList<BackwardChainingProof> Proofs => proofs ?? throw new InvalidOperationException("Query is not yet complete");

        /// <inheritdoc />
        public async Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
        {
            // ..while it might be nice to allow for other threads to just get the existing task back
            // if its already been started, the possibility of the cancellation token being different
            // makes it awkward. The complexity added by attempting to deal with that simply isn't worth it.
            // So, we just throw if the query is already in progress. Messing about with a query from
            // multiple threads is fairly unlikely anyway (as opposed wanting an individual query to
            // parallelise itself - which is definitely something I want to look at).
            if (Interlocked.Exchange(ref executeCount, 1) == 1)
            {
                throw new InvalidOperationException("Query execution has already begun via a prior ExecuteAsync invocation");
            }

            proofs = await ProvePredicate(queryGoal, new BackwardChainingProof()).ToListAsync(cancellationToken);
            return Result;
        }

        /// <summary>
        /// Gets a human-readable explanation of the query result, using a specified <see cref="SentenceFormatter"/> instance.
        /// </summary>
        /// <param name="sentenceFormatter">The sentence formatter to use.</param>
        public string GetResultExplanation(SentenceFormatter sentenceFormatter)
        {
            var resultExplanation = new StringBuilder();

            for (int i = 0; i < Proofs.Count; i++)
            {
                resultExplanation.AppendLine($"PROOF #{i + 1}:");
                resultExplanation.AppendLine();
                resultExplanation.Append(Proofs[i].GetExplanation(sentenceFormatter));
                resultExplanation.AppendLine();
            }

            return resultExplanation.ToString();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Nothing to do..
            GC.SuppressFinalize(this);
        }

        private async IAsyncEnumerable<BackwardChainingProof> ProvePredicate(Predicate goal, BackwardChainingProof parentProof)
        {
            // NB: This implementation is a basic depth-first and-or search, but the clause store can at least
            // control which branches get explored first by ordering the returned clause applications appropriately.
            await foreach (var (clause, substitution) in clauseStore.GetClauseApplications(goal, parentProof.Unifier))
            {
                await foreach (var clauseProof in ProvePredicates(clause.Conjuncts, new BackwardChainingProof(parentProof.Steps, substitution)))
                {
                    clauseProof.AddStep(clauseProof.ApplyUnifierTo(goal), clause);
                    yield return clauseProof;
                }
            }
        }

        private async IAsyncEnumerable<BackwardChainingProof> ProvePredicates(IEnumerable<Predicate> goals, BackwardChainingProof currentProof)
        {
            if (!goals.Any())
            {
                yield return currentProof;
            }
            else
            {
                await foreach (var firstGoalProof in ProvePredicate(currentProof.ApplyUnifierTo(goals.First()), currentProof))
                {
                    await foreach (var restOfGoalsProof in ProvePredicates(goals.Skip(1), firstGoalProof))
                    {
                        yield return restOfGoalsProof;
                    }
                }
            }
        }
    }
}
