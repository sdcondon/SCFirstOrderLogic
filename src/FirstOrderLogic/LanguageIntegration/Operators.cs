namespace LinqToKB.FirstOrderLogic.LanguageIntegration
{
    /// <summary>
    /// Static methods for logical operators that don't exist in C#.
    /// </summary>
    public static class Operators
    {
        /// <summary>
        /// Checks if material implication, ⇒, holds.
        /// </summary>
        /// <param name="antecedent">The antecedent.</param>
        /// <param name="consequent">The consequent.</param>
        /// <returns>True if the antecedent is false or the consequent is true; otherwise false.</returns>
        public static bool If(bool antecedent, bool consequent) => !antecedent || consequent;

        /// <summary>
        /// Checks if material equivalence, ⇔, holds.
        /// </summary>
        /// <param name="equivalent1">The first equivalent.</param>
        /// <param name="equivalent2">The second equivalent.</param>
        /// <returns>True if both equivalents are true or both are false; otherwise false.</returns>
        public static bool Iff(bool equivalent1, bool equivalent2) => equivalent1 == equivalent2;
    }
}
