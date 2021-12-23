using System;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a variable declaration term within a sentence of first order logic. These occur in quantifier sentences.
    /// </summary>
    public class VariableDeclaration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VariableDeclaration"/> class.
        /// </summary>
        /// <param name="name">The name of the variable.</param>
        public VariableDeclaration(string name) => Name = name;

        /// <summary>
        /// Gets the name of the variable.
        /// </summary>
        public string Name { get; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is VariableDeclaration otherVariableDeclaration && Name.Equals(otherVariableDeclaration.Name); 
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
