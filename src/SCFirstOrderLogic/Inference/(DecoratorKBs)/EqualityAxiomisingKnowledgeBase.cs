// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// <para>
    /// Decorator knowledge base class that adds equality axioms as knowledge is added to the underlying knowledge base.
    /// </para>
    /// <para>
    /// NB #1: works only as knowledge is *added* - knowledge already in the inner knowledge base at the time of instantiation
    /// will NOT be examined for functions and predicates to add equality rules for. This limitation is ultimately because IKnowledgeBase
    /// offers no way to enumerate known facts - and adding this would be a bad idea. A decorator clause store for each of the inference
    /// algorithms (which absolutely can be enumerated) would be another way to go - but this has its own problems. Consumers to whom this
    /// matters are invited to examine the source code and implement whatever they need based on it.
    /// TODO-EXTENSIBILITY: look again at doing this at the clause store level.
    /// </para>
    /// <para>
    /// NB #2: See §9.5.5 ("Equality") of Artifical Intelligence: A Modern Approach for more on dealing with equality by axiomising it.
    /// </para>
    /// </summary>
    public class EqualityAxiomisingKnowledgeBase : IKnowledgeBase
    {
        private readonly IKnowledgeBase innerKnowledgeBase;
        private readonly PredicateAndFunctionEqualityAxiomiser predicateAndFunctionEqualityAxiomiser;

        /// <summary>
        /// Initialises a new instance of the <see cref="EqualityAxiomisingKnowledgeBase"/> class.
        /// </summary>
        /// <param name="innerKnowledgeBase">The inner knowledge base decorated by this class.</param>
        private EqualityAxiomisingKnowledgeBase(IKnowledgeBase innerKnowledgeBase)
        {
            this.innerKnowledgeBase = innerKnowledgeBase;
            this.predicateAndFunctionEqualityAxiomiser = new PredicateAndFunctionEqualityAxiomiser(innerKnowledgeBase);
        }

        /// <summary>
        /// <para>
        /// Instantiates and initializes a new instance of the <see cref="EqualityAxiomisingKnowledgeBase"/> class.
        /// </para>
        /// <para>
        /// This method exists and the constructor for <see cref="EqualityAxiomisingKnowledgeBase"/> is private
        /// because complete initialisation here involves telling the knowledge base some things. Telling is asynchronous
        /// because it is potentially long-running (because in "real" clause stores it could easily involve IO), and
        /// including potentially long-running operations in a constructor is generally a bad idea.
        /// </para>
        /// </summary>
        /// <returns>A task that returns a new <see cref="EqualityAxiomisingKnowledgeBase"/> instance.</returns>
        public static async Task<EqualityAxiomisingKnowledgeBase> CreateAsync(IKnowledgeBase innerKnowledgeBase)
        {
            // ..could invoke these in parallel if we wanted to.
            await innerKnowledgeBase.TellAsync(ForAll(X, AreEqual(X, X))); // Reflexivity
            await innerKnowledgeBase.TellAsync(ForAll(X, Y, If(AreEqual(X, Y), AreEqual(Y, X)))); // Commutativity
            await innerKnowledgeBase.TellAsync(ForAll(X, Y, Z, If(And(AreEqual(X, Y), AreEqual(Y, Z)), AreEqual(X, Z)))); // Transitivity

            return new EqualityAxiomisingKnowledgeBase(innerKnowledgeBase);
        }

        /// <inheritdoc/>
        public async Task TellAsync(Sentence sentence, CancellationToken cancellationToken = default)
        {
            await innerKnowledgeBase.TellAsync(sentence, cancellationToken);
            predicateAndFunctionEqualityAxiomiser.Visit(sentence);
        }

        /// <inheritdoc/>
        public Task<IQuery> CreateQueryAsync(Sentence query, CancellationToken cancellationToken = default)
        {
            return innerKnowledgeBase.CreateQueryAsync(query, cancellationToken);
        }

        private class PredicateAndFunctionEqualityAxiomiser : RecursiveSentenceVisitor
        {
            private readonly IKnowledgeBase innerKnowledgeBase;
            private readonly HashSet<object> knownPredicateIdentifiers = new() { EqualityIdentifier.Instance };
            private readonly HashSet<object> knownFunctionIdentifiers = new();

            public PredicateAndFunctionEqualityAxiomiser(IKnowledgeBase innerKnowledgeBase)
            {
                this.innerKnowledgeBase = innerKnowledgeBase;
            }

            public override void Visit(Predicate predicate)
            {
                // NB: we check only for the identifier, not for the identifier with the particular
                // argument count. A fairly safe assumption that we could nevertheless eliminate at some point.
                if (!knownPredicateIdentifiers.Contains(predicate.Identifier) && predicate.Arguments.Count > 0)
                {
                    knownPredicateIdentifiers.Add(predicate.Identifier);

                    // For all predicates, we have something like this,
                    // depending on the argument count:
                    // ∀ l0, r0, l0 = r0 ⇒ [P(l0) ⇔ P(r0)]
                    // ∀ l0, r0, l1, r1, [l0 = r0 ∧ l1 = r1] ⇒ [P(l0, l1) ⇔ P(r0, r1)]
                    // ... and so on
                    var leftArgs = predicate.Arguments.Select((_, i) => new VariableReference($"l{i}")).ToArray();
                    var rightArgs = predicate.Arguments.Select((_, i) => new VariableReference($"r{i}")).ToArray();
                    var consequent = Iff(new Predicate(predicate.Identifier, leftArgs), new Predicate(predicate.Identifier, rightArgs));

                    Sentence antecedent = AreEqual(leftArgs[0], rightArgs[0]);
                    for (int i = 1; i < predicate.Arguments.Count; i++)
                    {
                        antecedent = And(AreEqual(leftArgs[i], rightArgs[i]), antecedent);
                    }

                    Sentence sentence = ForAll(leftArgs[0].Declaration, ForAll(rightArgs[0].Declaration, If(antecedent, consequent)));
                    for (int i = 1; i < predicate.Arguments.Count; i++)
                    {
                        sentence = ForAll(leftArgs[i].Declaration, ForAll(rightArgs[i].Declaration, sentence));
                    }

                    // TODO-PERFORMANCE: potentially long-running. Perhaps add some async visitor types?
                    innerKnowledgeBase.TellAsync(sentence).GetAwaiter().GetResult();
                }

                base.Visit(predicate);
            }

            public override void Visit(Function function)
            {
                // NB: we check only for the identifier, not for the identifier with the particular
                // argument count. A fairly safe assumption that we could nevertheless eliminate at some point.
                if (!knownFunctionIdentifiers.Contains(function.Identifier) && function.Arguments.Count > 0)
                {
                    knownFunctionIdentifiers.Add(function.Identifier);

                    // For all functions, we have something like this,
                    // depending on the argument count:
                    // ∀ l0, r0, l0 = r0 ⇒ [F(l0) = F(r0)]
                    // ∀ l0, r0, l1, r1, [l0 = r0 ∧ l1 = r1] ⇒ [F(l0, l1) = F(r0, r1)]
                    // .. and so on
                    var leftArgs = function.Arguments.Select((_, i) => new VariableReference($"l{i}")).ToArray();
                    var rightArgs = function.Arguments.Select((_, i) => new VariableReference($"r{i}")).ToArray();
                    var consequent = AreEqual(new Function(function.Identifier, leftArgs), new Function(function.Identifier, rightArgs));

                    Sentence antecedent = AreEqual(leftArgs[0], rightArgs[0]);
                    for (int i = 1; i < function.Arguments.Count; i++)
                    {
                        antecedent = new Conjunction(AreEqual(leftArgs[i], rightArgs[i]), antecedent);
                    }

                    Sentence sentence = ForAll(leftArgs[0].Declaration, ForAll(rightArgs[0].Declaration, If(antecedent, consequent)));
                    for (int i = 1; i < function.Arguments.Count; i++)
                    {
                        sentence = ForAll(leftArgs[i].Declaration, ForAll(rightArgs[i].Declaration, sentence));
                    }

                    // TODO-PERFORMANCE: potentially long-running. Perhaps add some async visitor types?
                    innerKnowledgeBase.TellAsync(sentence).GetAwaiter().GetResult();
                }

                base.Visit(function);
            }
        }
    }
}
