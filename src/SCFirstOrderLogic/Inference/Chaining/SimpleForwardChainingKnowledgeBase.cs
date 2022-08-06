﻿using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.Inference.Chaining
{
    /// <summary>
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a backward chaining algorithm. This one is implemented as close as possible
    /// (including variable and field naming, hence the naming that prioritises brevity over clarity) to the implementation in figure 9.6 of
    /// "Artificial Intelligence: A Modern Approach" - for reference and baselining purposes.
    /// </summary>
    public sealed class SimpleForwardChainingKnowledgeBase : IKnowledgeBase
    {
        private readonly List<CNFDefiniteClause> clauses = new ();

        /// <inheritdoc />
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            // First things first - normalise the sentence.
            var cnfSentence = new CNFSentence(sentence);

            // Now we need to verify that it consists only of definite clauses before adding anything to the store:
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
            return CreateQueryAsync(query).Result;
        }
    }
}