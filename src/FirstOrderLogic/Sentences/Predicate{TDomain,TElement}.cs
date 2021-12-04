using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>Predicate({term}, ..)</code>
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public abstract class Predicate<TDomain, TElement> : Sentence<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Predicate(IList<Term<TDomain, TElement>> arguments)
        {
            Arguments = new ReadOnlyCollection<Term<TDomain, TElement>>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term<TDomain, TElement>> Arguments { get; }

        /// <summary>
        /// Determines if another predicate has the same symbol as this one.
        /// </summary>
        /// <param name="other">The other predicate.</param>
        /// <returns>True if and only if the other predicate has the same symmbol as this one.</returns>
        /// <remarks>
        /// Utimately might be better to do this as an object-valued Symbol property?
        /// Because that could be used in hash code calculations, which would let us define equality and
        /// hash code in this base class.. Possible (probable..) follow-up commit to do that..
        /// </remarks>
        public abstract bool SymbolEquals(Predicate<TDomain, TElement> other);
    }
}
