using System;
using System.Collections.Generic;

namespace LinqToKB.FirstOrderLogic.Sentences
{
    /// <summary>
    /// Representation of a variable term within a sentence of first order logic. These are declared in quantifier sentences.
    /// </summary>
    /// <typeparam name="TDomain">The type of the domain.</typeparam>
    /// <typeparam name="TElement">The type that all elements of the domain are assignable to.</typeparam>
    public class Variable<TDomain, TElement> : Term<TDomain, TElement>
        where TDomain : IEnumerable<TElement>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Variable{TDomain, TElement}"/> class.
        /// </summary>
        /// <param name="name">The declaration of the variable.</param>
        public Variable(VariableDeclaration<TDomain, TElement> declaration) => Declaration = declaration;

        /// <summary>
        /// Gets the declaration of the variable.
        /// </summary>
        public VariableDeclaration<TDomain, TElement> Declaration { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => false;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Variable<TDomain, TElement> otherVariable && Declaration.Equals(otherVariable.Declaration); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Declaration);
        }
    }
}
