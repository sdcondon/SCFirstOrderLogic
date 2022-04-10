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
        /// <param name="symbol">
        /// An object representing the symbol of the constant.
        /// <para/>
        /// Symbol equality should indicate that it is the same constant in the domain.
        /// <see cref="object.ToString"/> of the Symbol should be appropriate for rendering in FoL syntax.
        /// </param>
        public Constant(object symbol)
        {
            Symbol = symbol;
        }

        /// <inheritdoc />
        public override sealed bool IsGroundTerm => true;

        /// <summary>
        /// Gets an object representing the symbol of the constant.
        /// <para/>
        /// Symbol equality should indicate that it is the same constant in the domain.
        /// <see cref="object.ToString"/> of the Symbol should be appropriate for rendering in FoL syntax.
        /// </summary>
        /// <remarks>
        /// TODO-ROBUSTNESS: Yeah, I don't like using object here. We could create an ISymbol interface (with a e.g. Render method),
        /// together with a StringSymbol implementation that is implicitly convertible from strings for ease of use.. Probably at the 
        /// same time as figuring out a good long-term approach to rendering and formatting.
        /// </remarks>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj) => obj is Constant otherConstant && otherConstant.Symbol.Equals(Symbol);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine(Symbol);
    }
}
