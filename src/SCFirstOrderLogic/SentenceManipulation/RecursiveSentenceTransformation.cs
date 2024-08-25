// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;

namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Base class for recursive transformations of <see cref="Sentence"/> instances to other <see cref="Sentence"/> instances.
/// </para>
/// <para>
/// That is, a base class for transformations in which the default implementation for any given non-terminal
/// sentence/term element leaves the element type unchanged and transforms the element's children - and does
/// nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveSentenceTransformation : ISentenceTransformation<Sentence>, ITermTransformation<Term>
{
    /// <summary>
    /// <para>
    /// Applies this transformation to a <see cref="Sentence"/> instance.
    /// </para>
    /// <para>
    /// The default implementation uses a pattern-matching switch expression to invoke the ApplyTo method appropriate to the actual type of the sentence.
    /// This is evidentally faster than calling <see cref="Sentence.Accept{TOut}(ISentenceTransformation{TOut})"/>.
    /// Whatever lookup-creating shenannigans the compiler gets up to are apparently quicker than a virtual method call.
    /// </para>
    /// </summary>
    /// <param name="sentence">The sentence to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Sentence sentence)
    {
        return sentence switch
        {
            Conjunction conjunction => ApplyTo(conjunction),
            Disjunction disjunction => ApplyTo(disjunction),
            Equivalence equivalence => ApplyTo(equivalence),
            Implication implication => ApplyTo(implication),
            Negation negation => ApplyTo(negation),
            Predicate predicate => ApplyTo(predicate),
            Quantification quantification => ApplyTo(quantification),
            _ => throw new ArgumentException("Unsupported sentence type", nameof(sentence))
        };
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Conjunction"/> instance.
    /// The default implementation returns a <see cref="Conjunction"/> of the result of calling <see cref="ApplyTo(Sentence)"/> on both of the existing sub-sentences.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Conjunction conjunction)
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
    /// The default implementation returns a <see cref="Disjunction"/> of the result of calling <see cref="ApplyTo(Sentence)"/> on both of the existing sub-sentences.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Disjunction disjunction)
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
    /// The default implementation returns an <see cref="Equivalence"/> of the result of calling <see cref="ApplyTo(Sentence)"/> on both of the existing sub-sentences.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Equivalence equivalence)
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
    /// The default implementation returns an <see cref="ExistentialQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo(VariableDeclaration)"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo(Sentence)"/> on the existing sentence.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(ExistentialQuantification existentialQuantification)
    {
        var variable = ApplyTo(existentialQuantification.Variable);
        var sentence = ApplyTo(existentialQuantification.Sentence);
        if (variable != existentialQuantification.Variable || sentence != existentialQuantification.Sentence)
        {
            return new ExistentialQuantification(variable, sentence);
        }

        return existentialQuantification;
    }

    /// <summary>
    /// Applies this transformation to an <see cref="Implication"/> instance. 
    /// The default implementation returns an <see cref="Implication"/> of the result of calling <see cref="ApplyTo(Sentence)"/> on both of the existing sub-sentences.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Implication implication)
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
    /// <param name="predicate">The <see cref="Predicate"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Predicate predicate)
    {
        var isChanged = false;
        var transformed = new Term[predicate.Arguments.Count];

        for (int i = 0; i < predicate.Arguments.Count; i++)
        {
            transformed[i] = ApplyTo(predicate.Arguments[i]);

            if (transformed[i] != predicate.Arguments[i])
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new Predicate(predicate.Identifier, transformed);
        }

        return predicate;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Negation"/> instance. 
    /// The default implementation returns a <see cref="Negation"/> of the result of calling <see cref="ApplyTo(Sentence)"/> on the current sub-sentence.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Negation negation)
    {
        var sentence = ApplyTo(negation.Sentence);

        if (sentence != negation.Sentence)
        {
            return new Negation(sentence);
        }

        return negation;
    }

    /// <summary>
    /// Applies this transformation to a <see cref="Quantification"/> instance. 
    /// The default implementation simply invokes the ApplyTo method appropriate to the type of the quantification.
    /// </summary>
    /// <param name="quantification">The <see cref="Quantification"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(Quantification quantification)
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
    /// The default implementation returns a <see cref="UniversalQuantification"/> for which the variable declaration is the result of <see cref="ApplyTo(VariableDeclaration)"/> on the existing declaration, and the sentence is the result of <see cref="ApplyTo(Sentence)"/> on the existing sentence.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to transform.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual Sentence ApplyTo(UniversalQuantification universalQuantification)
    {
        var variable = ApplyTo(universalQuantification.Variable);
        var sentence = ApplyTo(universalQuantification.Sentence);
        if (variable != universalQuantification.Variable || sentence != universalQuantification.Sentence)
        {
            return new UniversalQuantification(variable, sentence);
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
    /// <param name="term">The term to transform.</param>
    /// <returns>The transformed <see cref="Term"/>.</returns>
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
    /// <param name="variable">The variable to transform.</param>
    /// <returns>The transformed <see cref="Term"/>.</returns>
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
    /// <param name="function">The function to transform.</param>
    /// <returns>The transformed <see cref="Term"/>.</returns>
    public virtual Term ApplyTo(Function function)
    {
        var isChanged = false;
        var transformed = new Term[function.Arguments.Count];

        for (int i = 0; i < function.Arguments.Count; i++)
        {
            transformed[i] = ApplyTo(function.Arguments[i]);

            if (transformed[i] != function.Arguments[i])
            {
                isChanged = true;
            }
        }

        if (isChanged)
        {
            return new Function(function.Identifier, transformed);
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
