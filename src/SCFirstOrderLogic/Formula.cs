// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.FormulaFormatting;
using SCFirstOrderLogic.FormulaManipulation;
using System.Threading.Tasks;

namespace SCFirstOrderLogic;

/// <summary>
/// <para>
/// Representation of a formula of first order logic.
/// </para>
/// <para>
/// NB: There are no "intuitive" operator definitions (&amp; for conjunctions, | for disjunctions etc) here,
/// to keep the core formula classes as lean and mean as possible. C# / FoL syntax mapping
/// is better achieved with LINQ - see the types in the LanguageIntegration namespace. Or, if you're
/// really desperate for formula operators, also see the <see cref="FormulaCreation.OperableFormulaFactory"/> class,
/// which is something of a compromise.
/// </para>
/// <para>
/// Also, note that there's no validation method (for e.g. catching of variable declaration the
/// identifier of which is equal to one already in scope, or of references to non-declared variables).
/// This is better left to the normalisation process. Again, this is to keep the core classes as dumb
/// (and thus flexible) as possible.
/// </para>
/// </summary>
public abstract class Formula
{
    /// <summary>
    /// Accepts a <see cref="IFormulaVisitor"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    public abstract void Accept(IFormulaVisitor visitor);

    /// <summary>
    /// Accepts a <see cref="IFormulaVisitor{TState}"/> instance.
    /// </summary>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">The state that the visitor is to work with.</param>
    public abstract void Accept<TState>(IFormulaVisitor<TState> visitor, TState state);

    /// <summary>
    /// Accepts a <see cref="IFormulaVisitorR{TState}"/> instance.
    /// </summary>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">A reference to the state that the visitor is to work with.</param>
    public abstract void Accept<TState>(IFormulaVisitorR<TState> visitor, ref TState state);

    /// <summary>
    /// Accepts a <see cref="IFormulaTransformation{TOut}"/> instance.
    /// </summary>
    /// <typeparam name="TOut">the type that the transformation outputs.</typeparam>
    /// <param name="transformation">The transformation to apply.</param>
    /// <returns>The result of the transformation.</returns>
    public abstract TOut Accept<TOut>(IFormulaTransformation<TOut> transformation);

    /// <summary>
    /// Accepts a <see cref="IFormulaTransformation{TOut,TState}"/> instance.
    /// </summary>
    /// <typeparam name="TOut">the type that the transformation outputs.</typeparam>
    /// <typeparam name="TState">The type of state that the transformation works with.</typeparam>
    /// <param name="transformation">The transformation to apply.</param>
    /// <param name="state">The state that the transformation is to work with.</param>
    public abstract TOut Accept<TOut, TState>(IFormulaTransformation<TOut, TState> transformation, TState state);

    /// <summary>
    /// Accepts a <see cref="IAsyncFormulaVisitor"/> instance.
    /// </summary>
    /// <param name="visitor">The visitor to be visited by.</param>
    public abstract Task AcceptAsync(IAsyncFormulaVisitor visitor);

    /// <summary>
    /// Accepts a <see cref="IAsyncFormulaVisitor{TState}"/> instance.
    /// </summary>
    /// <typeparam name="TState">The type of state that the visitor works with.</typeparam>
    /// <param name="visitor">The visitor to be visited by.</param>
    /// <param name="state">The state that the visitor is to work with.</param>
    public abstract Task AcceptAsync<TState>(IAsyncFormulaVisitor<TState> visitor, TState state);

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="FormulaFormatter"/> object and uses it to format the formula.
    /// If the formula has been normalised (i.e. contains standardised variables or Skolem functions), its worth noting that this
    /// will not guarantee unique labelling of any normalisation terms (standardised variables and Skolem functions) across a set
    /// of formulas, or provide any choice as to the sets of labels used for them. If you want either of these behaviours,
    /// instantiate your own <see cref="FormulaFormatter"/> instance.
    /// </para>
    /// <para>
    /// Aside: I have wondered if it would perhaps better to just enforce explicit FormulaFormatter use by not defining an override here.
    /// That would however be a PITA if you just want to print out your nice, simple formula. It may even be non-normalised - in which
    /// case you definitely won't want to be messing around with sets of labels. So its important that this stays - to avoid a barrier to
    /// entry for the library.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => new FormulaFormatter().Format(this);
}
