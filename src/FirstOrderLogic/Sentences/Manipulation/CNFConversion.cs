using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences.Manipulation
{
    /// <summary>
    /// Implementation of <see cref="SentenceTransformation"/> that converts sentences to conjunctive normal form.
    /// </summary>
    public class CNFConversion : SentenceTransformation
    {
        private readonly ImplicationElimination implicationElimination = new ImplicationElimination();
        private readonly NNFConversion nnfConversion = new NNFConversion();
        private readonly VariableStandardisation variableStandardisation = new VariableStandardisation();
        private readonly Skolemisation skolemisation = new Skolemisation();
        private readonly UniversalQuantifierElimination universalQuantifierElimination = new UniversalQuantifierElimination();
        private readonly DisjunctionDistribution disjunctionDistribution = new DisjunctionDistribution();

        public override Sentence ApplyTo(Sentence sentence)
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

        private class ImplicationElimination : SentenceTransformation
        {
            public override Sentence ApplyTo(Implication implication)
            {
                return ApplyTo(new Disjunction(
                    new Negation(implication.Antecedent),
                    implication.Consequent));
            }
        }

        private class NNFConversion : SentenceTransformation
        {
            public override Sentence ApplyTo(Negation negation)
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

        private class VariableStandardisation : SentenceTransformation
        {
            public override Sentence ApplyTo(Sentence sentence)
            {
                var variableScopeFinder = new VariableScopeFinder();
                variableScopeFinder.ApplyTo(sentence);
                return new VariableRenamer(variableScopeFinder.VariableScopes).ApplyTo(sentence);
            }

            // Ick: Double-nested class.
            // Ick: A "Transformation" that doesn't transform. More evidence to suggest introduction of visitor pattern at some point.
            private class VariableScopeFinder : SentenceTransformation
            {
                public List<Sentence> VariableScopes { get; } = new List<Sentence>();

                public override Sentence ApplyTo(ExistentialQuantification existentialQuantification)
                {
                    VariableScopes.Add(existentialQuantification);
                    return base.ApplyTo(existentialQuantification);
                }

                public override Sentence ApplyTo(UniversalQuantification universalQuantification)
                {
                    VariableScopes.Add(universalQuantification);
                    return base.ApplyTo(universalQuantification);
                }
            }

            // Ick: Double-nested class.
            private class VariableRenamer : SentenceTransformation
            {
                Dictionary<VariableDeclaration, VariableDeclaration> mapping;

                public VariableRenamer(IEnumerable<Sentence> variableScopes)
                {
                    foreach (var scope in variableScopes)
                    {

                    }
                }

                public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
                {
                    if (mapping.TryGetValue(variableDeclaration, out var newDeclaration))
                    {
                        return newDeclaration;
                    }

                    return variableDeclaration;
                }
            }
        }

        private class Skolemisation : SentenceTransformation
        {
            // TODO!
        }

        private class UniversalQuantifierElimination : SentenceTransformation
        {
            public override Sentence ApplyTo(UniversalQuantification universalQuantification)
            {
                return ApplyTo(universalQuantification.Sentence);
            }
        }

        /// <summary>
        /// Sentence trnasformation that recursively distributes disjunctions over conjunctions.
        /// </summary>
        private class DisjunctionDistribution : SentenceTransformation
        {
            public override Sentence ApplyTo(Disjunction disjunction)
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
