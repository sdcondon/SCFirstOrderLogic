// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

public static class CNFConversion_WithoutTypeSwitch
{
    private static readonly ImplicationElimination implicationElimination = new();
    private static readonly NNFConversion nnfConversion = new();
    private static readonly UniversalQuantifierElimination universalQuantifierElimination = new();
    private static readonly DisjunctionDistribution disjunctionDistribution = new();

    /// <inheritdoc />
    public static Formula ApplyTo(Formula formula)
    {
        // We do variable standardisation first, before altering the formula in any
        // other way, so that we can easily store the context of the original variable in the new identifier. This
        // facilitates explanations of the origins of a particular variable (or Skolem function) in query result explanations.
        // NOT-TODO-ROBUSTNESS: If users include undeclared variables on the assumption they'll be treated as 
        // universally quantified and formula-wide in scope, the behaviour is going to be, well, wrong.
        // Should we validate here..? Or handle on the assumption that they are universally quantified?
        // Also should probably complain when nested definitions uses the same identifier (i.e. identifiers that are equal).
        formula = new VariableStandardisation(formula).ApplyTo(formula);

        // It might be possible to do some of these conversions at the same time, but for now
        // at least we do them sequentially - and in so doing favour maintainability over performance.
        // Perhaps revisit this later (but given that the main purpose of this library is learning, probably not).
        formula = formula.Accept(implicationElimination);
        formula = formula.Accept(nnfConversion);
        formula = new Skolemisation(formula).ApplyTo(formula);
        formula = formula.Accept(universalQuantifierElimination);
        formula = formula.Accept(disjunctionDistribution);

        return formula;
    }

    /// <summary>
    /// <para>
    /// Transformation that "standardises apart" variables - essentially ensuring that variable identifiers are unique.
    /// </para>
    /// <para>
    /// Public to allow callers to mess about with the normalisation process.
    /// </para>
    /// </summary>
    // Private inner class to hide necessarily short-lived object away from callers.
    // Would feel a bit uncomfortable publicly exposing a transformation class that can only be applied once.
    public class VariableStandardisation : RecursiveFormulaTransformation_WithoutTypeSwitch
    {
        private readonly Dictionary<VariableDeclaration, VariableDeclaration> mapping = new();
        private readonly Formula rootFormula;

        public VariableStandardisation(Formula rootFormula)
        {
            this.rootFormula = rootFormula;
        }

        public override Formula ApplyTo(ExistentialQuantification existentialQuantification)
        {
            /// Should we throw if the variable being standardised is already standardised? Or return it unchanged?
            /// Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
            mapping[existentialQuantification.Variable] = new VariableDeclaration(new StandardisedVariableIdentifier(existentialQuantification, rootFormula));
            return base.ApplyTo(existentialQuantification);
        }

        public override Formula ApplyTo(UniversalQuantification universalQuantification)
        {
            /// Should we throw if the variable being standardised is already standardised? Or return it unchanged?
            /// Just thinking about robustness in the face of weird usages potentially resulting in stuff being normalised twice?
            mapping[universalQuantification.Variable] = new VariableDeclaration(new StandardisedVariableIdentifier(universalQuantification, rootFormula));
            return base.ApplyTo(universalQuantification);
        }

        public override VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
        {
            return mapping[variableDeclaration];
        }
    }

    /// <summary>
    /// Transformation that eliminates implications by replacing P ⇒ Q with ¬P ∨ Q and P ⇔ Q with (¬P ∨ Q) ∧ (P ∨ ¬Q)
    /// </summary>
    public class ImplicationElimination : RecursiveFormulaTransformation_WithoutTypeSwitch
    {
        /// <inheritdoc />
        public override Formula ApplyTo(Implication implication)
        {
            // Convert  P ⇒ Q to ¬P ∨ Q 
            var replacement = new Disjunction(new Negation(implication.Antecedent), implication.Consequent);
            return replacement.Accept(this);
        }

        /// <inheritdoc />
        public override Formula ApplyTo(Equivalence equivalence)
        {
            // Convert P ⇔ Q to (¬P ∨ Q) ∧ (P ∨ ¬Q)
            var replacement = new Conjunction(
                new Disjunction(new Negation(equivalence.Left), equivalence.Right),
                new Disjunction(equivalence.Left, new Negation(equivalence.Right)));
            return replacement.Accept(this);
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
    public class NNFConversion : RecursiveFormulaTransformation_WithoutTypeSwitch
    {
        /// <inheritdoc />
        public override Formula ApplyTo(Negation negation)
        {
            if (negation.Formula is Negation n)
            {
                // Eliminate double negative: ¬(¬P) ≡ P
                return n.Formula.Accept(this);
            }
            else if (negation.Formula is Conjunction c)
            {
                // Apply de Morgan: ¬(P ∧ Q) ≡ (¬P ∨ ¬Q)
                var replacement = new Disjunction(
                    new Negation(c.Left),
                    new Negation(c.Right));
                return replacement.Accept(this);
            }
            else if (negation.Formula is Disjunction d)
            {
                // Apply de Morgan: ¬(P ∨ Q) ≡ (¬P ∧ ¬Q)
                var replacement = new Conjunction(
                    new Negation(d.Left),
                    new Negation(d.Right));
                return replacement.Accept(this);
            }
            else if (negation.Formula is UniversalQuantification u)
            {
                // Apply ¬∀x, p ≡ ∃x, ¬p
                var replacement = new ExistentialQuantification(
                    u.Variable,
                    new Negation(u.Formula));
                return replacement.Accept(this);
            }
            else if (negation.Formula is ExistentialQuantification e)
            {
                // Apply ¬∃x, p ≡ ∀x, ¬p
                var replacement = new UniversalQuantification(
                    e.Variable,
                    new Negation(e.Formula));
                return replacement.Accept(this);
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
    /// Transformation that eliminate existential quantification via the process of "Skolemisation". Replaces all existentially declared variables
    /// with a generated "Skolem" function that acts on all universally declared variables in scope when the existential variable was declared.
    /// </summary>
    // Private inner class to hide necessarily short-lived object away from callers.
    // Would feel a bit uncomfortable publicly exposing a transformation class that can only be applied once.
    private class Skolemisation : RecursiveFormulaTransformation_WithoutTypeSwitch
    {
        private readonly Formula rootFormula;
        private readonly IEnumerable<VariableDeclaration> universalVariablesInScope;
        private readonly Dictionary<VariableDeclaration, Function> existentialVariableMap;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScopedSkolemisation"/> class.
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
            Formula formula = universalQuantification.Formula.Accept(new Skolemisation(
                rootFormula,
                universalVariablesInScope.Append(universalQuantification.Variable), existentialVariableMap));

            return new UniversalQuantification(universalQuantification.Variable, formula);
        }

        public override Formula ApplyTo(ExistentialQuantification existentialQuantification)
        {
            existentialVariableMap[existentialQuantification.Variable] = new Function(
                new SkolemFunctionIdentifier((StandardisedVariableIdentifier)existentialQuantification.Variable.Identifier, rootFormula),
                universalVariablesInScope.Select(a => new VariableReference(a)).ToList<Term>());

            return existentialQuantification.Formula.Accept(this);
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

    /// <summary>
    /// Transformation that simply removes all universal quantifications.
    /// All variables in CNF formulas are assumed to be universally quantified.
    /// </summary>
    public class UniversalQuantifierElimination : RecursiveFormulaTransformation_WithoutTypeSwitch
    {
        public override Formula ApplyTo(UniversalQuantification universalQuantification)
        {
            return universalQuantification.Formula.Accept(this);
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
    public class DisjunctionDistribution : RecursiveFormulaTransformation_WithoutTypeSwitch
    {
        public override Formula ApplyTo(Disjunction disjunction)
        {
            Formula formula;

            if (disjunction.Right is Conjunction cRight)
            {
                // Apply distribution of ∨ over ∧: (α ∨ (β ∧ γ)) ≡ ((α ∨ β) ∧ (α ∨ γ))
                // NB the "else if" below is fine (i.e. we don't need a seperate case for if they are both &&s)
                // since if b.Left is also an &&, well end up distributing over it once we recurse down as far
                // as the Expression.OrElses we create here.
                formula = new Conjunction(
                    new Disjunction(disjunction.Left, cRight.Left),
                    new Disjunction(disjunction.Left, cRight.Right));
            }
            else if (disjunction.Left is Conjunction cLeft)
            {
                // Apply distribution of ∨ over ∧: ((β ∧ γ) ∨ α) ≡ ((β ∨ α) ∧ (γ ∨ α))
                formula = new Conjunction(
                    new Disjunction(cLeft.Left, disjunction.Right),
                    new Disjunction(cLeft.Right, disjunction.Right));
            }
            else
            {
                return base.ApplyTo(disjunction);
            }

            return formula.Accept(this);
        }

        public override Formula ApplyTo(Predicate predicate)
        {
            // There can't be any more disjunctions once we've hit an atomic formula, so just return.
            return predicate;
        }
    }
}
