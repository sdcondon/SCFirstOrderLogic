// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaCreation.Linq;

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
    /// <para>
    /// Checks if material equivalence, ⇔, holds.
    /// </para>
    /// <para>
    /// NB: The name of this method is a common abbreviation for "if and only if".
    /// </para>
    /// </summary>
    /// <param name="equivalent1">The first equivalent.</param>
    /// <param name="equivalent2">The second equivalent.</param>
    /// <returns>True if both equivalents are true or both are false; otherwise false.</returns>
    public static bool Iff(bool equivalent1, bool equivalent2) => equivalent1 == equivalent2;
}
