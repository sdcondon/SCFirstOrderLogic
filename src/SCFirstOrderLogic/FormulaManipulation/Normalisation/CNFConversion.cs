// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

/// <summary>
/// <para>
/// Converts formulas to conjunctive normal form.
/// </para>
/// <para>
/// This type is internal because its output is not *completely* normalised, since it *doesn't* normalise the
/// order of evaluation of the clauses (i.e. the conjunctions found at the root of the output formula),
/// or the literals within those clauses (i.e. disjunctions found below those top-level conjunctions).
/// The <see cref="CNFFormula"/> class does that. If there is ever a need to expose this class publicly (unlikely -
/// <see cref="CNFFormula"/> should suffice), I'd want to add some more robustness stuff to eliminate potential
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
    /// universal quantifier elimination, and disjunction distribution) to a given <see cref="Formula"/> instance.
    /// </summary>
    /// <param name="formula">The formula to convert.</param>
    /// <returns>The transformed formula.</returns>
    public static Formula ApplyTo(Formula formula)
    {
        // We do variable standardisation first, before altering the formula in any
        // other way, so that we can easily store the context of the original variable in the new identifier. This
        // facilitates explanations of the origins of a particular variable (or Skolem function) in query result explanations.
        // TODO-ROBUSTNESS: If users include undeclared variables on the assumption they'll be treated as 
        // universally quantified and formula-wide in scope, the behaviour is going to be, well, wrong.
        // Should we validate here..? Or handle on the assumption that they are universally quantified?
        // Trying to write some tests for this should help in establishing 'nice' behaviour.
        // Also should probably complain when nested definitions uses the same identifier (i.e. identifiers that are equal).
        formula = new VariableStandardisation(formula).ApplyTo(formula);
        var standardisedFormula = formula;

        // It might be possible to do some of these conversions at the same time, but for now
        // at least we do them sequentially - and in so doing favour maintainability over performance.
        // Perhaps revisit this later.
        formula = implicationElimination.ApplyTo(formula);
        formula = nnfConversion.ApplyTo(formula);
        formula = new Skolemisation(standardisedFormula).ApplyTo(formula);
        formula = universalQuantifierElimination.ApplyTo(formula);
        formula = disjunctionDistribution.ApplyTo(formula);

        return formula;
    }

    /// <summary>
    /// Transformation that "standardises apart" variables - essentially ensuring that variable identifiers are unique.
    /// </summary>
    public class VariableStandardisation : RecursiveFormulaTransformation
    {
        private readonly Dictionary<VariableDeclaration, VariableDeclaration> mapping = new();
        private readonly Formula rootFormula;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableStandardisation"/> class.
        /// </summary>
        /// <param name="rootFormula">
        /// The root formula being transformed. Stored against identifiers of the new standardised versions of the variables, for later use in explanations and the like.
        /// </param>
        public VariableStandardisation(Formula rootFormula)
        {
            this.rootFormula = rootFormula;
        }

        public override Formula ApplyTo(Quantification quantification)
        {
            // Should we throw if the variable being standardised is already standardised? Or return it unchanged?
            // Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
            mapping[quantification.Variable] = new VariableDeclaration(new StandardisedVariableIdentifier(quantification, rootFormula));
            return base.ApplyTo(quantification);
        }

        public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
        {
            // Undeclared variables are assumed to be (universal in nature and) formula-wide in scope.
            if (!mapping.TryGetValue(variableDeclaration, out var standardisedVariableDeclaration))
            {
                // Should we throw if the variable being standardised is already standardised? Or return it unchanged?
                // Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
                // TODO-ZZ-ROBUSTNESS: This creation of implicit scope is hacky. In particular, think about the inconsistency when there are multiple. Is this a problem? Ponder me.
                var implicitScope = new UniversalQuantification(variableDeclaration, rootFormula);
                standardisedVariableDeclaration = mapping[variableDeclaration] = new VariableDeclaration(new StandardisedVariableIdentifier(implicitScope, rootFormula));
            }

            return standardisedVariableDeclaration;
        }
    }

    /// <summary>
    /// Transformation that eliminates implications by replacing P ⇒ Q with ¬P ∨ Q and P ⇔ Q with (¬P ∨ Q) ∧ (P ∨ ¬Q)
    /// </summary>
    public class ImplicationElimination : RecursiveFormulaTransformation
    {
        /// <inheritdoc />
        public override Formula ApplyTo(Implication implication)
        {
            // Convert  P ⇒ Q to ¬P ∨ Q 
            return ApplyTo(new Disjunction(
                new Negation(implication.Antecedent),
                implication.Consequent));
        }

        /// <inheritdoc />
        public override Formula ApplyTo(Equivalence equivalence)
        {
            // Convert P ⇔ Q to (¬P ∨ Q) ∧ (P ∨ ¬Q)
            return ApplyTo(new Conjunction(
                new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                new Disjunction(equivalence.Left, new Negation(equivalence.Right))));
        }

        public override Formula ApplyTo(Predicate predicate)
        {
            // There can't be any more implications once we've hit an atomic formula, so just return.
            return predicate;
        }
    }

    /// <summary>
    /// Transformation that converts to Negation Normal Form by moving negations as far as possible from the root of the formula tree.
    /// That is, directly applied to predicates.
    /// </summary>
    public class NNFConversion : RecursiveFormulaTransformation
    {
        /// <inheritdoc />
        public override Formula ApplyTo(Negation negation)
        {
            if (negation.Formula is Negation n)
            {
                // Eliminate double negative: ¬(¬P) ≡ P
                return ApplyTo(n.Formula);
            }
            else if (negation.Formula is Conjunction c)
            {
                // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                return ApplyTo(new Disjunction(
                    new Negation(c.Left),
                    new Negation(c.Right)));
            }
            else if (negation.Formula is Disjunction d)
            {
                // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                return ApplyTo(new Conjunction(
                    new Negation(d.Left),
                    new Negation(d.Right)));
            }
            else if (negation.Formula is UniversalQuantification u)
            {
                // Apply ¬∀x, p ≡ ∃x, ¬p
                return ApplyTo(new ExistentialQuantification(
                    u.Variable,
                    new Negation(u.Formula)));
            }
            else if (negation.Formula is ExistentialQuantification e)
            {
                // Apply ¬∃x, p ≡ ∀x, ¬p
                return ApplyTo(new UniversalQuantification(
                    e.Variable,
                    new Negation(e.Formula)));
            }
            else
            {
                return base.ApplyTo(negation);
            }
        }

        public override Formula ApplyTo(Predicate predicate)
        {
            // There can't be any more negations once we've hit an atomic formula, so just return.
            return predicate;
        }
    }

    /// <summary>
    /// Transformation that eliminates existential quantification via the process of "Skolemisation". Replaces all existentially declared variables
    /// with a generated "Skolem" function that acts on all universally declared variables in scope when the existential variable was declared.
    /// </summary>
    private class Skolemisation : RecursiveFormulaTransformation
    {
        private readonly Formula rootFormula;
        private readonly IEnumerable<VariableDeclaration> universalVariablesInScope;
        private readonly Dictionary<VariableDeclaration, Function> existentialVariableMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="Skolemisation"/> class.
        /// </summary>
        /// <param name="rootFormula">The root formula being transformed. Stored against the resulting Skolem function identifiers - for later explanations and the like.</param>
        public Skolemisation(Formula rootFormula)
            : this(rootFormula, Enumerable.Empty<VariableDeclaration>(), new Dictionary<VariableDeclaration, Function>())
        {
        }

        private Skolemisation(
            Formula rootFormula,
            IEnumerable<VariableDeclaration> universalVariablesInScope,
            Dictionary<VariableDeclaration, Function> existentialVariableMap)
        {
            this.rootFormula = rootFormula;
            this.universalVariablesInScope = universalVariablesInScope;
            this.existentialVariableMap = existentialVariableMap;
        }

        public override Formula ApplyTo(UniversalQuantification universalQuantification)
        {
            return new UniversalQuantification(
                universalQuantification.Variable,
                new Skolemisation(rootFormula, universalVariablesInScope.Append(universalQuantification.Variable), existentialVariableMap).ApplyTo(universalQuantification.Formula));
        }

        public override Formula ApplyTo(ExistentialQuantification existentialQuantification)
        {
            // NB: don't need to validate that the variable is standardised here, because this class is private.
            existentialVariableMap[existentialQuantification.Variable] = new Function(
                new SkolemFunctionIdentifier((StandardisedVariableIdentifier)existentialQuantification.Variable.Identifier, rootFormula),
                universalVariablesInScope.Select(a => new VariableReference(a)).ToList<Term>());

            return ApplyTo(existentialQuantification.Formula);
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
    /// All variables in CNF formulas are assumed to be universally quantified.
    /// </summary>
    public class UniversalQuantifierElimination : RecursiveFormulaTransformation
    {
        public override Formula ApplyTo(UniversalQuantification universalQuantification)
        {
            return ApplyTo(universalQuantification.Formula);
        }

        public override Formula ApplyTo(Predicate predicate)
        {
            // There can't be any more quantifications once we've hit an atomic formula, so just return.
            return predicate;
        }
    }

    /// <summary>
    /// Transformation that recursively distributes disjunctions over conjunctions.
    /// </summary>
    public class DisjunctionDistribution : RecursiveFormulaTransformation
    {
        public override Formula ApplyTo(Disjunction disjunction)
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

        public override Formula ApplyTo(Predicate predicate)
        {
            // There can't be any more disjunctions once we've hit an atomic formula, so just return.
            return predicate;
        }
    }
}
