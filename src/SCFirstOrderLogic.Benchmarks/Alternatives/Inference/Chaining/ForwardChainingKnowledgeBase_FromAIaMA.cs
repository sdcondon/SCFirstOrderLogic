using SCFirstOrderLogic.Inference.Unification;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
            var cnfSentence = new CNFSentence(sentence);

            // For each clause of the sentence..
            foreach (var clause in cnfSentence.Clauses)
            {
                // ..verify that it is a definite clause..
                if (!clause.IsDefiniteClause)
                {
                    throw new ArgumentException("This knowledge base supports only knowledge in the form of definite clauses", nameof(sentence));
                }

                var definiteClause = new CNFDefiniteClause(clause);

                // ..add it to a (simple in-memory) list of known clauses..
                // (of course, in a production scenario we'd probably want to index by antecedent symbol - but this is intended as a very simple example, so we don't)
                clauses.Add(definiteClause);
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

            internal Query(Predicate α, List<CNFDefiniteClause> clauses)
            {
                this.α = α;
                this.kb = new List<CNFDefiniteClause>(clauses);
            }

            /// <inheritdoc />
            public bool IsComplete { get; private set; }

            /// <inheritdoc />
            public bool Result { get; private set; }

            public VariableSubstitution Substitution { get; private set; }

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

                                if (LiteralUnifier.TryCreate(qDash, α, out var φ))
                                {
                                    Result = true;
                                    Substitution = φ;
                                    IsComplete = true;
                                    return Task.FromResult(true);
                                }
                            }
                        }
                    }

                    kb.AddRange(@new);
                }
                while (@new.Count > 0);

                Result = false;
                IsComplete = true;
                return Task.FromResult(false);
            }

            private IEnumerable<VariableSubstitution> MatchWithKnownFacts(CNFDefiniteClause clause)
            {
                // NB: no specific conjunct ordering here - just look at them in the order they happen to fall.
                // In a production scenario, we'd at least TRY to order the antecedents in a way that minimises
                // the amount of work we have to do.
                return MatchWithKnownFacts(clause.Antecedents, new VariableSubstitution());
            }

            // I'm not a huge fan of recursion when trying to write reference code but I'll admit it is handy here.. May revisit this..
            private IEnumerable<VariableSubstitution> MatchWithKnownFacts(IEnumerable<Predicate> antecedents, VariableSubstitution unifier)
            {
                if (!antecedents.Any())
                {
                    yield return unifier;
                }
                else
                {
                    // Here we just iterate through ALL known predicates trying to find something that unifies with the first antecedent.
                    // We'd use an index here in anything approaching a production scenario:
                    foreach (var knownClause in kb.Where(k => k.IsUnitClause)) 
                    {
                        if (LiteralUnifier.TryUpdate(knownClause.Consequent, antecedents.First(), unifier))
                        {
                            foreach(var substitution in MatchWithKnownFacts(antecedents.Skip(1), new VariableSubstitution(unifier)))
                            {
                                yield return substitution;
                            }
                        }
                    }
                }
            }
        }

        // Decorator type that adds methods and properties appropriate for definite clauses.
        // A useful class, no doubt, and one that at some point might get "promoted" to live in the SentenceManipulation namespace.
        // HOWEVER, not quite sure how I want to deal with it just yet, so internal for now.
        // e.g. should it derive from CNFClause as well/instead of being composed of one?
        internal class CNFDefiniteClause
        {
            private readonly CNFClause clause;

            public CNFDefiniteClause(CNFClause clause)
            {
                // If this were public we'd need to validate here..
                this.clause = clause;
            }

            public CNFDefiniteClause(Predicate predicate)
            {
                this.clause = new CNFClause(new CNFLiteral[] { predicate });;
            }

            /// <summary>
            /// Gets the consequent of this clause (that is, the C of A₁ ∧ A₂ ∧ .. ∧ Aₙ ⇒ C)
            /// </summary>
            public Predicate Consequent => clause.Literals.Single(l => l.IsPositive).Predicate;

            /// <summary>
            /// Gets the antecedents of this clause (that is, the A₁, .. Aₙ of A₁ ∧ A₂ ∧ .. ∧ Aₙ ⇒ C)
            /// </summary>
            public IEnumerable<Predicate> Antecedents => clause.Literals.Where(l => l.IsNegated).Select(l => l.Predicate);

            public bool IsUnitClause => clause.IsUnitClause;

            // NB: not specific to definite clauses..
            public bool UnifiesWithAnyOf(IEnumerable<CNFDefiniteClause> clauses)
            {
                return clauses.Any(c => TryUnify(c, this, out var _));
            }

            // NB: not specific to definite clauses..
            private static bool TryUnify(CNFDefiniteClause clause1, CNFDefiniteClause clause2, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
            {
                if (clause1.clause.Literals.Count != clause2.clause.Literals.Count)
                {
                    unifier = null;
                    return false;
                }

                unifier = new VariableSubstitution();

                foreach (var (literal1, literal2) in clause1.clause.Literals.Zip(clause2.clause.Literals))
                {
                    if (!LiteralUnifier.TryUpdate(literal1, literal2, unifier))
                    {
                        unifier = null;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
