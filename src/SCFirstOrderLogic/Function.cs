// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a function term within a sentence of first order logic. In typical FOL syntax, this is written as:
/// <code>{function identifier}({term}, ..)</code>
/// Or, for functions with arity zero (that is, constants), sometimes just:
/// <code>{function identifier}</code>
/// </summary>
public sealed class Function : Term
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Function"/> class.
    /// </summary>
    /// <param name="identifier">
    /// <para>
    /// An object that serves as the unique identifier of the function.
    /// </para>
    /// <para>
    /// Identifier equality should indicate that it is the "same" function in the domain.
    /// ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </param>
    /// <param name="arguments">The arguments of this function.</param>
    public Function(object identifier, params Term[] arguments)
        : this(identifier, (IList<Term>)arguments)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Predicate"/> class.
    /// </summary>
    /// <param name="identifier">
    /// <para>
    /// An object that serves as the unique identifier of the function.
    /// </para>
    /// <para>
    /// Identifier equality should indicate that it is the "same" function in the domain.
    /// ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </param>
    /// <param name="arguments">The arguments of this function.</param>
    public Function(object identifier, IEnumerable<Term> arguments)
    {
        Identifier = identifier;
        Arguments = new ReadOnlyCollection<Term>(arguments.ToArray());
    }

    /// <summary>
    /// Gets the arguments of this function.
    /// </summary>
    public ReadOnlyCollection<Term> Arguments { get; }

    /// <inheritdoc />
    public override bool IsGroundTerm => Arguments.All(a => a.IsGroundTerm);

    /// <summary>
    /// <para>
    /// Gets an object that serves as the unique identifier of the function.
    /// </para>
    /// <para>
    /// Identifier equality should indicate that it is the "same" function in the domain.
    /// ToString of the identifier should be appropriate for rendering in FoL syntax.
    /// </para>
    /// </summary>
    public object Identifier { get; }

    /// <inheritdoc />
    public override void Accept(ITermVisitor visitor) => visitor.Visit(this);

    /// <inheritdoc />
    public override void Accept<T>(ITermVisitor<T> visitor, T state) => visitor.Visit(this, state);

    /// <inheritdoc />
    public override void Accept<T>(ITermVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

    /// <inheritdoc />
    public override TOut Accept<TOut>(ITermTransformation<TOut> transformation) => transformation.ApplyTo(this);

    /// <inheritdoc />
    public override TOut Accept<TOut, TState>(ITermTransformation<TOut, TState> transformation, TState state) => transformation.ApplyTo(this, state);

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is not Function otherFunction
            || !otherFunction.Identifier.Equals(Identifier)
            || otherFunction.Arguments.Count != Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < Arguments.Count; i++)
        {
            if (!Arguments[i].Equals(otherFunction.Arguments[i]))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = new HashCode();

        hashCode.Add(Identifier.GetHashCode());
        foreach (var argument in Arguments)
        {
            hashCode.Add(argument);
        }

        return hashCode.ToHashCode();
    }
}
