using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.Inference
{
    /// <summary>
    /// Decorator knowledge base class that adds equality axioms as knowledge is added to the underlying knowledge base.
    /// See §9.5.5 ("Equality") of Artifical Intelligence: A Modern Approach for more on dealing with equality by axiomising it.
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
        /// Instantiates and initializes a new instance of the <see cref="EqualityAxiomisingKnowledgeBase"/> class.
        /// <para/>
        /// This method exists and the constructor for <see cref="EqualityAxiomisingKnowledgeBase"/> is private
        /// because complete initialisation here involves telling the knowledge base some things. Telling is asynchronous
        /// because it is potentially long-running, and including potentially long-running operations in a constructor is generally a bad idea.
        /// </summary>
        /// <returns>A task that returns a new <see cref="EqualityAxiomisingKnowledgeBase"/> instance.</returns>
        public static async Task<EqualityAxiomisingKnowledgeBase> CreateAsync(IKnowledgeBase innerKnowledgeBase)
        {
            // ..could invoke these in parallel if we wanted to. At the time of writing the only KB we have isn't re-entrant though..
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
            private readonly HashSet<object> knownPredicateSymbols = new();
            private readonly HashSet<object> knownFunctionSymbols = new();

            public PredicateAndFunctionEqualityAxiomiser(IKnowledgeBase innerKnowledgeBase)
            {
                this.innerKnowledgeBase = innerKnowledgeBase;
            }

            public override void Visit(Predicate predicate)
            {
                // NB: we check only for the symbol, not for the symbol with the particular
                // argument count. A fairly safe assumption that we could nevertheless eliminate at some point.
                if (!knownPredicateSymbols.Contains(predicate.Symbol) && predicate.Arguments.Count > 0)
                {
                    knownPredicateSymbols.Add(predicate.Symbol);

                    // For all predicates, we have something like this,
                    // depending on the argument count:
                    // ∀ l0, r0, l0 = r0 ⇒ [P(l0) ⇔ P(r0)]
                    // ∀ l0, r0, l1, r1 [l0 = r0 ∧ l1 = r1] ⇒ [P(l0, l1) ⇔ P(r0, r1)]
                    // ... and so on
                    var leftArgs = predicate.Arguments.Select((_, i) => new VariableReference($"l{i}")).ToArray();
                    var rightArgs = predicate.Arguments.Select((_, i) => new VariableReference($"r{i}")).ToArray();
                    var consequent = Iff(new Predicate(predicate.Symbol, leftArgs), new Predicate(predicate.Symbol, rightArgs));

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

                    innerKnowledgeBase.TellAsync(sentence).Wait(); // potentially long-running..
                }

                base.Visit(predicate);
            }

            public override void Visit(Function function)
            {
                // NB: we check only for the symbol, not for the symbol with the particular
                // argument count. A fairly safe assumption that we could nevertheless eliminate at some point.
                if (!knownFunctionSymbols.Contains(function.Symbol))
                {
                    knownFunctionSymbols.Add(function.Symbol);

                    // For all functions, we have something like this,
                    // depending on the argument count:
                    // ∀ l0, r0, l0 = r0 ⇒ [F(l0) = F(r0)]
                    // ∀ l0, r0, l1, r1, [l0 = r0 ∧ l1 = r1] ⇒ [F(l0, l1) = F(r0, r1)]
                    // .. and so on
                    var leftArgs = function.Arguments.Select((_, i) => new VariableReference($"l{i}")).ToArray();
                    var rightArgs = function.Arguments.Select((_, i) => new VariableReference($"r{i}")).ToArray();
                    var consequent = AreEqual(new Function(function.Symbol, leftArgs), new Function(function.Symbol, rightArgs));

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

                    innerKnowledgeBase.TellAsync(sentence).Wait(); // potentially long-running..
                }

                base.Visit(function);
            }
        }
    }
}
