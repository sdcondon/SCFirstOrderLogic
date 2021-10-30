using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. Specifically,
    /// represents a so-called "Skolem" function, created on-the-fly by the normalisation process.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class SkolemFunction<TDomain, TElement> : Function<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="arguments">The arguments of this function.</param>
        public SkolemFunction(string label, IList<Term<TDomain, TElement>> arguments)
            : base(arguments)
        {
            Label = label;
        }

        /// <summary>
        /// Gets the label for this function. NB: string labels probably not the right call here (uniqueness is what matters - GUID?) - but will do for now.
        /// </summary>
        public string Label { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is SkolemFunction<TDomain, TElement> otherFunction)
                || !otherFunction.Label.Equals(Label)
                || otherFunction.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            // TODO: factor to base class..
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

            hashCode.Add(Label.GetHashCode());
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
