using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. Specifically,
    /// represents a "Skolem" function, created on-the-fly by the normalisation process.
    /// </summary>
    public class SkolemFunction : Function
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkolemFunction"/> class.
        /// </summary>
        /// <param name="symbol">The symbol of the function.</param>
        /// <param name="arguments">The arguments of this function.</param>
        public SkolemFunction(string symbol, params Term[] arguments)
            : base(symbol, arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkolemFunction"/> class.
        /// </summary>
        /// <param name="symbol">The symbol of the function.</param>
        /// <param name="arguments">The arguments of this function.</param>
        public SkolemFunction(string symbol, IList<Term> arguments)
            : base(symbol, arguments)
        {
        }
    }
}
