// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// Base class for recursive visitors of <see cref="Sentence"/> instances that reference external state.
/// </summary>
public abstract class RecursiveSentenceVisitor_WithTypeSwitch<TState> : ISentenceVisitorR<TState>, ITermVisitorR<TState>
{
    /// <summary>
    /// Applies this transformation to a <see cref="Sentence"/> instance.
    /// </summary>
    /// <param name="sentence">The sentence to visit.</param>
    /// <returns>The transformed <see cref="Sentence"/>.</returns>
    public virtual void Visit(Sentence sentence, ref TState state)
    {
        switch (sentence)
        {
            case Conjunction conjunction:
                Visit(conjunction, ref state);
                break;
            case Disjunction disjunction:
                Visit(disjunction, ref state);
                break;
            case Equivalence equivalence:
                Visit(equivalence, ref state);
                break;
            case Implication implication:
                Visit(implication, ref state);
                break;
            case Negation negation:
                Visit(negation, ref state);
                break;
            case Predicate predicate:
                Visit(predicate, ref state);
                break;
            case Quantification quantification:
                Visit(quantification, ref state);
                break;
            default:
                throw new ArgumentException($"Unsupported sentence type '{sentence.GetType()}'", nameof(sentence));
        };
    }

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-sentences.
    /// </summary>
    /// <param name="conjunction">The conjunction instance to visit.</param>
    public virtual void Visit(Conjunction conjunction, ref TState state)
    {
        Visit(conjunction.Left, ref state);
        Visit(conjunction.Right, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-sentences.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    public virtual void Visit(Disjunction disjunction, ref TState state)
    {
        Visit(disjunction.Left, ref state);
        Visit(disjunction.Right, ref state);
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-sentences.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    public virtual void Visit(Equivalence equivalence, ref TState state)
    {
        Visit(equivalence.Left, ref state);
        Visit(equivalence.Right, ref state);
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sentence.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    public virtual void Visit(ExistentialQuantification existentialQuantification, ref TState state)
    {
        Visit(existentialQuantification.Variable, ref state);
        Visit(existentialQuantification.Sentence, ref state);
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-sentences.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    public virtual void Visit(Implication implication, ref TState state)
    {
        Visit(implication.Antecedent, ref state);
        Visit(implication.Consequent, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    public virtual void Visit(Predicate predicate, ref TState state)
    {
        foreach (var argument in predicate.Arguments)
        {
            Visit(argument, ref state);
        }
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-sentence.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    public virtual void Visit(Negation negation, ref TState state)
    {
        Visit(negation.Sentence, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Quantification"/> instance. 
    /// The default implementation simply invokes the Visit method appropriate to the type of the quantification.
    /// </summary>
    /// <param name="quantification">The <see cref="Quantification"/> instance to visit.</param>
    public virtual void Visit(Quantification quantification, ref TState state)
    {
        switch (quantification)
        {
            case ExistentialQuantification existentialQuantification:
                Visit(existentialQuantification, ref state);
                break;
            case UniversalQuantification universalQuantification:
                Visit(universalQuantification, ref state);
                break;
            default:
                throw new ArgumentException($"Unsupported Quantification type '{quantification.GetType()}'", nameof(quantification));
        }
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sentence.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    public virtual void Visit(UniversalQuantification universalQuantification, ref TState state)
    {
        Visit(universalQuantification.Variable, ref state);
        Visit(universalQuantification.Sentence, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the term.
    /// </summary>
    /// <param name="term">The term to visit.</param>
    public virtual void Visit(Term term, ref TState state)
    {
        switch (term)
        {
            case Constant constant:
                Visit(constant, ref state);
                break;
            case VariableReference variable:
                Visit(variable, ref state);
                break;
            case Function function:
                Visit(function, ref state);
                break;
            default:
                throw new ArgumentException($"Unsupported Term type '{term.GetType()}'", nameof(term));
        }
    }

    /// <summary>
    /// Visits a <see cref="Constant"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="term">The constant to visit.</param>
    public virtual void Visit(Constant constant, ref TState state)
    {
    }

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    public virtual void Visit(VariableReference variable, ref TState state)
    {
        Visit(variable.Declaration, ref state);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    public virtual void Visit(Function function, ref TState state)
    {
        foreach (var argument in function.Arguments)
        {
            Visit(argument, ref state);
        }
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    public virtual void Visit(VariableDeclaration variableDeclaration, ref TState state)
    {
    }
}
