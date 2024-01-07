// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceManipulation;
using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a universal quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∀ {variable}, {sentence}</code>
    /// </summary>
    public sealed class UniversalQuantification : Quantification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UniversalQuantification"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public UniversalQuantification(VariableDeclaration variable, Sentence sentence)
            : base(variable, sentence)
        {
        }

        /// <inheritdoc />
        public override void Accept(ISentenceVisitor visitor) => visitor.Visit(this);

        /// <inheritdoc />
        public override void Accept<T>(ISentenceVisitor<T> visitor, T state) => visitor.Visit(this, state);

        /// <inheritdoc />
        public override void Accept<T>(ISentenceVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

        /// <inheritdoc />
        public override TOut Accept<TOut>(ISentenceTransformation<TOut> transformation) => transformation.ApplyTo(this);

        /// <inheritdoc />
        public override TOut Accept<TOut, TState>(ISentenceTransformation<TOut, TState> transformation, TState state) => transformation.ApplyTo(this, state);

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is UniversalQuantification universalQuantification && base.Equals(universalQuantification);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Variable, Sentence);
    }
}
