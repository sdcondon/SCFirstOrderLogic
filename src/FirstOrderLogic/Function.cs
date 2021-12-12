using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{function symbol}({term}, ..)</code>
    /// </summary>
    public class Function : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="symbol">An object representing the symbol of the function.</param>
        /// <param name="arguments">The arguments of this function.</param>
        public Function(object symbol, IList<Term> arguments)
        {
            Symbol = symbol;
            Arguments = new ReadOnlyCollection<Term>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term> Arguments { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => Arguments.All(a => a.IsGroundTerm);

        /// <summary>
        /// Gets an object representing the symbol of the function.
        /// </summary>
        /// <remarks>
        /// Symbol equality indicates that it is the "same" function in the domain. ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// </remarks>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Function otherFunction)
                || !otherFunction.Symbol.Equals(Symbol)
                || otherFunction.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals(otherFunction.Arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(Symbol.GetHashCode());
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
