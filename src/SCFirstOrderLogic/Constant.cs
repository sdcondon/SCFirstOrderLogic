using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a constant term within a sentence of first order logic.
    /// </summary>
    public class Constant : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="symbol">An object representing the symbol of the constant.</param>
        public Constant(object symbol)
        {
            Symbol = symbol;
        }

        /// <inheritdoc />
        public override sealed bool IsGroundTerm => true;

        /// <summary>
        /// Gets an object representing the symbol of the constant.
        /// </summary>
        /// <remarks>
        /// Symbol equality should indicate that it is the same constant in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// </remarks>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Constant otherConstant && otherConstant.Symbol.Equals(Symbol);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Symbol.GetHashCode());
    }
}
