using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a variable declaration within a sentence of first order logic. These occur in quantifier sentences.
    /// <para/>
    /// The existence of this type (as distinct from <see cref="VariableReference"/>) is for robust sentence transformations. When transforming a
    /// <see cref="VariableReference"/> (which is a <see cref="Term"/> - it can occur anywhere in a sentence where a <see cref="Term"/>
    /// is valid), it will always be valid to transform it into a different kind of <see cref="Term"/>. <see cref="VariableDeclaration"/>s
    /// however (which are NOT <see cref="Term"/>s), occur only in <see cref="Quantification"/> (and <see cref="VariableReference"/>) instances,
    /// both of which require the <see cref="VariableDeclaration"/> type exactly.
    /// <para/>
    /// <see cref="VariableDeclaration"/> instances are implicitly convertible to <see cref="VariableReference"/> instances referring to them,
    /// to aid in the succinct creation of sentences.
    /// </summary>
    public sealed class VariableDeclaration : IEquatable<VariableDeclaration>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDeclaration"/> class.
        /// </summary>
        /// <param name="symbol">The symbol of the variable. Equality of symbols should indicate that it is the same variable in the domain, and ToString of the symbol should be appropriate for rendering in FoL syntax.</param>
        public VariableDeclaration(object symbol) => Symbol = symbol;

        /// <summary>
        /// Gets an object representing the symbol of the variable.
        /// </summary>
        /// <remarks>
        /// Symbol equality should indicate that it is the same variable in the domain. ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// </remarks>
        /// <remarks>
        /// TODO-ROBUSTNESS: Not 100% convinced by using object here. We could create an ISymbol interface (with a e.g. Render method),
        /// together with a StringSymbol implementation that is implicitly convertible from strings for ease of use.. Probably at the 
        /// same time as figuring out a good long-term approach to rendering and formatting.
        /// </remarks>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is VariableDeclaration otherVariableDeclaration && Equals(otherVariableDeclaration);

        /// <inheritdoc />
        public bool Equals(VariableDeclaration other) => Symbol.Equals(other.Symbol);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Symbol);

        /// <summary>
        /// Defines the implicit conversion operator from a variable declaration to a reference.
        /// </summary>
        /// <param name="declaration">The declaration to convert.</param>
        public static implicit operator VariableReference(VariableDeclaration declaration) => new VariableReference(declaration);
    }
}
