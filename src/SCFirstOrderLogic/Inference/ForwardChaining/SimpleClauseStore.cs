using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// A (rather inefficient) implementation of <see cref="IClauseStore"/> that just uses a <see cref="HashSet{T}"/> to store knowledge.
    /// </summary>
    public class SimpleClauseStore : IKnowledgeBaseClauseStore
    {
        private readonly HashSet<CNFDefiniteClause> clauses = new();

        /// <inheritdoc/>
        public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(clauses.Add(clause));
        }

#pragma warning disable CS1998 // async lacks await.. Could add await Task.Yield() to silence this, but it is not worth the overhead.
        /// <inheritdoc />
        public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            foreach (var clause in clauses)
            {
                cancellationToken.ThrowIfCancellationRequested();
                yield return clause;
            }
        }
#pragma warning restore CS1998

        /// <inheritdoc />
        public IQueryClauseStore CreateQueryClauseStore() => new QueryClauseStore(clauses);

        /// <summary>
        /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="SimpleClauseStore"/>.
        /// </summary>
        private class QueryClauseStore : IQueryClauseStore
        {
            private readonly HashSet<CNFDefiniteClause> clauses;

            public QueryClauseStore(IEnumerable<CNFDefiniteClause> clauses) => this.clauses = new HashSet<CNFDefiniteClause>(clauses);

            /// <inheritdoc />
            public Task<bool> AddAsync(CNFDefiniteClause clause, CancellationToken cancellationToken = default)
            {
                return Task.FromResult(clauses.Add(clause));
            }

#pragma warning disable CS1998 // async lacks await.. Could add await Task.Yield() to silence this, but it is not worth the overhead.
            /// <inheritdoc />
            public async IAsyncEnumerator<CNFDefiniteClause> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                foreach (var clause in clauses)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return clause;
                }
            }
#pragma warning restore CS1998

            /// <inheritdoc />
            public async IAsyncEnumerable<CNFDefiniteClause> GetApplicableRules(
                IEnumerable<Predicate> facts,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                await foreach (var clause in this.WithCancellation(cancellationToken))
                {
                    foreach (var conjunct in clause.Conjuncts)
                    {
                        foreach (var fact in facts)
                        {
                            if (LiteralUnifier.TryCreate(conjunct, fact, out _))
                            {
                                yield return clause;
                            }
                        }
                    }
                }
            }

            /// <inheritdoc />
            public async IAsyncEnumerable<(Predicate knownFact, VariableSubstitution unifier)> MatchWithKnownFacts(
                Predicate fact,
                VariableSubstitution substitution,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                // Here we just iterate through ALL known predicates trying to find something that unifies with the first conjunct.
                // We'd use an index here in anything approaching a production scenario:
                // TODO: ..we don't even store facts and rules separately, which we probably should..
                await foreach (var knownClause in this.WithCancellation(cancellationToken))
                {
                    if (knownClause.IsUnitClause)
                    {
                        if (LiteralUnifier.TryUpdate(knownClause.Consequent, fact, ref substitution))
                        {
                            yield return (knownClause.Consequent, substitution);
                        }
                    }
                }
            }

            /// <inheritdoc />
            public void Dispose()
            {
                //// Nothing to do..
            }
        }
    }
}
