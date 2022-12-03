using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Decorator knowledge base class that adds "axioms" for the unique names assumption as knowledge is added to the underlying knowledge base.
    /// <para/>
    /// Keeps track of all constants that feature in sentences, and adds "not equal" knowledge for all pairs
    /// with non-equal symbols. NB: only adds one ordering of arguments, and adds no knowledge that constants
    /// are equal to themselves - on the understanding that commutativity/reflexivity will be handled elsewhere
    /// (e.g. with <see cref="EqualityAxiomisingKnowledgeBase"/> or with an inner KB that utilises para/demodulation).
    /// </summary>
    public class UniqueNamesAxiomisingKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBase innerKnowledgeBase;
        private readonly UniqueNamesAxiomiser uniqueNameAxiomiser;

        /// <summary>
        /// Initialises a new instance of the <see cref="UniqueNamesAxiomisingKnowledgeBase"/> class.
        /// </summary>
        /// <param name="innerKnowledgeBase">The inner knowledge base decorated by this class.</param>
        public UniqueNamesAxiomisingKnowledgeBase(IKnowledgeBase innerKnowledgeBase)
        {
            this.innerKnowledgeBase = innerKnowledgeBase;
            this.uniqueNameAxiomiser = new UniqueNamesAxiomiser(innerKnowledgeBase);
        }

        /// <inheritdoc/>
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            await innerKnowledgeBase.TellAsync(sentence, cancellationToken);
            uniqueNameAxiomiser.Visit(sentence);
        }

        /// <inheritdoc/>
        public Task<IQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            return innerKnowledgeBase.CreateQueryAsync(query, cancellationToken);
        }

        // NB: The implementation doesn't need to look at the symbols because the
        // Constant class uses the Symbol for its equality implementation.
        private class UniqueNamesAxiomiser : RecursiveSentenceVisitor
        {
            private readonly IKnowledgeBase innerKnowledgeBase;
            private readonly HashSet<Constant> knownConstants = new();

            public UniqueNamesAxiomiser(IKnowledgeBase innerKnowledgeBase)
            {
                this.innerKnowledgeBase = innerKnowledgeBase;
            }

            public override void Visit(Constant constant)
            {
                if (!knownConstants.Contains(constant))
                {
                    foreach (var knownConstant in knownConstants)
                    {
                        innerKnowledgeBase.TellAsync(Not(AreEqual(constant, knownConstant))).Wait();
                    }

                    knownConstants.Add(constant);
                }
            }
        }
    }
}
