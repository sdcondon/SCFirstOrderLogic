using SCFirstOrderLogic.Inference.Resolution.Utility;
using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// A knowledge base that uses a very simple implementation of resolution to answer queries.
    /// See §9.5 ("Resolution") of 'Artificial Intelligence: A Modern Approach' for a detailed explanation of resolution.
    /// Notes:
    /// <list type="bullet">
    /// <item/>Includes functionality for fine-grained execution and examination of individual steps of queries. Use the <see cref="CreateQuery"/> method.
    /// <item/>Has no in-built handling of equality - so, if equality appears in the knowledge base, its properties need to be axiomised - see §9.5.5 of 'Artifical Intelligence: A Modern Approach'.
    /// <item/>Not thread-safe (i.e. not re-entrant) - despite the fact that resolution is ripe for parallelisation.
    /// </list>
    /// </summary>
    public sealed class SimpleResolutionKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBaseClauseStore clauseStore;
        private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
        private readonly Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison;

        /// <summary>
        /// Initialises a new instance of the <see cref="SimpleResolutionKnowledgeBase"/> class.
        /// </summary>
        /// <param name="clausePairFilter">
        /// A delegate to use to filter the pairs of clauses to be queued for a unification attempt.
        /// A true value indicates that the pair should be enqueued.
        /// </param>
        /// <param name="clausePairPriorityComparer">An object to use to compare the pairs of clauses to be queued for a unification attempt.</param>
        public SimpleResolutionKnowledgeBase(IKnowledgeBaseClauseStore clauseStore, Func<(CNFClause, CNFClause), bool> clausePairFilter, Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison)
        {
            this.clauseStore = clauseStore;
            this.clausePairFilter = clausePairFilter; 
            this.clausePairPriorityComparison = clausePairPriorityComparison;
        }

        /// <inheritdoc />
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            foreach(var clause in new CNFSentence(sentence).Clauses)
            {
                await clauseStore.AddAsync(clause, cancellationToken);
            }
        }

        /// <inheritdoc />
        public async Task<bool> AskAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            var query = await CreateQueryAsync(sentence);
            return await query.CompleteAsync(cancellationToken);
        }

        /// <summary>
        /// Creates an <see cref="IResolutionQuery"/> instance for fine-grained execution and examination of a query.
        /// </summary>
        /// <param name="sentence">The query sentence.</param>
        /// <returns>An <see cref="IResolutionQuery"/> instance that can be used to execute and examine the query.</returns>
        public async Task<IResolutionQuery> CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            return await Query.CreateAsync(clauseStore, clausePairFilter, clausePairPriorityComparison, sentence, cancellationToken);
        }

        /// <remark>
        /// The only real reason not to make this public (probably called SimpleResolutionQuery or somesuch) is the async initialisation
        /// issue. I don't want to have to check that its been initialised on each and every step invocation, and don't want to make
        /// construction long-running..
        /// </remarks>
        private class Query : IResolutionQuery
        {
            private readonly IQueryClauseStore clauseStore;
            private readonly Func<(CNFClause, CNFClause), bool> clausePairFilter;
            private readonly MaxPriorityQueue<(CNFClause, CNFClause, VariableSubstitution, CNFClause)> queue;
            private readonly Dictionary<CNFClause, (CNFClause, CNFClause, VariableSubstitution)> steps;

            private bool result;

            private Query(
                IKnowledgeBaseClauseStore clauseStore,
                Func<(CNFClause, CNFClause), bool> clausePairFilter,
                Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison,
                Sentence query)
            {
                this.clauseStore = clauseStore.CreateQueryClauseStore();
                this.clausePairFilter = clausePairFilter;
                queue = new MaxPriorityQueue<(CNFClause, CNFClause, VariableSubstitution, CNFClause)>((x, y) =>
                {
                    return clausePairPriorityComparison((x.Item1, x.Item2), (y.Item1, y.Item2));  // TODO: ugh, hideous (and perhaps slow - test me)
                });
                steps = new Dictionary<CNFClause, (CNFClause, CNFClause, VariableSubstitution)>();

                NegatedQuery = new CNFSentence(new Negation(query));
            }

            /// <summary>
            /// Gets the (CNF representation of) the negation of the query.
            /// </summary>
            public CNFSentence NegatedQuery { get; }

            /// <inheritdoc/>
            public bool IsComplete { get; private set; } = false;

            /// <inheritdoc/>
            public bool Result
            {
                get
                {
                    if (!IsComplete)
                    {
                        throw new InvalidOperationException("Query is not yet complete");
                    }

                    return result;
                }
            }

            /// <inheritdoc/>
            public IReadOnlyDictionary<CNFClause, (CNFClause, CNFClause, VariableSubstitution)> Steps => steps;

            /// <summary>
            /// Creates and initialises a new instance of the <see cref="Query"/> class.
            /// Initialisation can potentially be a long-running operation (and long-running constructors are a bad idea) -
            /// hence this method exists and the constructor is private.
            /// </summary>
            /// <returns>A new query.</returns>
            public static async Task<Query> CreateAsync(
                IKnowledgeBaseClauseStore clauseStore,
                Func<(CNFClause, CNFClause), bool> clausePairFilter,
                Comparison<(CNFClause, CNFClause)> clausePairPriorityComparison,
                Sentence querySentence,
                CancellationToken cancellationToken = default)
            {
                var query = new Query(clauseStore, clausePairFilter, clausePairPriorityComparison, querySentence);

                // Initialise the clause store with the clauses from the negation of the query:
                foreach (var clause in query.NegatedQuery.Clauses)
                {
                    await clauseStore.AddAsync(clause, cancellationToken);
                }

                // Queue up all initial clause pairings - adhering to our clause pair filter and priority comparer.
                await foreach (var clause in clauseStore)
                {
                    await query.EnqueueUnfilteredResolventsAsync(clause, cancellationToken);
                }

                return query;
            }

            /// <inheritdoc/>
            public async Task NextStepAsync(CancellationToken cancellationToken = default)
            {
                if (IsComplete)
                {
                    throw new InvalidOperationException("Query is complete");
                }

                // Grab the next resolvent (in addition to the two clauses it originates from and the variable substitution) from the queue..
                var (ci, cj, sub, clause) = queue.Dequeue();
                steps[clause] = (ci, cj, sub);

                // If the resolvent is an empty clause, we've found a contradiction and can thus return a positive result:
                if (clause.Equals(CNFClause.Empty))
                {
                    steps[CNFClause.Empty] = (ci, cj, sub);
                    result = true;
                    IsComplete = true;
                    return;
                }

                // Otherwise, check if we've found a new clause (i.e. something that we didn't know already)..
                // Upside of using Add: Don't need a separate "Contains" method on the store (which would raise potential
                // misunderstandings about what the store means by "contains" - c.f. subsumption..)
                // Downside of using Add: clause store will encounter itself when looking for unifiers - not a big deal,
                // but a performance/maintainability tradeoff nonetheless
                if (clauseStore.AddAsync(clause, cancellationToken).Result)
                {
                    // This is a new clause, so we queue up some more clause pairings -
                    // (combinations of the resolvent and existing known clauses)
                    // adhering to any filtering and ordering we have in place.

                    // ..and iterate through its resolvents (if any) - also make a note of the unifier so that we can include it in the record of steps that we maintain:
                    await EnqueueUnfilteredResolventsAsync(clause, cancellationToken);
                }

                // If we've run out of clauses to smash together, return a negative result.
                if (queue.Count == 0)
                {
                    result = false;
                    IsComplete = true;
                }
            }

            /// <inheritdoc/>
            public async Task<bool> CompleteAsync(CancellationToken cancellationToken = default)
            {
                while (!IsComplete)
                {
                    await NextStepAsync(cancellationToken);
                }
                
                return result;
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                clauseStore.Dispose();
            }

            private async Task EnqueueUnfilteredResolventsAsync(CNFClause clause, CancellationToken cancellationToken = default)
            {
                await foreach (var (otherClause, unifier, unified) in clauseStore.FindUnifiers(clause, cancellationToken))
                {
                    // NB: Throwing away clauses returned by the unifier store has performance impact.
                    // Could instead/also use a store that knows to not look for certain clause pairings in the first place..
                    // However, REQUIRING the store to do this felt a little ugly from a code perspective, since the store is
                    // then a mix of implementation (how unifiers are stored/indexed) and strategy, plus there's a bit more
                    // strategy in the form of the priority comparer. This feels a good compromise - there are of course
                    // alternatives (e.g. some kind of strategy object that encapsulates both) - but they felt like overkill
                    // for this simple implementation.
                    if (clausePairFilter((clause, otherClause)))
                    {
                        queue.Enqueue((clause, otherClause, unifier, unified));
                    }
                }
            }
        }
    }
}
