using SCFirstOrderLogic;
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Unification;

namespace SCFirstOrderLogic.Inference.ForwardChaining
{
    /// <summary>
    /// An implementation of <see cref="IKnowledgeBase"/> that uses a forward chaining algorithm. This one is implemented as close as possible
    /// (including variable and field naming, hence the naming that prioritises brevity over clarity) to the implementation in figure 9.6 of
    /// "Artificial Intelligence: A Modern Approach" - for reference and baselining purposes.
    /// </summary>
    public sealed class ForwardChainingKnowledgeBase_FromAIaMA : IKnowledgeBase
    {
        private readonly List<CNFDefiniteClause> clauses = new ();

        /// <inheritdoc />
        public Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            // First things first - normalise the sentence. Yes, the book hasn't talked about CNF for first-order logic by this point,
            // but this accomplishes a few things nice and easily:
            // * Puts it into a form where we can easily verify that it is all definite clauses
            // * Standardises variables for us
            // * Means we don't have to do existential instantiation - since thats essentially done for us via Skolemisation
            var cnfSentence = sentence.ToCNF();

            // Now we need to verify that it consists only of definite clauses before adding anything to the store:
            if (cnfSentence.Clauses.Any(c => !c.IsDefiniteClause))
            {
                throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses", nameof(sentence));
            }

            // Finally just add each clause to a (simple in-memory) list of known clauses.
            // Of course, in a production scenario we'd want some indexing. More on this in the implementation below.
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

        /// <inheritdoc />
        public Task<Query> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
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

            return Task.FromResult(new Query(p, clauses));
        }

        /// <summary>
        /// Query implementation used by <see cref="ForwardChainingKnowledgeBase_FromAIaMA"/>.
        /// </summary>
        public sealed class Query : IQuery
        {
            private readonly Predicate α;
            private readonly List<CNFDefiniteClause> kb;

            private bool? result;
            private VariableSubstitution? substitution;

            internal Query(Predicate α, List<CNFDefiniteClause> clauses)
            {
                this.α = α;
                this.kb = new List<CNFDefiniteClause>(clauses);
            }

            /// <inheritdoc />
            public bool IsComplete => result != null;

            /// <inheritdoc />
            public bool Result => result ?? throw new InvalidOperationException("Query is not yet complete");

            /// <summary>
            /// Gets the variable substitution that is carried out to resolve the query. Throws an exception if the query is not complete or returned a negative result.
            /// </summary>
            public VariableSubstitution Substitution
            {
                get
                {
                    if (!IsComplete)
                    {
                        throw new InvalidOperationException("Query is not yet complete");
                    }

                    if (!Result)
                    {
                        throw new InvalidOperationException("Query returned a negative result");
                    }

                    return substitution!;
                }
            }

            /// <inheritdoc />
            public void Dispose()
            {
            }

            /// <inheritdoc />
            public Task<bool> ExecuteAsync(CancellationToken cancellationToken = default)
            {
                var @new = new List<CNFDefiniteClause>();

                do
                {
                    @new.Clear();

                    foreach (var rule in kb)
                    {
                        // NB: we don't need the variable standardisation line from the book because that's already
                        // happened as part of the conversion to CNF that is carried out when the KB is told things.
                        // So all we do is call the consequent 'q' to match the book listing:
                        var q = rule.Consequent;

                        foreach (var θ in MatchWithKnownFacts(rule))
                        {
                            var qDash = θ.ApplyTo(q).Predicate;
                            var qDashAsClause = new CNFDefiniteClause(qDash); // Worth looking into a bit more type fluidity at some point..

                            if (!qDashAsClause.UnifiesWithAnyOf(kb) && !qDashAsClause.UnifiesWithAnyOf(@new))
                            {
                                @new.Add(qDashAsClause);

                                // WOULD-BE-A-BUG-IF-THIS-WERE-PRODUCTION-CODE: Only doing this when we have something "new" means that
                                // the KB will fail to confirm a sentence that is in the KB directly. Of course the fix (doing this out the
                                // outset of the outer foreach) has a significant performance impact, so I can KIND OF see why they've written
                                // this way in the book. Then again, they should have at least made a note about this.. Meh, never mind.
                                if (LiteralUnifier.TryCreate(qDash, α, out var φ))
                                {
                                    result = true;
                                    substitution = φ;
                                    return Task.FromResult(true);
                                }
                            }
                        }
                    }

                    kb.AddRange(@new);
                }
                while (@new.Count > 0);

                result = false;
                return Task.FromResult(false);
            }

            private IEnumerable<VariableSubstitution> MatchWithKnownFacts(CNFDefiniteClause clause)
            {
                // NB: no specific conjunct ordering here - just look at them in the order they happen to fall.
                // In a production scenario, we'd at least TRY to order the conjuncts in a way that minimises
                // the amount of work we have to do.
                return MatchWithKnownFacts(clause.Conjuncts, new VariableSubstitution());
            }

            // I'm not a huge fan of recursion when trying to write reference code but I'll admit it is handy here.. May revisit this..
            private IEnumerable<VariableSubstitution> MatchWithKnownFacts(IEnumerable<Predicate> conjuncts, VariableSubstitution unifier)
            {
                if (!conjuncts.Any())
                {
                    yield return unifier;
                }
                else
                {
                    // Here we just iterate through ALL known predicates trying to find something that unifies with the first conjunct.
                    // We'd use an index here in anything approaching a production scenario:
                    foreach (var knownClause in kb.Where(k => k.IsUnitClause)) 
                    {
                        var updatedUnifier = new VariableSubstitution(unifier);

                        if (LiteralUnifier.TryUpdateUnsafe(knownClause.Consequent, conjuncts.First(), updatedUnifier))
                        {
                            foreach(var substitution in MatchWithKnownFacts(conjuncts.Skip(1), updatedUnifier))
                            {
                                yield return substitution;
                            }
                        }
                    }
                }
            }
        }
    }
}
