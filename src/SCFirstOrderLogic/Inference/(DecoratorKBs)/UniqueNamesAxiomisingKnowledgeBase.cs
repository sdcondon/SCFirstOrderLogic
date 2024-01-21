// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// <para>
    /// Decorator knowledge base class that adds "axioms" for the unique names assumption as knowledge is added to the underlying knowledge base.
    /// </para>
    /// <para>
    /// Keeps track of all constants that feature in sentences, and adds "not equal" sentences for all pairs
    /// with non-equal identifiers. NB: only adds one ordering of arguments, and adds no knowledge that constants
    /// are equal to themselves - on the understanding that commutativity/reflexivity will be handled elsewhere
    /// (e.g. with <see cref="EqualityAxiomisingKnowledgeBase"/> or with an inner KB that utilises para/demodulation).
    /// </para>
    /// <para>
    /// NB: works only as knowledge is *added* - knowledge already in the inner knowledge base at the time of instantiation
    /// will NOT be examined for constants to add unique names knowledge for. This limitation is ultimately because IKnowledgeBase
    /// offers no way to enumerate known facts - and I'm rather reluctant to add this, for several reasons. A decorator clause store
    /// for each of the inference algorithms (which absolutely can be enumerated) would be another way to go - but this has its own
    /// problems. Consumers to whom this matters are invited to examine the source code and implement whatever they need based on it.
    /// TODO-EXTENSIBILITY: look again at doing this at the clause store level.
    /// </para>
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

        private class UniqueNamesAxiomiser : RecursiveSentenceVisitor
        {
            private readonly IKnowledgeBase innerKnowledgeBase;

            // NB: We only need to consider the constant (i.e. not the identifier) here
            // because the Constant class uses the Identifier for its equality implementation.
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
                        // TODO-PERFORMANCE: potentially long-running. Perhaps add some async visitor types?
                        innerKnowledgeBase.TellAsync(Not(AreEqual(constant, knownConstant))).GetAwaiter().GetResult();
                    }

                    knownConstants.Add(constant);
                }
            }
        }
    }
}
