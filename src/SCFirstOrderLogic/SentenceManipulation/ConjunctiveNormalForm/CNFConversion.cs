using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    /// <summary>
    /// Implementation of <see cref="SentenceTransformation"/> that converts sentences to conjunctive normal form.
    /// <para/>
    /// Well.. It's arguable whether the output could be considered *completely* normalised, since it *won't* normalise the
    /// order of evaluation of the clauses (i.e. the conjunctions found at the root of the output sentence),
    /// or the literals within those clauses (i.e. disjunctions found below those top-level conjunctions).
    /// The <see cref="CNFSentence"/> class does that (TODO: not true any more - ordering apparently not as useful as in propositional logic). It is because of this (and because the half-job done by this class is of limited
    /// use on its own) that this class should probably be internal.
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
            // It might be possible to do some of these conversions at the same time, but for now
            // at least we do them sequentially - and in so doing favour maintainability over performance.
            // Revisit this later.
            sentence = implicationElimination.ApplyTo(sentence);
            sentence = nnfConversion.ApplyTo(sentence);
            sentence = variableStandardisation.ApplyTo(sentence);
            sentence = skolemisation.ApplyTo(sentence);
            sentence = universalQuantifierElimination.ApplyTo(sentence);
            sentence = disjunctionDistribution.ApplyTo(sentence);

            // TODO-ROBUSTNESS: If users include undeclared variables on the assumption they'll be treated as 
            // universally quantified and sentence-wide in scope, the behaviour is going to be, well, wrong.
            // Should we validate here..? Or handle on the assumption that they are universally quantified?

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
                // Convert  P ⇒ Q to ¬P ∨ Q 
                return ApplyTo(new Disjunction(
                    new Negation(implication.Antecedent),
                    implication.Consequent));
            }

            /// <inheritdoc />
            protected override Sentence ApplyTo(Equivalence equivalence)
            {
                // Convert P ⇔ Q to (¬P ∨ Q) ∧ (P ∨ ¬Q)
                return ApplyTo(new Conjunction(
                    new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                    new Disjunction(equivalence.Left, new Negation(equivalence.Right))));
            }
        }

        /// <summary>
        /// Transformation that converts to Negation Normal Form by moving negations as far as possible from the root of the sentence tree.
        /// That is, directly applied to predicates.
        /// </summary>
        private class NNFConversion : SentenceTransformation
        {
            /// <inheritdoc />
            protected override Sentence ApplyTo(Negation negation)
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
        }

        /// <summary>
        /// Tranformation that "standardises apart" variables - essentially ensuring that variable symbols are unique.
        /// </summary>
        private class VariableStandardisation : SentenceTransformation
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Sentence sentence)
            {
                return new ScopedVariableStandardisation().ApplyTo(sentence);
            }

            // NB: A "Transformation" that doesn't transform and has to use a property to expose its output. More evidence to suggest introduction of visitor pattern at some point.
            private class ScopedVariableStandardisation : SentenceTransformation
            {
                private readonly Dictionary<VariableDeclaration, VariableDeclaration> mapping = new Dictionary<VariableDeclaration, VariableDeclaration>();

                protected override Sentence ApplyTo(Quantification quantification)
                {
                    mapping[quantification.Variable] = new VariableDeclaration(new StandardisedVariableSymbol(quantification.Variable.Symbol));
                    return base.ApplyTo(quantification);
                }

                protected override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
                {
                    return mapping[variableDeclaration];
                }
            }
        }

        /// <summary>
        /// Class for symbols of variables that have been standardised.
        /// <para/>
        /// NB: Doesn't override equality or hash code, so uses reference equality;
        /// and we create exactly one instance per variable scope - thus achieving standardisation
        /// without having to muck about with anything like trying to ensure names that are unique strings
        /// (which should only be a rendering concern anyway).
        /// </summary>
        /// <remarks>
        /// TODO-TESTABILITY: Difficult to test. Would much rather implement value semantics for equality.
        /// Same variable in same sentence is the same (inside the var store sentence tree re-arranged so that var is the root
        /// equality would need to avoid infinite loop though. and couldn't work on output of this CNFConversion since this
        /// class doesn't completely normalise. Perhaps made easier by the fact that after normalisation, all surviving variables
        /// are universally quantified).
        /// <para/>
        /// Also: should we throw if the variable being standardised is already standardised? Or return it unchanged?
        /// Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
        /// </remarks>
        public class StandardisedVariableSymbol
        {
            internal StandardisedVariableSymbol(object underlyingSymbol) => UnderlyingSymbol = underlyingSymbol;

            /// <summary>
            /// Gets the underlying variable symbol that this symbol is the standardisation of.
            /// </summary>
            public object UnderlyingSymbol { get; }

            /// <inheritdoc/>
            /// <remarks>
            /// NB: SentenceFormatter has a special case when rendering these (to ensure that they are rendered distinctly),
            /// so this ToString override is "just in case".
            /// </remarks>
            public override string ToString() => $"ST:{UnderlyingSymbol}";
        }

        /// <summary>
        /// Transformation that eliminate existential quantification via the process of "Skolemisation". Replaces all existentially declared variables
        /// with a generated "Skolem" function that acts on all universally declared variables in scope when the existential variable was declared.
        /// <para/>
        /// NB: Doesn't override equality or hash code, so uses reference equality;
        /// and we create exactly one instance per variable scope - thus achieving standardisation
        /// without having to muck about with anything like trying to ensure names that are unique strings
        /// (which should only be a rendering concern anyway).
        /// </summary>
        /// <remarks>
        /// As with standardised variables, would prefer to use value semantics for equality.
        /// </remarks>
        private class Skolemisation : SentenceTransformation
        {
            /// <inheritdoc />
            public override Sentence ApplyTo(Sentence sentence)
            {
                return new ScopedSkolemisation(Enumerable.Empty<VariableDeclaration>(), new Dictionary<VariableDeclaration, Function>()).ApplyTo(sentence);
            }

            private class ScopedSkolemisation : SentenceTransformation
            {
                private readonly IEnumerable<VariableDeclaration> universalVariablesInScope;
                private readonly Dictionary<VariableDeclaration, Function> existentialVariableMap;

                public ScopedSkolemisation(IEnumerable<VariableDeclaration> universalVariablesInScope, Dictionary<VariableDeclaration, Function> existentialVariableMap)
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
                    existentialVariableMap[existentialQuantification.Variable] = new Function(
                        new SkolemFunctionSymbol(existentialQuantification.Variable.Symbol),
                        universalVariablesInScope.Select(a => new VariableReference(a)).ToList<Term>());
                    return base.ApplyTo(existentialQuantification.Sentence);
                }

                protected override Term ApplyTo(VariableReference variable)
                {
                    if (existentialVariableMap.TryGetValue(variable.Declaration, out var skolemFunction))
                    {
                        return skolemFunction;
                    }

                    // NB: if we didn't find it in the map, its a universally quantified variable - and we leave it alone:
                    return base.ApplyTo(variable);
                }
            }
        }

        /// <summary>
        /// Class for Skolem function symbols.
        /// </summary>
        public class SkolemFunctionSymbol
        {
            internal SkolemFunctionSymbol(object underlyingSymbol) => UnderlyingSymbol = underlyingSymbol;

            /// <summary>
            /// Gets the underlying (existentially quantified) variable symbol that this symbol is the standardisation of.
            /// </summary>
            public object UnderlyingSymbol { get; }

            /// <inheritdoc/>
            /// <remarks>
            /// NB: SentenceFormatter has a special case when rendering these (to ensure that they are rendered distinctly),
            /// so this ToString override is "just in case".
            /// </remarks>
            public override string ToString() => $"SK:{UnderlyingSymbol}";
        }

        /// <summary>
        /// Transformation that simply removes all universal quantifications.
        /// All variables in CNF sentences are assumed to be universally quantified.
        /// </summary>
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
