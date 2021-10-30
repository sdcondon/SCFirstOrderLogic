using LinqToKB.FirstOrderLogic.InternalUtilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Function<TDomain, TElement> : Term<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="arguments">The arguments of this function.</param>
        public Function(IList<Term<TDomain, TElement>> arguments)
        {
            Arguments = new ReadOnlyCollection<Term<TDomain, TElement>>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term<TDomain, TElement>> Arguments { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => Arguments.All(a => a.IsGroundTerm);
    }
}
