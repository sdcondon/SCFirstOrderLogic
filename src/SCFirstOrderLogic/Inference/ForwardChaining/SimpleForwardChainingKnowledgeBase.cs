using SCFirstOrderLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a forward chaining algorithm.
    /// <para/>
    /// This is a VERY simple (read - inefficient) implementation that differs from the implementation in figure 9.3 of
    /// "Artificial Intelligence: A Modern Approach" only by the inclusion of proof tree retrieval.
    /// </summary>
    public sealed class SimpleForwardChainingKnowledgeBase : IKnowledgeBase
    {
        // TODO*-V3: what if large. At some point add a clause store equivalent..
        private readonly List<CNFDefiniteClause> clauses = new ();

        /// <inheritdoc />
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            // Normalize, then verify that the sentence consists only of definite clauses
            // before indexing ANY of them:
            var cnfSentence = sentence.ToCNF();

            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses", nameof(sentence));
            }

            // Finally just add each clause to a (simple in-memory) list of known clauses.
            // Of course, in a production scenario we'd want some indexing. More on this in the query class.
            foreach (var clause in cnfSentence.Clauses)
            {
                clauses.Add(new CNFDefiniteClause(clause));
            }

            return Task.CompletedTask;
        }

        /// <inheritdoc />
        async Task<IQuery> IKnowledgeBase.CreateQueryAsync(Sentence sentence, CancellationToken cancellationToken)
        {
            return await CreateQueryAsync(sentence, cancellationToken);
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <param name="cancellationToken">A cancellation token for the operation.</param>
        /// <returns>A task that returns an <see cref="SimpleForwardChainingQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public Task<SimpleForwardChainingQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            if (query is not Predicate p)
            {
                throw new ArgumentException("This knowledge base supports only queries that are predicates");
            }

            // Doesn't hurt to not standardise here - wont clash because all of the KB rules *are* standardised
            // (assuming the symbols in the query don't have weird equality rules)..
            // ..and in any case our standardisation logic assumes all variables to be quantified, otherwise it crashes..
            //var standardisation = new VariableStandardisation(query);
            //p = (Predicate)standardisation.ApplyTo(p);

            return Task.FromResult(new SimpleForwardChainingQuery(p, clauses));
        }

        /// <summary>
        /// Initiates a new query against the knowledge base.
        /// </summary>
        /// <param name="query">The query sentence.</param>
        /// <returns>An <see cref="SimpleForwardChainingQuery"/> instance that can be used to execute the query and examine the details of the result.</returns>
        public SimpleForwardChainingQuery CreateQuery(Sentence query)
        {
            return CreateQueryAsync(query).GetAwaiter().GetResult();
        }
    }
}
