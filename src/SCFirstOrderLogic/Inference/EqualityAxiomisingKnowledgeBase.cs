#if FALSE
using System.Collections.Generic;
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
        private readonly ISet<object> knownPredicateSymbols = new HashSet<object>();
        private readonly ISet<object> knownFunctionSymbols = new HashSet<object>();

        /// <summary>
        /// Initialises a new instance of the <see cref="EqualityAxiomisingKnowledgeBase"/> class.
        /// </summary>
        public EqualityAxiomisingKnowledgeBase(IKnowledgeBase innerKnowledgeBase)
        {
            this.innerKnowledgeBase = innerKnowledgeBase;

            // Tell the knowledge base the fundamental properties of equality:
            innerKnowledgeBase.Tell(ForAll(X, AreEqual(X, X))); // Reflexivity
            innerKnowledgeBase.Tell(ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X)))); // Commutativity
            innerKnowledgeBase.Tell(ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z)))); // Transitivity
        }

        /// <inheritdoc/>
        public bool Ask(Sentence query)
        {
            return innerKnowledgeBase.Ask(query);
        }

        /// <inheritdoc/>
        public void Tell(Sentence sentence)
        {
            innerKnowledgeBase.Tell(sentence);

            // TODO: Look for new functions and predicates, add equality axioms for them
            // .. To figure out for myself - why don't the fundamental axioms suffice (given that using the fundamental axioms
            // means that unification can encounter the same function or predicate?)
        }
    }
}
#endif