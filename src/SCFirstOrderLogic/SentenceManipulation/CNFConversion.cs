// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation
{
    /// <summary>
    /// <para>
    /// Converts sentences to conjunctive normal form.
    /// </para>
    /// <para>
    /// This type is internal because its output is not *completely* normalised, since it *doesn't* normalise the
    /// order of evaluation of the clauses (i.e. the conjunctions found at the root of the output sentence),
    /// or the literals within those clauses (i.e. disjunctions found below those top-level conjunctions).
    /// The <see cref="CNFSentence"/> class does that. If there is ever a need to expose this class publicly (unlikely -
    /// <see cref="CNFSentence"/> should suffice), I'd want to add some more robustness stuff to eliminate potential
    /// confusion.
    /// </para>
    /// </summary>
    internal static class CNFConversion
    {
        private static readonly ImplicationElimination implicationElimination = new();
        private static readonly NNFConversion nnfConversion = new();
        private static readonly UniversalQuantifierElimination universalQuantifierElimination = new();
        private static readonly DisjunctionDistribution disjunctionDistribution = new();

        /// <summary>
        /// Applies the various normalisation transformations (variable standardisation, implication elimination, negation normalisation, Skolemisation,
        /// universal quantifier elimination, and disjunction distribution) to a given sentence <see cref="Sentence"/> instance.
        /// </summary>
        /// <param name="sentence">The sentence to convert.</param>
        /// <returns>The transformed sentence.</returns>
        public static Sentence ApplyTo(Sentence sentence)
        {
            // We do variable standardisation first, before altering the sentence in any
            // other way, so that we can easily store the context of the original variable in the new symbol. This
            // facilitates explanations of the origins of a particular variable (or Skolem function) in query result explanations.
            // TODO-ROBUSTNESS: If users include undeclared variables on the assumption they'll be treated as 
            // universally quantified and sentence-wide in scope, the behaviour is going to be, well, wrong.
            // Should we validate here..? Or handle on the assumption that they are universally quantified?
            // Trying to write some tests for this should help in establishing 'nice' behaviour.
            // Also should probably complain when nested definitions uses the same symbol (i.e. symbols that are equal).
            sentence = new VariableStandardisation(sentence).ApplyTo(sentence);
            var standardisedSentence = sentence;

            // It might be possible to do some of these conversions at the same time, but for now
            // at least we do them sequentially - and in so doing favour maintainability over performance.
            // Perhaps revisit this later (but given that the main purpose of this library is learning, probably not).
            sentence = implicationElimination.ApplyTo(sentence);
            sentence = nnfConversion.ApplyTo(sentence);
            sentence = new Skolemisation(standardisedSentence).ApplyTo(sentence);
            sentence = universalQuantifierElimination.ApplyTo(sentence);
            sentence = disjunctionDistribution.ApplyTo(sentence);

            return sentence;
        }

        /// <summary>
        /// Transformation that "standardises apart" variables - essentially ensuring that variable symbols are unique.
        /// </summary>
        public class VariableStandardisation : RecursiveSentenceTransformation
        {
            private readonly Dictionary<VariableDeclaration, VariableDeclaration> mapping = new();
            private readonly Sentence rootSentence;

            /// <summary>
            /// Initializes a new instance of the <see cref="VariableStandardisation"/> class.
            /// </summary>
            /// <param name="rootSentence">
            /// The root sentence being transformed. Stored against symbols of the new standardised versions of the variables, for later use in explanations and the like.
            /// </param>
            public VariableStandardisation(Sentence rootSentence)
            {
                this.rootSentence = rootSentence;
            }

            public override Sentence ApplyTo(Quantification quantification)
            {
                // Should we throw if the variable being standardised is already standardised? Or return it unchanged?
                // Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
                mapping[quantification.Variable] = new VariableDeclaration(new StandardisedVariableSymbol(quantification, rootSentence));
                return base.ApplyTo(quantification);
            }

            public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
            {
                // Undeclared variables are assumed to be (universal in nature and) sentence-wide in scope.
                if (!mapping.TryGetValue(variableDeclaration, out var standardisedVariableDeclaration))
                {
                    // Should we throw if the variable being standardised is already standardised? Or return it unchanged?
                    // Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
                    // TODO-ZZ-ROBUSTNESS: This creation of implicit scope is hacky. In particular, think about the inconsistency when there are multiple. Is this a problem? Ponder me.
                    var implicitScope = new UniversalQuantification(variableDeclaration, rootSentence);
                    standardisedVariableDeclaration = mapping[variableDeclaration] = new VariableDeclaration(new StandardisedVariableSymbol(implicitScope, rootSentence));
                }

                return standardisedVariableDeclaration;
            }
        }

        /// <summary>
        /// Transformation that eliminates implications by replacing P ⇒ Q with ¬P ∨ Q and P ⇔ Q with (¬P ∨ Q) ∧ (P ∨ ¬Q)
        /// </summary>
        public class ImplicationElimination : RecursiveSentenceTransformation
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Implication implication)
            {
                // Convert  P ⇒ Q to ¬P ∨ Q 
                return ApplyTo(new Disjunction(
                    new Negation(implication.Antecedent),
                    implication.Consequent));
            }

            /// <inheritdoc />
            public override Sentence ApplyTo(Equivalence equivalence)
            {
                // Convert P ⇔ Q to (¬P ∨ Q) ∧ (P ∨ ¬Q)
                return ApplyTo(new Conjunction(
                    new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                    new Disjunction(equivalence.Left, new Negation(equivalence.Right))));
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
        public class NNFConversion : RecursiveSentenceTransformation
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Negation negation)
            {
                if (negation.Sentence is Negation n)
                {
                    // Eliminate double negative: ¬(¬P) ≡ P
                    return ApplyTo(n.Sentence);
                }
                else if (negation.Sentence is Conjunction c)
                {
                    // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                    return ApplyTo(new Disjunction(
                        new Negation(c.Left),
                        new Negation(c.Right)));
                }
                else if (negation.Sentence is Disjunction d)
                {
                    // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                    return ApplyTo(new Conjunction(
                        new Negation(d.Left),
                        new Negation(d.Right)));
                }
                else if (negation.Sentence is UniversalQuantification u)
                {
                    // Apply ¬∀x, p ≡ ∃x, ¬p
                    return ApplyTo(new ExistentialQuantification(
                        u.Variable,
                        new Negation(u.Sentence)));
                }
                else if (negation.Sentence is ExistentialQuantification e)
                {
                    // Apply ¬∃x, p ≡ ∀x, ¬p
                    return ApplyTo(new UniversalQuantification(
                        e.Variable,
                        new Negation(e.Sentence)));
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
        /// Transformation that eliminates existential quantification via the process of "Skolemisation". Replaces all existentially declared variables
        /// with a generated "Skolem" function that acts on all universally declared variables in scope when the existential variable was declared.
        /// </summary>
        private class Skolemisation : RecursiveSentenceTransformation
        {
            private readonly Sentence rootSentence;
            private readonly IEnumerable<VariableDeclaration> universalVariablesInScope;
            private readonly Dictionary<VariableDeclaration, Function> existentialVariableMap;

            /// <summary>
            /// Initializes a new instance of the <see cref="Skolemisation"/> class.
            /// </summary>
            /// <param name="rootSentence">The root sentence being transformed. Stored against the resulting Skolem function symbols - for later explanations and the like.</param>
            public Skolemisation(Sentence rootSentence)
                : this(rootSentence, Enumerable.Empty<VariableDeclaration>(), new Dictionary<VariableDeclaration, Function>())
            {
            }

            private Skolemisation(
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
                return new UniversalQuantification(
                    universalQuantification.Variable,
                    new Skolemisation(rootSentence, universalVariablesInScope.Append(universalQuantification.Variable), existentialVariableMap).ApplyTo(universalQuantification.Sentence));
            }

            public override Sentence ApplyTo(ExistentialQuantification existentialQuantification)
            {
                // NB: don't need to validate that the variable is standardised here, because this class is private.
                existentialVariableMap[existentialQuantification.Variable] = new Function(
                    new SkolemFunctionSymbol((StandardisedVariableSymbol)existentialQuantification.Variable.Symbol, rootSentence),
                    universalVariablesInScope.Select(a => new VariableReference(a)).ToList<Term>());

                return ApplyTo(existentialQuantification.Sentence);
            }

            public override Term ApplyTo(VariableReference variable)
            {
                if (existentialVariableMap.TryGetValue(variable.Declaration, out var skolemFunction))
                {
                    return skolemFunction;
                }

                // NB: if we didn't find it in the map, then its a universally quantified variable - and we leave it alone:
                return base.ApplyTo(variable);
            }
        }

        /// <summary>
        /// Transformation that simply removes all universal quantifications.
        /// All variables in CNF sentences are assumed to be universally quantified.
        /// </summary>
        public class UniversalQuantifierElimination : RecursiveSentenceTransformation
        {
            public override Sentence ApplyTo(UniversalQuantification universalQuantification)
            {
                return ApplyTo(universalQuantification.Sentence);
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
        public class DisjunctionDistribution : RecursiveSentenceTransformation
        {
            public override Sentence ApplyTo(Disjunction disjunction)
            {
                if (disjunction.Right is Conjunction cRight)
                {
                    // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                    // NB the "else if" below is fine (i.e. we don't need a seperate case for if they are both &&s)
                    // since if b.Left is also an &&, we'll end up distributing over it once we recurse down as far
                    // as the Disjunctions we create here.
                    return ApplyTo(new Conjunction(
                        new Disjunction(disjunction.Left, cRight.Left),
                        new Disjunction(disjunction.Left, cRight.Right)));
                }
                else if (disjunction.Left is Conjunction cLeft)
                {
                    // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
                    return ApplyTo(new Conjunction(
                        new Disjunction(cLeft.Left, disjunction.Right),
                        new Disjunction(cLeft.Right, disjunction.Right)));
                }
                else
                {
                    return base.ApplyTo(disjunction);
                }
            }

            public override Sentence ApplyTo(Predicate predicate)
            {
                // There can't be any more disjunctions once we've hit an atomic sentence, so just return.
                return predicate;
            }
        }
    }
}
