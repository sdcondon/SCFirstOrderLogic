using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{predicate symbol}({term}, ..)</code>
    /// </summary>
    public abstract class Predicate : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Predicate(IList<Term> arguments)
        {
            Arguments = new ReadOnlyCollection<Term>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term> Arguments { get; }

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
        public abstract bool SymbolEquals(Predicate other);
    }
}
