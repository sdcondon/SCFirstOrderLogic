using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Decorator knowledge base class that adds equality axioms as knowledge is added to the underlying knowledge base.
    /// See §9.5.5 of Artifical Intelligence: A Modern Approach for more on dealing with equality by axiomising it.
    /// </summary>
    public class EqualityAxiomisingKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBase innerKnowledgeBase;
        private readonly PredicateAndFunctionEqualityAxiomiser predicateAndFunctionEqualityAxiomiser;

        /// <summary>
        /// Initialises a new instance of the <see cref="EqualityAxiomisingKnowledgeBase"/> class.
        /// </summary>
        public EqualityAxiomisingKnowledgeBase(IKnowledgeBase innerKnowledgeBase)
        {
            this.innerKnowledgeBase = innerKnowledgeBase;
            this.predicateAndFunctionEqualityAxiomiser = new PredicateAndFunctionEqualityAxiomiser(innerKnowledgeBase);

            // Tell the knowledge base the fundamental properties of equality:
            innerKnowledgeBase.TellAsync(ForAll(X, AreEqual(X, X))).Wait(); // Reflexivity
            innerKnowledgeBase.TellAsync(ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X)))).Wait(); // Commutativity
            innerKnowledgeBase.TellAsync(ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z)))).Wait(); // Transitivity
        }

        /// <inheritdoc/>
        public Task<bool> AskAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            return innerKnowledgeBase.AskAsync(query, cancellationToken);
        }

        /// <inheritdoc/>
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            await innerKnowledgeBase.TellAsync(sentence);
            predicateAndFunctionEqualityAxiomiser.ApplyTo(sentence);
        }

        // TODO: Look for new functions and predicates, add equality axioms for them
        // .. To figure out for myself - why don't the fundamental axioms suffice (given that using the fundamental axioms
        // means that unification can encounter the same function or predicate?)
        private class PredicateAndFunctionEqualityAxiomiser : SentenceTransformation
        {
            private readonly IKnowledgeBase innerKnowledgeBase;
            private readonly HashSet<object> knownPredicateSymbols = new HashSet<object>();
            private readonly HashSet<object> knownFunctionSymbols = new HashSet<object>();

            public PredicateAndFunctionEqualityAxiomiser(IKnowledgeBase innerKnowledgeBase)
            {
                this.innerKnowledgeBase = innerKnowledgeBase;
            }

            protected override Sentence ApplyTo(Predicate predicate)
            {
                // NB: we check only for the symbol, not for the symbol with the particular
                // argument count. a fairly safe assumption that we could possible eliminate at some point.
                if (!knownPredicateSymbols.Contains(predicate.Symbol))
                {
                    knownPredicateSymbols.Add(predicate.Symbol);
                    // todo: tell
                }

                return base.ApplyTo(predicate);
            }

            protected override Term ApplyTo(Function function)
            {
                // NB: we check only for the symbol, not for the symbol with the particular
                // argument count. a fairly safe assumption that we could possible eliminate at some point.
                if (!knownFunctionSymbols.Contains(function.Symbol))
                {
                    knownFunctionSymbols.Add(function.Symbol);
                    // todo: tell
                }

                return base.ApplyTo(function);
            }
        }
    }
}
