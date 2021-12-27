using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Implementation of <see cref="SentenceTransformation"/> that converts sentences to conjunctive normal form.
    /// </summary>
    public class CNFConversion : SentenceTransformation
    {
        private static readonly ImplicationElimination implicationElimination = new ImplicationElimination();
        private static readonly NNFConversion nnfConversion = new NNFConversion();
        private static readonly VariableStandardisation variableStandardisation = new VariableStandardisation();
        private static readonly Skolemisation skolemisation = new Skolemisation();
        private static readonly UniversalQuantifierElimination universalQuantifierElimination = new UniversalQuantifierElimination();
        private static readonly DisjunctionDistribution disjunctionDistribution = new DisjunctionDistribution();

        /// <summary>
        /// Gets a singleton instance of the <see cref="CNFConversion"/> class.
        /// </summary>
        public static CNFConversion Instance => new CNFConversion();

        /// <inheritdoc />
        public override Sentence ApplyTo(Sentence sentence)
        {
            // Might be possible to do some of these conversions at the same time, but for now
            // at least, do them sequentially - favour maintainability over performance for the mo.
            sentence = implicationElimination.ApplyTo(sentence);
            sentence = nnfConversion.ApplyTo(sentence);
            sentence = variableStandardisation.ApplyTo(sentence);
            sentence = skolemisation.ApplyTo(sentence);
            sentence = universalQuantifierElimination.ApplyTo(sentence);
            sentence = disjunctionDistribution.ApplyTo(sentence);

            // TODO?: Strictly-speaking its not needed for the normalisation process, but I wonder if we should also
            // ensure left- (or right-) associativity so that the Sentence propoerties of CNFSentence and CNFClause evaluate as equal
            // for sentences that normalise to (effectively) the same thing.

            return sentence;
        }

        /// <summary>
        /// Transformation that eliminates implications by replacing P ⇒ Q with ¬P ∨ Q and P ⇔ Q with (¬P ∨ Q) ∧ (P ∨ ¬Q)
        /// </summary>
        private class ImplicationElimination : SentenceTransformation
        {
            /// <inheritdoc />
            protected override Sentence ApplyTo(Implication implication)
            {
                return ApplyTo(new Disjunction(
                    new Negation(implication.Antecedent),
                    implication.Consequent));
            }

            /// <inheritdoc />
            protected override Sentence ApplyTo(Equivalence equivalence)
            {
                return ApplyTo(new Conjunction(
                    new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                    new Disjunction(equivalence.Left, new Negation(equivalence.Right))));
            }
        }

        /// <summary>
        /// Transformation that converts to Negation Normal Form by moving negations as far down as possible in the sentence tree.
        /// </summary>
        private class NNFConversion : SentenceTransformation
        {
            /// <inheritdoc />
            protected override Sentence ApplyTo(Negation negation)
            {
                Sentence sentence;

                if (negation.Sentence is Negation n)
                {
                    // Eliminate double negative: ¬(¬P) ≡ P
                    sentence = n.Sentence;
                }
                else if (negation.Sentence is Conjunction c)
                {
                    // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                    sentence = new Disjunction(
                        new Negation(c.Left),
                        new Negation(c.Right));
                }
                else if (negation.Sentence is Disjunction d)
                {
                    // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                    sentence = new Conjunction(
                        new Negation(d.Left),
                        new Negation(d.Right));
                }
                else if (negation.Sentence is UniversalQuantification u)
                {
                    // Apply ¬∀x, p ≡ ∃x, ¬p
                    sentence = new ExistentialQuantification(
                        u.Variable,
                        new Negation(u.Sentence));
                }
                else if (negation.Sentence is ExistentialQuantification e)
                {
                    // Apply ¬∃x, p ≡ ∀x, ¬p
                    sentence = new UniversalQuantification(
                        e.Variable,
                        new Negation(e.Sentence));
                }
                else
                {
                    return base.ApplyTo(negation);
                }

                return ApplyTo(sentence);
            }
        }

        /// <summary>
        /// Tranformation that "standardises" variables - essentially ensuring that variable names are unique.
        /// </summary>
        private class VariableStandardisation : SentenceTransformation
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Sentence sentence)
            {
                var quantificationFinder = new QuantificationFinder();
                quantificationFinder.ApplyTo(sentence);
                return new VariableRenamer(quantificationFinder.Quantifications).ApplyTo(sentence);
            }

            // NB: A "Transformation" that doesn't transform and has to use a property to expose its output. More evidence to suggest introduction of visitor pattern at some point.
            private class QuantificationFinder : SentenceTransformation
            {
                public List<Quantification> Quantifications { get; } = new List<Quantification>();

                protected override Sentence ApplyTo(Quantification quantification)
                {
                    Quantifications.Add(quantification);
                    return base.ApplyTo(quantification);
                }
            }

            private class VariableRenamer : SentenceTransformation
            {
                Dictionary<VariableDeclaration, VariableDeclaration> mapping = new Dictionary<VariableDeclaration, VariableDeclaration>();

                public VariableRenamer(List<Quantification> quantifications)
                {
                    for (int i = 0; i < quantifications.Count; i++)
                    {
                        // While a more complex approach that leaves variable names alone where it can has its benefits, here
                        // we just take a simple, even-handed approach and prepend the index of the variable (i.e. the order of
                        // discovery by the DFS done by QuantificationFinder).
                        mapping[quantifications[i].Variable] = new VariableDeclaration($"{i}:{quantifications[i].Variable.Name}");
                    }
                }

                protected override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration) => mapping[variableDeclaration];
            }
        }

        /// <summary>
        /// Transformation that eliminate existential quantification via the process of "Skolemisation". Replaces all existentially declared variables
        /// with a generated "Skolem" function that acts on all universally declared variables in scope when the existential variable was declared.
        /// </summary>
        private class Skolemisation : SentenceTransformation
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Sentence sentence)
            {
                return new ScopedSkolemisation(Enumerable.Empty<VariableDeclaration>(), new Dictionary<VariableDeclaration, SkolemFunction>()).ApplyTo(sentence);
            }

            private class ScopedSkolemisation : SentenceTransformation
            {
                private readonly IEnumerable<VariableDeclaration> universalVariablesInScope;
                private readonly Dictionary<VariableDeclaration, SkolemFunction> existentialVariableMap;

                public ScopedSkolemisation(IEnumerable<VariableDeclaration> universalVariablesInScope, Dictionary<VariableDeclaration, SkolemFunction> existentialVariableMap)
                {
                    this.universalVariablesInScope = universalVariablesInScope;
                    this.existentialVariableMap = existentialVariableMap;
                }

                protected override Sentence ApplyTo(UniversalQuantification universalQuantification)
                {
                    return new UniversalQuantification(
                        universalQuantification.Variable,
                        new ScopedSkolemisation(universalVariablesInScope.Append(universalQuantification.Variable), existentialVariableMap).ApplyTo(universalQuantification.Sentence));
                }

                protected override Sentence ApplyTo(ExistentialQuantification existentialQuantification)
                {
                    existentialVariableMap[existentialQuantification.Variable] = new SkolemFunction(
                        $"Skolem{existentialVariableMap.Count + 1}",
                        universalVariablesInScope.Select(a => new Variable(a)).ToList<Term>());
                    return base.ApplyTo(existentialQuantification.Sentence);
                }

                protected override Term ApplyTo(Variable variable)
                {
                    if (existentialVariableMap.TryGetValue(variable.Declaration, out var skolemFunction))
                    {
                        return skolemFunction;
                    }

                    // NB: leave universally declared variables alone
                    return base.ApplyTo(variable);
                }
            }
        }

        private class UniversalQuantifierElimination : SentenceTransformation
        {
            protected override Sentence ApplyTo(UniversalQuantification universalQuantification)
            {
                return ApplyTo(universalQuantification.Sentence);
            }
        }

        /// <summary>
        /// Transformation that recursively distributes disjunctions over conjunctions.
        /// </summary>
        private class DisjunctionDistribution : SentenceTransformation
        {
            protected override Sentence ApplyTo(Disjunction disjunction)
            {
                Sentence sentence;

                if (disjunction.Right is Conjunction cRight)
                {
                    // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                    // NB the "else if" below is fine (i.e. we don't need a seperate case for if they are both &&s)
                    // since if b.Left is also an &&, well end up distributing over it once we recurse down as far
                    // as the Expression.OrElses we create here.
                    sentence = new Conjunction(
                        new Disjunction(disjunction.Left, cRight.Left),
                        new Disjunction(disjunction.Left, cRight.Right));
                }
                else if (disjunction.Left is Conjunction cLeft)
                {
                    // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
                    sentence = new Conjunction(
                        new Disjunction(cLeft.Left, disjunction.Right),
                        new Disjunction(cLeft.Right, disjunction.Right));
                }
                else
                {
                    return base.ApplyTo(disjunction);
                }

                return ApplyTo(sentence);
            }
        }
    }
}
