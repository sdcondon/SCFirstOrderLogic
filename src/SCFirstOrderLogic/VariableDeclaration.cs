using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a variable declaration within a sentence of first order logic. These occur in quantifier sentences.
    /// </summary>
    /// <remarks>
    /// The existence of this type (as distinct to <see cref="VariableReference"/>) is for robust transformations. When transforming a
    /// <see cref="VariableReference"/> (which is a <see cref="Term"/> - it can occur anywhere in a sentence where a <see cref="Term"/>
    /// is valid), it will always be valid to transform it into a different kind of <see cref="Term"/>.
    /// <para/>
    /// <see cref="VariableDeclaration"/>s however (which are NOT <see cref="Term"/>s), occur only in <see cref="VariableReference"/> and
    /// <see cref="Quantification"/> instances, both of which require the <see cref="VariableDeclaration"/> type exactly.
    /// </remarks>
    public class VariableDeclaration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDeclaration"/> class.
        /// </summary>
        /// <param name="symbol">The aymbol of the variable. Equality of symbols should indicate that it is the same variable in the domain, and ToString of the symbol should be appropriate for rendering in FoL syntax.</param>
        public VariableDeclaration(object symbol) => Symbol = symbol;

        /// <summary>
        /// Gets the symbol of the variable. Equality of symbols should indicate that it is the same variable in the domain, and ToString of the symbol should be appropriate for rendering in FoL syntax.
        /// </summary>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VariableDeclaration otherVariableDeclaration && Symbol.Equals(otherVariableDeclaration.Symbol); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Symbol);
        }

        /// <summary>
        /// Defines the implicit conversion operator from a variable reference to its declaration (which I think is always a safe thing to do,
        /// and makes invocation of e.g. factory methods for ForAll and ThereExist easier).
        /// </summary>
        /// <param name="declaration">The declaration to convert.</param>
        public static implicit operator VariableDeclaration(VariableReference reference) => reference.Declaration;
    }
}
