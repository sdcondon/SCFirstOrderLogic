// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    public sealed class Constant : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="symbol">
        /// <para>
        /// An object representing the symbol of the constant.
        /// </para>
        /// <para>
        /// Symbol equality should indicate that it is the same constant in the domain.
        /// <see cref="object.ToString"/> of the Symbol should be appropriate for rendering in FoL syntax.
        /// </para>
        /// <para>
        /// NB: Yes, we *could* declare an ISymbol interface that is IEquatable&lt;ISymbol&gt; and defines a
        /// string-returning 'Render' method. However, given that the only things we need of a symbol are
        /// equatability and the ability to convert them to a string, and both of these things are possible with the
        /// object base class, for now at least we err on the side of simplicity and say that symbols can be any object.
        /// </para>
        /// </param>
        public Constant(object symbol)
        {
            Symbol = symbol;
        }

        /// <inheritdoc />
        public override sealed bool IsGroundTerm => true;

        /// <summary>
        /// <para>
        /// Gets an object representing the symbol of the constant.
        /// </para>
        /// <para>
        /// Symbol equality should indicate that it is the same constant in the domain.
        /// <see cref="object.ToString"/> of the Symbol should be appropriate for rendering in FoL syntax.
        /// </para>
        /// <para>
        /// NB: Yes, we *could* declare an ISymbol interface that is IEquatable&lt;ISymbol&gt; and defines a
        /// string-returning 'Render' method. However, given that the only things we need of a symbol are
        /// equatability and the ability to convert them to a string, and both of these things are possible with the
        /// object base class, we err on the side of simplicity and say that symbols can be any object. This
        /// simplicity of course has the added benefit of allowing certain likely types (strings, integers..) to be used
        /// as identifiers without needing to wrap them.
        /// </para>
        /// </summary>
        // TODO-V5-BREAKING: Now that this library has actual FoL parsing logic in it, its more noticeable that this
        // is misnamed - should be "Identifier". This in turn will make it easier to talk about advanced parsing config
        // in which users can provide a delegate to map from symbols (that is, string-valued) to identifiers.
        public object Symbol { get; }

        /// <inheritdoc />
        public override void Accept(ITermVisitor visitor) => visitor.Visit(this);

        /// <inheritdoc />
        public override void Accept<T>(ITermVisitor<T> visitor, T state) => visitor.Visit(this, state);

        /// <inheritdoc />
        public override void Accept<T>(ITermVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

        /// <inheritdoc />
        public override TOut Accept<TOut>(ITermTransformation<TOut> transformation) => transformation.ApplyTo(this);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is Constant otherConstant && otherConstant.Symbol.Equals(Symbol);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Symbol);
    }
}
