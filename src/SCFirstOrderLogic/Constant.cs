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
        /// <param name="identifier">
        /// <para>
        /// An object that serves as the unique identifier of the constant.
        /// </para>
        /// <para>
        /// Identifier equality should indicate that it is the same constant in the domain.
        /// <see cref="object.ToString"/> of the identifier should be appropriate for rendering in FoL syntax.
        /// </para>
        /// </param>
        public Constant(object identifier)
        {
            Identifier = identifier;
        }

        /// <inheritdoc />
        public override sealed bool IsGroundTerm => true;

        /// <summary>
        /// <para>
        /// Gets the object that serves as the unique identifier of the constant.
        /// </para>
        /// <para>
        /// Identifier equality should indicate that it is the same constant in the domain.
        /// <see cref="object.ToString"/> of the identifier should be appropriate for rendering in FoL syntax.
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
        public override bool Equals(object? obj) => obj is Constant otherConstant && otherConstant.Identifier.Equals(Identifier);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Identifier);
    }
}
