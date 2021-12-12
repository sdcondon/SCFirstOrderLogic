using System;

namespace LinqToKB.FirstOrderLogic
{
    /// <summary>
    /// Representation of a variable term within a sentence of first order logic. These are declared in quantifier sentences.
    /// </summary>
    public class Variable : Term
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The declaration of the variable.</param>
        public Variable(VariableDeclaration declaration) => Declaration = declaration;

        /// <summary>
        /// Initializes a new instance of the <see cref="Variable"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public Variable(string name) => Declaration = new VariableDeclaration(name);

        /// <summary>
        /// Gets the declaration of the variable.
        /// </summary>
        public VariableDeclaration Declaration { get; }

        /// <inheritdoc />
        public override bool IsGroundTerm => false;

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is Variable otherVariable && Declaration.Equals(otherVariable.Declaration); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Declaration);
        }
    }
}
