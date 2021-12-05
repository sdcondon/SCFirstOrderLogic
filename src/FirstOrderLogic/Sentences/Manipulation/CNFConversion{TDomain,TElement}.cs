using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    /// <summary>
    /// Implementation of <see cref="SentenceTransformation{TDomain, TElement}"/> that converts sentences to conjunctive normal form.
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public class CNFConversion<TDomain, TElement> : SentenceTransformation<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        private readonly ImplicationElimination implicationElimination = new ImplicationElimination();
        private readonly NNFConversion nnfConversion = new NNFConversion();
        private readonly VariableStandardisation variableStandardisation = new VariableStandardisation();
        private readonly Skolemisation skolemisation = new Skolemisation();
        private readonly UniversalQuantifierElimination universalQuantifierElimination = new UniversalQuantifierElimination();
        private readonly DisjunctionDistribution disjunctionDistribution = new DisjunctionDistribution();

        public override Sentence<TDomain, TElement> ApplyTo(Sentence<TDomain, TElement> sentence)
        {
            // Might be possible to do some of these conversions at the same time, but for now
            // at least, do them sequentially.
            sentence = implicationElimination.ApplyTo(sentence);
            sentence = nnfConversion.ApplyTo(sentence);
            sentence = variableStandardisation.ApplyTo(sentence);
            sentence = skolemisation.ApplyTo(sentence);
            sentence = universalQuantifierElimination.ApplyTo(sentence);
            sentence = disjunctionDistribution.ApplyTo(sentence);

            return sentence;
        }

        private class ImplicationElimination : SentenceTransformation<TDomain, TElement>
        {
            public override Sentence<TDomain, TElement> ApplyTo(Implication<TDomain, TElement> implication)
            {
                return ApplyTo(new Disjunction<TDomain, TElement>(
                    new Negation<TDomain, TElement>(implication.Antecedent),
                    implication.Consequent));
            }
        }

        private class NNFConversion : SentenceTransformation<TDomain, TElement>
        {
            public override Sentence<TDomain, TElement> ApplyTo(Negation<TDomain, TElement> negation)
            {
                Sentence<TDomain, TElement> sentence;

                if (negation.Sentence is Negation<TDomain, TElement> n)
                {
                    // Eliminate double negative: ¬(¬P) ≡ P
                    sentence = n.Sentence;
                }
                else if (negation.Sentence is Conjunction<TDomain, TElement> c)
                {
                    // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                    sentence = new Disjunction<TDomain, TElement>(
                        new Negation<TDomain, TElement>(c.Left),
                        new Negation<TDomain, TElement>(c.Right));
                }
                else if (negation.Sentence is Disjunction<TDomain, TElement> d)
                {
                    // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                    sentence = new Conjunction<TDomain, TElement>(
                        new Negation<TDomain, TElement>(d.Left),
                        new Negation<TDomain, TElement>(d.Right));
                }
                else if (negation.Sentence is UniversalQuantification<TDomain, TElement> u)
                {
                    // Apply ¬∀x, p ≡ ∃x, ¬p
                    sentence = new ExistentialQuantification<TDomain, TElement>(
                        u.Variable,
                        new Negation<TDomain, TElement>(u.Sentence));
                }
                else if (negation.Sentence is ExistentialQuantification<TDomain, TElement> e)
                {
                    // Apply ¬∃x, p ≡ ∀x, ¬p
                    sentence = new UniversalQuantification<TDomain, TElement>(
                        e.Variable,
                        new Negation<TDomain, TElement>(e.Sentence));
                }
                else
                {
                    return base.ApplyTo(negation);
                }

                return ApplyTo(sentence);
            }
        }

        private class VariableStandardisation : SentenceTransformation<TDomain, TElement>
        {
            public override Sentence<TDomain, TElement> ApplyTo(Sentence<TDomain, TElement> sentence)
            {
                var variableScopeFinder = new VariableScopeFinder();
                variableScopeFinder.ApplyTo(sentence);
                return new VariableRenamer(variableScopeFinder.VariableScopes).ApplyTo(sentence);
            }

            // Ick: Double-nested class.
            // Ick: A "Transformation" that doesn't transform. More evidence to suggest introduction of visitor pattern at some point.
            private class VariableScopeFinder : SentenceTransformation<TDomain, TElement>
            {
                public List<Sentence<TDomain, TElement>> VariableScopes { get; } = new List<Sentence<TDomain, TElement>>();

                public override Sentence<TDomain, TElement> ApplyTo(ExistentialQuantification<TDomain, TElement> existentialQuantification)
                {
                    VariableScopes.Add(existentialQuantification);
                    return base.ApplyTo(existentialQuantification);
                }

                public override Sentence<TDomain, TElement> ApplyTo(UniversalQuantification<TDomain, TElement> universalQuantification)
                {
                    VariableScopes.Add(universalQuantification);
                    return base.ApplyTo(universalQuantification);
                }
            }

            // Ick: Double-nested class.
            private class VariableRenamer : SentenceTransformation<TDomain, TElement>
            {
                Dictionary<VariableDeclaration<TDomain, TElement>, VariableDeclaration<TDomain, TElement>> mapping;

                public VariableRenamer(IEnumerable<Sentence<TDomain, TElement>> variableScopes)
                {
                    foreach (var scope in variableScopes)
                    {

                    }
                }

                public override VariableDeclaration<TDomain, TElement> ApplyTo(VariableDeclaration<TDomain, TElement> variableDeclaration)
                {
                    if (mapping.TryGetValue(variableDeclaration, out var newDeclaration))
                    {
                        return newDeclaration;
                    }

                    return variableDeclaration;
                }
            }
        }

        private class Skolemisation : SentenceTransformation<TDomain, TElement>
        {
            // TODO!
        }

        private class UniversalQuantifierElimination : SentenceTransformation<TDomain, TElement>
        {
            public override Sentence<TDomain, TElement> ApplyTo(UniversalQuantification<TDomain, TElement> universalQuantification)
            {
                return ApplyTo(universalQuantification.Sentence);
            }
        }

        /// <summary>
        /// Sentence trnasformation that recursively distributes disjunctions over conjunctions.
        /// </summary>
        private class DisjunctionDistribution : SentenceTransformation<TDomain, TElement>
        {
            public override Sentence<TDomain, TElement> ApplyTo(Disjunction<TDomain, TElement> disjunction)
            {
                Sentence<TDomain, TElement> sentence;

                if (disjunction.Right is Conjunction<TDomain, TElement> cRight)
                {
                    // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                    // NB the "else if" below is fine (i.e. we don't need a seperate case for if they are both &&s)
                    // since if b.Left is also an &&, well end up distributing over it once we recurse down as far
                    // as the Expression.OrElses we create here.
                    sentence = new Conjunction<TDomain, TElement>(
                        new Disjunction<TDomain, TElement>(disjunction.Left, cRight.Left),
                        new Disjunction<TDomain, TElement>(disjunction.Left, cRight.Right));
                }
                else if (disjunction.Left is Conjunction<TDomain, TElement> cLeft)
                {
                    // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
                    sentence = new Conjunction<TDomain, TElement>(
                        new Disjunction<TDomain, TElement>(cLeft.Left, disjunction.Right),
                        new Disjunction<TDomain, TElement>(cLeft.Right, disjunction.Right));
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
