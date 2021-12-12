using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{predicate symbol}({term}, ..)</code>
    /// </summary>
    public class Predicate : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="symbol">An object representing the symbol of the predicate.</param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Predicate(object symbol, IList<Term> arguments)
        {
            Symbol = symbol;
            Arguments = new ReadOnlyCollection<Term>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term> Arguments { get; }

        /// <summary>
        /// Gets an object representing the symbol of the predicate.
        /// </summary>
        /// <remarks>
        /// Symbol equality indicates that it is the "same" predicate in the domain. ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// </remarks>
        public object Symbol { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is Predicate otherPredicate)
                || !otherPredicate.Symbol.Equals(Symbol)
                || otherPredicate.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals(otherPredicate.Arguments[i]))
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

            hashCode.Add(Symbol);
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
