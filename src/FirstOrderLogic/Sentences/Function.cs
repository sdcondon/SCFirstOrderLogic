using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a function term within a sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>{function symbol}({term}, ..)</code>
    /// </summary>
    public abstract class Function : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="arguments">The arguments of this function.</param>
        public Function(IList<Term> arguments)
        {
            Arguments = new ReadOnlyCollection<Term>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term> Arguments { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => Arguments.All(a => a.IsGroundTerm);
    }
}
