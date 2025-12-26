// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Linq;

namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// Base class for recursive transformations of <see cref="Formula"/> instances to other <see cref="Formula"/> instances.
/// </summary>
public abstract class RecursiveFormulaTransformation_Linq : IFormulaTransformation<Formula>, ITermTransformation<Term>
{
    /// <summary>
    /// <para>
    /// Applies this transformation to a <see cref="Formula"/> instance.
    /// </para>
    /// <para>
    /// The default implementation uses a pattern-matching switch expression to invoke the ApplyTo method appropriate to the actual type of the formula.
    /// This is evidentally faster than calling <see cref="Formula.Accept{TOut}(IFormulaTransformation{TOut})"/>.
    /// Whatever lookup-creating shenannigans the compiler gets up to are apparently quicker than a virtual method call.
    /// </para>
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Formula formula)
    {
        return formula switch
        {
            Conjunction conjunction => ApplyTo(conjunction),
            Disjunction disjunction => ApplyTo(disjunction),
            Equivalence equivalence => ApplyTo(equivalence),
            Implication implication => ApplyTo(implication),
            Negation negation => ApplyTo(negation),
            Predicate predicate => ApplyTo(predicate),
            Quantification quantification => ApplyTo(quantification),
            _ => throw new ArgumentException("Unsupported formula type", nameof(formula))
        };
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Conjunction"/> instance.
    /// The default implementation returns a <see cref="Conjunction"/> of the result of calling <see cref="ApplyTo(Formula)"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Conjunction conjunction)
    {
        var left = ApplyTo(conjunction.Left);
        var right = ApplyTo(conjunction.Right);
        if (left != conjunction.Left || right != conjunction.Right)
        {
            return new Conjunction(left, right);
        }

        return conjunction;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Disjunction"/> instance.
    /// The default implementation returns a <see cref="Disjunction"/> of the result of calling <see cref="ApplyTo(Formula)"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Disjunction disjunction)
    {
        var left = ApplyTo(disjunction.Left);
        var right = ApplyTo(disjunction.Right);
        if (left != disjunction.Left || right != disjunction.Right)
        {
            return new Disjunction(left, right);
        }

        return disjunction;
    }

    /// <summary>
    /// Applies this transformation to an <see cref="Equivalence"/> instance. 
    /// The default implementation returns an <see cref="Equivalence"/> of the result of calling <see cref="ApplyTo(Formula)"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Equivalence equivalence)
    {
        var equivalent1 = ApplyTo(equivalence.Left);
        var equivalent2 = ApplyTo(equivalence.Right);
        if (equivalent1 != equivalence.Left || equivalent2 != equivalence.Right)
        {
            return new Equivalence(equivalent1, equivalent2);
        }

        return equivalence;
    }

    /// <summary>
    /// Applies this transformation to an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation returns an <see cref="ExistentialQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo(VariableDeclaration)"/> on the existing declaration, and the sub-formula is the result of <see cref="ApplyTo(Formula)"/> on the existing sub-formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(ExistentialQuantification existentialQuantification)
    {
        var variable = ApplyTo(existentialQuantification.Variable);
        var formula = ApplyTo(existentialQuantification.Formula);
        if (variable != existentialQuantification.Variable || formula != existentialQuantification.Formula)
        {
            return new ExistentialQuantification(variable, formula);
        }

        return existentialQuantification;
    }

    /// <summary>
    /// Applies this transformation to an <see cref="Implication"/> instance. 
    /// The default implementation returns an <see cref="Implication"/> of the result of calling <see cref="ApplyTo(Formula)"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Implication implication)
    {
        var antecedent = ApplyTo(implication.Antecedent);
        var consequent = ApplyTo(implication.Consequent);

        if (antecedent != implication.Antecedent || consequent != implication.Consequent)
        {
            return new Implication(antecedent, consequent);
        }

        return implication;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Predicate"/> instance. 
    /// The default implementation returns a <see cref="Predicate"/> with the same identifier and with an argument list that is the result of calling <see cref="ApplyTo(Term)"/> on all of the existing arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Predicate predicate)
    {
        var isChanged = false;

        var arguments = predicate.Arguments.Select(a =>
        {
            var transformed = ApplyTo(a);

            if (transformed != a)
            {
                isChanged = true;
            }

            return transformed;
        }).ToList();

        if (isChanged)
        {
            return new Predicate(predicate.Identifier, arguments);
        }

        return predicate;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Negation"/> instance. 
    /// The default implementation returns a <see cref="Negation"/> of the result of calling <see cref="ApplyTo(Formula)"/> on the current sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Negation negation)
    {
        var formula = ApplyTo(negation.Formula);

        if (formula != negation.Formula)
        {
            return new Negation(formula);
        }

        return negation;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Quantification"/> instance. 
    /// The default implementation simply invokes the ApplyTo method appropriate to the type of the quantification.
    /// </summary>
    /// <param name="quantification">The <see cref="Quantification"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Quantification quantification)
    {
        return quantification switch
        {
            ExistentialQuantification existentialQuantification => ApplyTo(existentialQuantification),
            UniversalQuantification universalQuantification => ApplyTo(universalQuantification),
            _ => throw new ArgumentException($"Unsupported Quantification type '{quantification.GetType()}'", nameof(quantification))
        };
    }

    /// <summary>
    /// Applies this transformation to a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation returns a <see cref="UniversalQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo(VariableDeclaration)"/> on the existing declaration, and the sub-formula is the result of <see cref="ApplyTo(Formula)"/> on the existing sub-formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(UniversalQuantification universalQuantification)
    {
        var variable = ApplyTo(universalQuantification.Variable);
        var formula = ApplyTo(universalQuantification.Formula);
        if (variable != universalQuantification.Variable || formula != universalQuantification.Formula)
        {
            return new UniversalQuantification(variable, formula);
        }

        return universalQuantification;
    }

    /// <summary>
    /// <para>
    /// Applies this transformation to a <see cref="Term"/> instance.
    /// </para>
    /// <para>
    /// The default implementation uses a pattern-matching switch expression to invoke the ApplyTo method appropriate to the actual type of the term.
    /// This is evidentally faster than calling <see cref="Term.Accept{TOut}(ITermTransformation{TOut})"/>.
    /// Whatever lookup-creating shenannigans the compiler gets up to are apparently quicker than a virtual method call.
    /// </para>
    /// </summary>
    /// <param name="term">The term to visit.</param>
    /// <returns>The transformed term.</returns>
    public virtual Term ApplyTo(Term term)
    {
        return term switch
        {
            VariableReference variable => ApplyTo(variable),
            Function function => ApplyTo(function),
            _ => throw new ArgumentException($"Unsupported Term type '{term.GetType()}'", nameof(term))
        };
    }

    /// <summary>
    /// Applies this transformation to a <see cref="VariableReference"/> instance.
    /// The default implementation returns a <see cref="VariableReference"/> referring to the variable that is the result of calling <see cref="ApplyTo(VariableDeclaration)"/> on the current declaration.
    /// </summary>
    /// <param name="variable">The variable to visit.</param>
    /// <returns>The transformed term.</returns>
    public virtual Term ApplyTo(VariableReference variable)
    {
        var variableDeclaration = ApplyTo(variable.Declaration);
        if (variableDeclaration != variable.Declaration)
        {
            return new VariableReference(variableDeclaration);
        }

        return variable;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Function"/> instance.
    /// The default implementation returns a <see cref="Function"/> with the same identifier and with an argument list that is the result of calling <see cref="ApplyTo(Term)"/> on each of the existing arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <returns>The transformed term.</returns>
    public virtual Term ApplyTo(Function function)
    {
        var isChanged = false;

        var arguments = function.Arguments.Select(a =>
        {
            var transformed = ApplyTo(a);

            if (transformed != a)
            {
                isChanged = true;
            }

            return transformed;
        }).ToList();

        if (isChanged)
        {
            return new Function(function.Identifier, arguments);
        }

        return function;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="VariableDeclaration"/> instance.
    /// The default implementation simply returns the passed value.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to transform.</param>
    /// <returns>The transformed <see cref="VariableReference"/> declaration.</returns>
    public virtual VariableDeclaration ApplyTo(VariableDeclaration variableDeclaration)
    {
        return variableDeclaration;
    }
}
