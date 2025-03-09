﻿// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Linq;
using System.Threading.Tasks;

namespace SCFirstOrderLogic.SentenceManipulation;

/// <summary>
/// <para>
/// Base class for recursive asynchronous visitors of <see cref="Sentence"/> instances that reference external state.
/// </para>
/// <para>
/// That is, a base class for visitors in which the default implementation for any non-terminal
/// sentence/term element simply visits the element's children - and does nothing for terminal elements.
/// </para>
/// </summary>
public abstract class RecursiveAsyncSentenceVisitor<TState> : IAsyncSentenceVisitor<TState>, IAsyncTermVisitor<TState>
{
    /// <summary>
    /// Visits a <see cref="Sentence"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the sentence (via <see cref="Sentence.Accept{TState}(ISentenceVisitor{TState}, TState)"/>.
    /// </summary>
    /// <param name="sentence">The sentence to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Sentence sentence, TState state)
    {
        await sentence.AcceptAsync(this, state);
    }

    /// <summary>
    /// Visits a <see cref="Conjunction"/> instance.
    /// The default implementation just visits both of the sub-sentences.
    /// </summary>
    /// <param name="conjunction">The <see cref="Conjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Conjunction conjunction, TState state)
    {
        await Task.WhenAll(
            VisitAsync(conjunction.Left, state),
            VisitAsync(conjunction.Right, state));
    }

    /// <summary>
    /// Visits a <see cref="Disjunction"/> instance.
    /// The default implementation just visits the both of the sub-sentences.
    /// </summary>
    /// <param name="disjunction">The <see cref="Disjunction"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Disjunction disjunction, TState state)
    {
        await Task.WhenAll(
            VisitAsync(disjunction.Left, state),
            VisitAsync(disjunction.Right, state));
    }

    /// <summary>
    /// Visits an <see cref="Equivalence"/> instance. 
    /// The default implementation just visits both of the sub-sentences.
    /// </summary>
    /// <param name="equivalence">The <see cref="Equivalence"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Equivalence equivalence, TState state)
    {
        await Task.WhenAll(
            VisitAsync(equivalence.Left, state),
            VisitAsync(equivalence.Right, state));
    }

    /// <summary>
    /// Visits an <see cref="ExistentialQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sentence.
    /// </summary>
    /// <param name="existentialQuantification">The <see cref="ExistentialQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(ExistentialQuantification existentialQuantification, TState state)
    {
        await Task.WhenAll(
            VisitAsync(existentialQuantification.Variable, state),
            VisitAsync(existentialQuantification.Sentence, state));
    }

    /// <summary>
    /// Visits an <see cref="Implication"/> instance. 
    /// The default implementation just visits both of the sub-sentences.
    /// </summary>
    /// <param name="implication">The <see cref="Implication"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Implication implication, TState state)
    {
        await Task.WhenAll(
            VisitAsync(implication.Antecedent, state),
            VisitAsync(implication.Consequent, state));
    }

    /// <summary>
    /// Visits a <see cref="Predicate"/> instance. 
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="predicate">The <see cref="Predicate"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Predicate predicate, TState state)
    {
        await Task.WhenAll(predicate.Arguments.Select(a => VisitAsync(a, state)));
    }

    /// <summary>
    /// Visits a <see cref="Negation"/> instance. 
    /// The default implementation just visits the sub-sentence.
    /// </summary>
    /// <param name="negation">The <see cref="Negation"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Negation negation, TState state)
    {
        await VisitAsync(negation.Sentence, state);
    }

    /// <summary>
    /// Visits a <see cref="UniversalQuantification"/> instance. 
    /// The default implementation just visits the variable declaration and sentence.
    /// </summary>
    /// <param name="universalQuantification">The <see cref="UniversalQuantification"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(UniversalQuantification universalQuantification, TState state)
    {
        await Task.WhenAll(
            VisitAsync(universalQuantification.Variable, state),
            VisitAsync(universalQuantification.Sentence, state));
    }

    /// <summary>
    /// Visits a <see cref="Term"/> instance.
    /// The default implementation simply invokes the Visit method appropriate to the type of the term (via <see cref="Term.Accept{TState}(ITermVisitor{TState}, TState)"/>.
    /// </summary>
    /// <param name="term">The term to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Term term, TState state)
    {
        await term.AcceptAsync(this, state);
    }

    /// <summary>
    /// Visits a <see cref="VariableReference"/> instance.
    /// The default implementation just visits the variable declaration.
    /// </summary>
    /// <param name="variable">The variable reference to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(VariableReference variable, TState state)
    {
        await VisitAsync(variable.Declaration, state);
    }

    /// <summary>
    /// Visits a <see cref="Function"/> instance.
    /// The default implementation just visits each of the arguments.
    /// </summary>
    /// <param name="function">The function to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual async Task VisitAsync(Function function, TState state)
    {
        await Task.WhenAll(function.Arguments.Select(a => VisitAsync(a, state)));
    }

    /// <summary>
    /// Visits a <see cref="VariableDeclaration"/> instance.
    /// The default implementation doesn't do anything.
    /// </summary>
    /// <param name="variableDeclaration">The <see cref="VariableDeclaration"/> instance to visit.</param>
    /// <param name="state">The state of this visitation.</param>
    public virtual Task VisitAsync(VariableDeclaration variableDeclaration, TState state)
    {
        return Task.CompletedTask;
    }
}
