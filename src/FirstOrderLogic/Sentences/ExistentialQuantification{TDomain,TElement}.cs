using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a existential quantification sentence of first order logic. In typical FOL syntax, this is written as:
    /// <code>∃ {variable}, {sentence}</code>
    /// In LinqToKB, the equivalent expression acting on the domain (as well as any relevant variables and constants) is:
    /// <code>{domain}.Any({variable} => {expression})</code>
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class ExistentialQuantification<TDomain, TElement> : Quantification<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FOLExistentialQuantification{TModel}"/> class.
        /// </summary>
        /// <param name="variable">The variable declared by this quantification.</param>
        /// <param name="sentence">The sentence that this quantification applies to.</param>
        public ExistentialQuantification(VariableDeclaration<TDomain, TElement> variable, Sentence<TDomain, TElement> sentence)
            : base(variable, sentence)
        {
        }
    }
}
