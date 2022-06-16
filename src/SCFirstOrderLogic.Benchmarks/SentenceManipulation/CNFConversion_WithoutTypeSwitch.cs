using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation
{
    public class CNFConversion_WithoutTypeSwitch
    {
        private static readonly ImplicationElimination implicationElimination = new();
        private static readonly NNFConversion nnfConversion = new();
        private static readonly UniversalQuantifierElimination universalQuantifierElimination = new();
        private static readonly DisjunctionDistribution disjunctionDistribution = new();

        /// <summary>
        /// Gets a singleton instance of the <see cref="CNFConversion_WithoutTypeSwitch"/> class.
        /// </summary>
        public static CNFConversion_WithoutTypeSwitch Instance => new();

        /// <inheritdoc />
        public static Sentence ApplyTo(Sentence sentence)
        {
            // We do variable standardisation first, before altering the sentence in any
            // other way, so that we can easily store the context of the original variable in the new symbol. This
            // facilitates explanations of the origins of a particular variable (or Skolem function) in query result explanations.
            // TODO-ROBUSTNESS: If users include undeclared variables on the assumption they'll be treated as 
            // universally quantified and sentence-wide in scope, the behaviour is going to be, well, wrong.
            // Should we validate here..? Or handle on the assumption that they are universally quantified?
            // Also should probably complain when nested definitions uses the same symbol (i.e. symbols that are equal).
            sentence = VariableStandardisation.ApplyTo(sentence);

            // It might be possible to do some of these conversions at the same time, but for now
            // at least we do them sequentially - and in so doing favour maintainability over performance.
            // Perhaps revisit this later (but given that the main purpose of this library is learning, probably not).
            sentence = sentence.Accept(implicationElimination);
            sentence = sentence.Accept(nnfConversion);
            sentence = Skolemisation.ApplyTo(sentence);
            sentence = sentence.Accept(universalQuantifierElimination);
            sentence = sentence.Accept(disjunctionDistribution);

            return sentence;
        }

        /// <summary>
        /// Transformation that "standardises apart" variables - essentially ensuring that variable symbols are unique.
        /// <para/>
        /// Public to allow callers to mess about with the normalisation process.
        /// </summary>
        public static class VariableStandardisation
        {
            /// <inheritdoc />
            public static Sentence ApplyTo(Sentence sentence)
            {
                Sentence transormedSentence = sentence.Accept(new ScopedVariableStandardisation(sentence));
                return transormedSentence;
            }

            // Private inner class to hide necessarily short-lived object away from callers.
            // Would feel a bit uncomfortable publicly exposing a transformation class that can only be applied once.
            private class ScopedVariableStandardisation : SentenceTransformation_WithoutTypeSwitch
            {
                private readonly Dictionary<VariableDeclaration, VariableDeclaration> mapping = new();
                private readonly Sentence rootSentence;

                public ScopedVariableStandardisation(Sentence rootSentence)
                {
                    this.rootSentence = rootSentence;
                }

                public override Sentence ApplyTo(ExistentialQuantification existentialQuantification)
                {
                    /// Should we throw if the variable being standardised is already standardised? Or return it unchanged?
                    /// Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
                    mapping[existentialQuantification.Variable] = new VariableDeclaration(new StandardisedVariableSymbol(existentialQuantification, rootSentence));
                    return base.ApplyTo(existentialQuantification);
                }

                public override Sentence ApplyTo(UniversalQuantification universalQuantification)
                {
                    /// Should we throw if the variable being standardised is already standardised? Or return it unchanged?
                    /// Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
                    mapping[universalQuantification.Variable] = new VariableDeclaration(new StandardisedVariableSymbol(universalQuantification, rootSentence));
                    return base.ApplyTo(universalQuantification);
                }

                public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
                {
                    return mapping[variableDeclaration];
                }
            }
        }

        /// <summary>
        /// Transformation that eliminates implications by replacing P ⇒ Q with ¬P ∨ Q and P ⇔ Q with (¬P ∨ Q) ∧ (P ∨ ¬Q)
        /// </summary>
        public class ImplicationElimination : SentenceTransformation_WithoutTypeSwitch
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Implication implication)
            {
                // Convert  P ⇒ Q to ¬P ∨ Q 
                var replacement = new Disjunction(new Negation(implication.Antecedent), implication.Consequent);
                return replacement.Accept(this);
            }

            /// <inheritdoc />
            public override Sentence ApplyTo(Equivalence equivalence)
            {
                // Convert P ⇔ Q to (¬P ∨ Q) ∧ (P ∨ ¬Q)
                var replacement = new Conjunction(
                    new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                    new Disjunction(equivalence.Left, new Negation(equivalence.Right)));
                return replacement.Accept(this);
            }

            public override Sentence ApplyTo(Predicate predicate)
            {
                // There can't be any more implications once we've hit an atomic sentence, so just return.
                return predicate;
            }
        }

        /// <summary>
        /// Transformation that converts to Negation Normal Form by moving negations as far as possible from the root of the sentence tree.
        /// That is, directly applied to predicates.
        /// </summary>
        public class NNFConversion : SentenceTransformation_WithoutTypeSwitch
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Negation negation)
            {
                if (negation.Sentence is Negation n)
                {
                    // Eliminate double negative: ¬(¬P) ≡ P
                    return n.Sentence.Accept(this);
                }
                else if (negation.Sentence is Conjunction c)
                {
                    // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                    var replacement = new Disjunction(
                        new Negation(c.Left),
                        new Negation(c.Right));
                    return replacement.Accept(this);
                }
                else if (negation.Sentence is Disjunction d)
                {
                    // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                    var replacement = new Conjunction(
                        new Negation(d.Left),
                        new Negation(d.Right));
                    return replacement.Accept(this);
                }
                else if (negation.Sentence is UniversalQuantification u)
                {
                    // Apply ¬∀x, p ≡ ∃x, ¬p
                    var replacement = new ExistentialQuantification(
                        u.Variable,
                        new Negation(u.Sentence));
                    return replacement.Accept(this);
                }
                else if (negation.Sentence is ExistentialQuantification e)
                {
                    // Apply ¬∃x, p ≡ ∀x, ¬p
                    var replacement = new UniversalQuantification(
                        e.Variable,
                        new Negation(e.Sentence));
                    return replacement.Accept(this);
                }
                else
                {
                    return base.ApplyTo(negation);
                }
            }

            public override Sentence ApplyTo(Predicate predicate)
            {
                // There can't be any more negations once we've hit an atomic sentence, so just return.
                return predicate;
            }
        }

        /// <summary>
        /// Transformation that eliminate existential quantification via the process of "Skolemisation". Replaces all existentially declared variables
        /// with a generated "Skolem" function that acts on all universally declared variables in scope when the existential variable was declared.
        /// </summary>
        public static class Skolemisation
        {
            /// <inheritdoc />
            public static Sentence ApplyTo(Sentence sentence)
            {
                return sentence.Accept(new ScopedSkolemisation(sentence, Enumerable.Empty<VariableDeclaration>(), new Dictionary<VariableDeclaration, Function>()));
            }

            // Private inner class to hide necessarily short-lived object away from callers.
            // Would feel a bit uncomfortable publicly exposing a transformation class that can only be applied once.
            private class ScopedSkolemisation : SentenceTransformation_WithoutTypeSwitch
            {
                private readonly Sentence rootSentence;
                private readonly IEnumerable<VariableDeclaration> universalVariablesInScope;
                private readonly Dictionary<VariableDeclaration, Function> existentialVariableMap;

                public ScopedSkolemisation(
                    Sentence rootSentence,
                    IEnumerable<VariableDeclaration> universalVariablesInScope,
                    Dictionary<VariableDeclaration, Function> existentialVariableMap)
                {
                    this.rootSentence = rootSentence;
                    this.universalVariablesInScope = universalVariablesInScope;
                    this.existentialVariableMap = existentialVariableMap;
                }

                public override Sentence ApplyTo(UniversalQuantification universalQuantification)
                {
                    Sentence sentence = universalQuantification.Sentence.Accept(new ScopedSkolemisation(
                        rootSentence,
                        universalVariablesInScope.Append(universalQuantification.Variable), existentialVariableMap));

                    return new UniversalQuantification(universalQuantification.Variable, sentence);
                }

                public override Sentence ApplyTo(ExistentialQuantification existentialQuantification)
                {
                    existentialVariableMap[existentialQuantification.Variable] = new Function(
                        new SkolemFunctionSymbol(existentialQuantification, rootSentence),
                        universalVariablesInScope.Select(a => new VariableReference(a)).ToList<Term>());

                    return existentialQuantification.Sentence.Accept(this);
                }

                public override Term ApplyTo(VariableReference variable)
                {
                    if (existentialVariableMap.TryGetValue(variable.Declaration, out var skolemFunction))
                    {
                        return skolemFunction;
                    }
                    else
                    {
                        // NB: if we didn't find it in the map, then its a universally quantified variable - and we leave it alone:
                        return base.ApplyTo(variable);
                    }
                }
            }
        }

        /// <summary>
        /// Transformation that simply removes all universal quantifications.
        /// All variables in CNF sentences are assumed to be universally quantified.
        /// </summary>
        public class UniversalQuantifierElimination : SentenceTransformation_WithoutTypeSwitch
        {
            public override Sentence ApplyTo(UniversalQuantification universalQuantification)
            {
                return universalQuantification.Sentence.Accept(this);
            }

            public override Sentence ApplyTo(Predicate predicate)
            {
                // There can't be any more quantifications once we've hit an atomic sentence, so just return.
                return predicate;
            }
        }

        /// <summary>
        /// Transformation that recursively distributes disjunctions over conjunctions.
        /// </summary>
        public class DisjunctionDistribution : SentenceTransformation_WithoutTypeSwitch
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

                return sentence.Accept(this);
            }

            public override Sentence ApplyTo(Predicate predicate)
            {
                // There can't be any more disjunctions once we've hit an atomic sentence, so just return.
                return predicate;
            }
        }
    }
}
