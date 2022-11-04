using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;
using System;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClauseStore"/> class.
        /// </summary>
        public SimpleClauseStore() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleClauseStore"/> class that is pre-populated with some knowledge.
        /// <para/>
        /// NB: Of course, most implementations of <see cref="IClauseStore"/> won't have a constructor for pre-population, because most
        /// clause stores will do IO when adding knowledge, and including long-running operations in a ctor is generally a bad idea.
        /// We only include it here because of (the in-memory nature of this implementation and) its usefulness for tests.
        /// </summary>
        /// <param name="sentences">The initial content of the store.</param>
        public SimpleClauseStore(IEnumerable<Sentence> sentences)
        {
            foreach (var sentence in sentences)
            {
                foreach (var clause in sentence.ToCNF().Clauses)
                {
                    if (!clause.IsDefiniteClause)
                    {
                        throw new ArgumentException($"All forward chaining knowledge must be expressable as definite clauses. The normalisation of {sentence} includes {clause}, which is not a definite clause");
                    }

                    clauses.Add(new CNFDefiniteClause(clause));
                }
            }
        }

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
        public Task<IQueryClauseStore> CreateQueryStoreAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IQueryClauseStore>(new QueryStore(clauses));
        }

        /// <summary>
        /// Implementation of <see cref="IQueryClauseStore"/> that is used solely by <see cref="SimpleClauseStore"/>.
        /// </summary>
        private class QueryStore : IQueryClauseStore
        {
            private readonly HashSet<CNFDefiniteClause> clauses;

            public QueryStore(IEnumerable<CNFDefiniteClause> clauses) => this.clauses = new HashSet<CNFDefiniteClause>(clauses);

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
                bool AnyConjunctUnifiesWithAnyFact(CNFDefiniteClause clause)
                {
                    foreach (var conjunct in clause.Conjuncts)
                    {
                        foreach (var fact in facts)
                        {
                            if (LiteralUnifier.TryCreate(conjunct, fact, out _))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }

                await foreach (var clause in this.WithCancellation(cancellationToken))
                {
                    if (AnyConjunctUnifiesWithAnyFact(clause))
                    {
                        yield return clause;
                    }
                }
            }

            /// <inheritdoc />
            public async IAsyncEnumerable<(Predicate knownFact, VariableSubstitution unifier)> MatchWithKnownFacts(
                Predicate fact,
                VariableSubstitution constraints,
                [EnumeratorCancellation] CancellationToken cancellationToken = default)
            {
                // Here we just iterate through ALL known predicates trying to find something that unifies with the fact.
                // A better implementation would do some kind of indexing:
                // TODO-PERFORMANCE: ..we don't even store facts and rules separately..
                await foreach (var knownClause in this.WithCancellation(cancellationToken))
                {
                    if (knownClause.IsUnitClause)
                    {
                        var unifier = new VariableSubstitution(constraints);

                        if (LiteralUnifier.TryUpdateUnsafe(knownClause.Consequent, fact, unifier))
                        {
                            yield return (knownClause.Consequent, unifier);
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
