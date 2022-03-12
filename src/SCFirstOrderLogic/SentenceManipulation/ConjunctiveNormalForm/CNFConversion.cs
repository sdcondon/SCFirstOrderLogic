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
    /// The <see cref="CNFSentence"/> class does that. It is because of this (and because the half-job done by this class is of limited
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
            // universally quantified and sentence-wide in scope, the behaviour is going to be, well, wrong. Should validate here..?
            // Or handle on the assumption that they are universally quantified?

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

            private class StandardisedVariableSymbol
            {
                private readonly object underlyingSymbol;

                public StandardisedVariableSymbol(object underlyingSymbol) => this.underlyingSymbol = underlyingSymbol;

                public override string ToString() => underlyingSymbol.ToString(); // Should we do.. something to indicate that its standardised?

                //// NB: Doesn't override equality or hash code, so uses reference equality -
                //// and we create exactly one instance per variable scope - thus achieving standardisation
                //// without having to muck about with trying to ensure names that are unique strings.
                //// TODO-TESTABILITY: Difficult to test. Would much rather implement value semantics for equality.
                //// Same variable in same sentence is the same (inside the var store sentence tree re-arranged so that var is the root
                //// equality would need to avoid infinite loop though. and couldn't work on output of this CNFConversion since this
                //// class doesn't completely normalise. Perhaps made easier by the fact that after normalisation, all surviving variables
                //// are universally quantified)
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
                    // TODO-MAINTAINABILITY: Skolem function equality shouldn't be on name.
                    // Skolem function equality should be based on being the same function (at the same location)
                    // in the same original sentence (note "original" - else equality would be circular)
                    existentialVariableMap[existentialQuantification.Variable] = new Function(
                        new SkolemFunctionSymbol($"Skolem{existentialVariableMap.Count + 1}"),
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

            // Use our own symbol class rather than just a string for Skolem function symbols to eliminate
            // the possibility of Skolem functions clashing with unfortunately-named user-provided functions.
            private class SkolemFunctionSymbol
            {
                private readonly string name;

                public SkolemFunctionSymbol(string name) => this.name = name;

                public override string ToString() => name;

                public override bool Equals(object obj) => obj is SkolemFunctionSymbol skolem && skolem.name.Equals(name);

                public override int GetHashCode() => HashCode.Combine(name);
            }
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
