using SCFirstOrderLogic.SentenceManipulation;
using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a variable term within a sentence of first order logic. These are declared in quantifier sentences.
    /// </summary>
    public sealed class VariableReference : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReference"/> class.
        /// </summary>
        /// <param name="declaration">The declaration of the variable.</param>
        public VariableReference(VariableDeclaration declaration) => Declaration = declaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="VariableReference"/> class.
        /// </summary>
        /// <param name="symbol">The symbol of the variable. Equality of symbols should indicate that it is the same variable in the domain, and ToString of the symbol should be appropriate for rendering in FoL syntax.</param>
        public VariableReference(object symbol) => Declaration = new VariableDeclaration(symbol);

        /// <summary>
        /// Gets the declaration of the variable.
        /// </summary>
        public VariableDeclaration Declaration { get; }

        /// <summary>
        /// Gets the symbol of the variable.
        /// </summary>
        public object Symbol => Declaration.Symbol;

        /// <inheritdoc />
        public override bool IsGroundTerm => false;

        /// <inheritdoc />
        public override void Accept(ITermVisitor visitor) => visitor.Visit(this);

        /// <inheritdoc />
        public override void Accept<T>(ITermVisitor<T> visitor, T state) => visitor.Visit(this, state);

        /// <inheritdoc />
        public override void Accept<T>(ITermVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

        /// <inheritdoc />
        public override TOut Accept<TOut>(ITermTransformation<TOut> transformation) => transformation.ApplyTo(this);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is VariableReference otherVariable && Declaration.Equals(otherVariable.Declaration); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Declaration);
        }
    }
}
