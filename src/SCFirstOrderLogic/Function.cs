using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{function symbol}({term}, ..)</code>
    /// </summary>
    public sealed class Function : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberFunction"/> class.
        /// </summary>
        /// <param name="symbol">
        /// An object representing the symbol of the function.
        /// <para/>
        /// Symbol equality should indicate that it is the "same" function in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// </param>
        /// <param name="arguments">The arguments of this function.</param>
        public Function(object symbol, params Term[] arguments)
            : this(symbol, (IList<Term>)arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="symbol">
        /// An object representing the symbol of the function.
        /// <para/>
        /// Symbol equality should indicate that it is the "same" function in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// </param>
        /// <param name="arguments">The arguments of this function.</param>
        public Function(object symbol, IList<Term> arguments)
        {
            Symbol = symbol;
            Arguments = new ReadOnlyCollection<Term>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this function.
        /// </summary>
        public ReadOnlyCollection<Term> Arguments { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => Arguments.All(a => a.IsGroundTerm);

        /// <summary>
        /// Gets an object representing the symbol of the function.
        /// <para/>
        /// Symbol equality should indicate that it is the "same" function in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// </summary>
        /// <remarks>
        /// TODO-ROBUSTNESS: Yeah, I don't like using object here. We could create an ISymbol interface (with a e.g. Render method),
        /// together with a StringSymbol implementation that is implicitly convertible from strings for ease of use.. Probably at the 
        /// same time as figuring out a good long-term approach to rendering and formatting.
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
