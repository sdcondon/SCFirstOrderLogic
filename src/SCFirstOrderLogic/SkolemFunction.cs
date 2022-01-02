using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic
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
        /// <param name="name">The name of the function.</param>
        /// <param name="arguments">The arguments of this function.</param>
        public SkolemFunction(string name, params Term[] arguments)
            : base(new Symbol(name), arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SkolemFunction"/> class.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="arguments">The arguments of this function.</param>
        public SkolemFunction(string name, IList<Term> arguments)
            : base(new Symbol(name), arguments)
        {
        }

        // Use our own symbol class rather than just a string to eliminate the possibility
        // of Skolem functions clashing with unfortunately-named user-provided functions.
        private new class Symbol
        {
            private readonly string name;

            public Symbol(string name) => this.name = name;

            public override bool Equals(object obj) => obj is Symbol skolem && skolem.name.Equals(name);

            public override int GetHashCode() => HashCode.Combine(name);
        }
    }
}
