// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;

namespace SCFirstOrderLogic.FormulaManipulation;

/// <summary>
/// Alternative version of <see cref="RecursiveFormulaTransformation_LinqIterateTwice"/> that calls <see cref="Formula.Accept{TOut}(IFormulaTransformation{TOut})"/> instead of using a pattern-matching type switch.
/// </summary>
public class RecursiveFormulaTransformation_WithoutTypeSwitch : IFormulaTransformation<Formula>, ITermTransformation<Term>
{
    /// <summary>
    /// Applies this transformation to a <see cref="Formula"/> instance.
    /// </summary>
    /// <param name="formula">The formula to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Formula formula) => formula.Accept(this);

    /// <summary>
    /// Applies this transformation to a <see cref="Conjunction"/> instance.
    /// The default implementation returns a <see cref="Conjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Conjunction conjunction)
    {
        Formula left = conjunction.Left.Accept(this);
        Formula right = conjunction.Right.Accept(this);
        if (left != conjunction.Left || right != conjunction.Right)
        {
            return new Conjunction(left, right);
        }
        else
        {
            return conjunction;
        }
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Disjunction"/> instance.
    /// The default implementation returns a <see cref="Disjunction"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Disjunction disjunction)
    {
        Formula left = disjunction.Left.Accept(this);
        Formula right = disjunction.Right.Accept(this);
        if (left != disjunction.Left || right != disjunction.Right)
        {
            return new Disjunction(left, right);
        }
        else
        {
            return disjunction;
        }
    }

    /// <summary>
    /// Applies this transformation to an <see cref="Equivalence"/> instance. 
    /// The default implementation returns an <see cref="Equivalence"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Equivalence equivalence)
    {
        Formula left = equivalence.Left.Accept(this);
        Formula right = equivalence.Right.Accept(this);
        if (left != equivalence.Left || right != equivalence.Right)
        {
            return new Equivalence(left, right);
        }
        else
        {
            return equivalence;
        }
    }

    /// <summary>
    /// Applies this transformation to an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation returns an <see cref="ExistentialQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sub-formula is the result of <see cref="ApplyTo"/> on the existing sub-formula.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(ExistentialQuantification existentialQuantification)
    {
        VariableDeclaration variableDeclaration = ApplyTo(existentialQuantification.Variable);
        Formula formula = existentialQuantification.Formula.Accept(this);
        if (variableDeclaration != existentialQuantification.Variable || formula != existentialQuantification.Formula)
        {
            return new ExistentialQuantification(variableDeclaration, formula);
        }
        else
        {
            return existentialQuantification;
        }
    }

    /// <summary>
    /// Applies this transformation to an <see cref="Implication"/> instance. 
    /// The default implementation returns an <see cref="Implication"/> of the result of calling <see cref="ApplyTo"/> on both of the existing sub-formulas.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Implication implication)
    {
        Formula antecedent = implication.Antecedent.Accept(this);
        Formula consequent = implication.Consequent.Accept(this);

        if (antecedent != implication.Antecedent || consequent != implication.Consequent)
        {
            return new Implication(antecedent, consequent);
        }
        else
        {
            return implication;
        }
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Predicate"/> instance. 
    /// The default implementation returns a <see cref="Predicate"/> with the same identifier and with an argument list that is the result of calling <see cref="ApplyTo"/> on all of the existing arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Predicate predicate)
    {
        var isChanged = false;
        var transformed = new Term[predicate.Arguments.Count];

        for (int i = 0; i < predicate.Arguments.Count; i++)
        {
            transformed[i] = predicate.Arguments[i].Accept(this);

            if (transformed[i] != predicate.Arguments[i])
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new Predicate(predicate.Identifier, transformed);
        }
        else
        {
            return predicate;
        }
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Negation"/> instance. 
    /// The default implementation returns a <see cref="Negation"/> of the result of calling <see cref="ApplyTo"/> on the current sub-formula.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(Negation negation)
    {
        Formula formula = negation.Formula.Accept(this);

        if (formula != negation.Formula)
        {
            return new Negation(formula);
        }
        else
        {
            return negation;
        }
    }

    /// <summary>
    /// Applies this transformation to a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation returns a <see cref="UniversalQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo"/> on the existing declaration, and the sub-formula is the result of <see cref="ApplyTo"/> on the existing sub-formula.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <returns>The transformed <see cref="Formula"/>.</returns>
    public virtual Formula ApplyTo(UniversalQuantification universalQuantification)
    {
        VariableDeclaration variableDeclaration = ApplyTo(universalQuantification.Variable);
        Formula formula = universalQuantification.Formula.Accept(this);
        if (variableDeclaration != universalQuantification.Variable || formula != universalQuantification.Formula)
        {
            return new UniversalQuantification(variableDeclaration, formula);
        }
        else
        {
            return universalQuantification;
        }
    }

    /// <summary>
    /// Applies this transformation to a <see cref="VariableReference"/> instance.
    /// The default implementation returns a <see cref="VariableReference"/> of the result of calling <see cref="ApplyTo"/> on the current declaration.
    /// </summary>
    /// <param name="term">The variable to visit.</param>
    /// <returns>The transformed term.</returns>
    public virtual Term ApplyTo(VariableReference variable)
    {
        VariableDeclaration variableDeclaration = ApplyTo(variable.Declaration);

        if (variableDeclaration != variable.Declaration)
        {
            return new VariableReference(variableDeclaration);
        }
        else
        {
            return variable;
        }
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Function"/> instance.
    /// The default implementation returns a <see cref="Function"/> with the same identifier and with an argument list that is the result of calling <see cref="ApplyTo"/> on each of the existing arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <returns>The transformed term.</returns>
    public virtual Term ApplyTo(Function function)
    {
        var isChanged = false;
        var transformed = new Term[function.Arguments.Count];

        for (int i = 0; i < function.Arguments.Count; i++)
        {
            transformed[i] = function.Arguments[i].Accept(this);

            if (transformed[i] != function.Arguments[i])
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new Function(function.Identifier, transformed);
        }
        else
        {
            return function;
        }
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
