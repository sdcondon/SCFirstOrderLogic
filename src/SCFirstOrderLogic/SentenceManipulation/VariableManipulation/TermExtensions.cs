using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// Useful variable manipulation and inspection methods for <see cref="Term"/> instances.
/// </summary>
public static class TermExtensions
{
    /// <summary>
    /// <para>
    /// Ordinalises a term; replacing all variable identifiers with the (zero-based) ordinal of where they
    /// first appear in a depth-first traversal of the term.
    /// </para>
    /// <para>
    /// This is useful because it makes the original identifiers irrelevant but preserves distinctness, so that 
    /// e.g. F(X, X) is transformed to a term that is identical to the transformation of F(Y, Y), but different
    /// to the transformation of F(X, Y). Making the original identifier irrelevant is useful when e.g. indexing.
    /// </para>
    /// </summary>
    /// <param name="term">The term to ordinalise.</param>
    /// <returns>The ordinalised term.</returns>
    public static Term Ordinalise(this Term term)
    {
        return term.Accept(new VariableOrdinalisation());
    }

    /// <summary>
    /// Checks whether this term is an instance of another.
    /// </summary>
    /// <param name="term">The potential instance.</param>
    /// <param name="generalisation">The generalisation.</param>
    /// <returns>A value indicating whether this term is an instance of the generalisation.</returns>
    public static bool IsInstanceOf(this Term term, Term generalisation)
    {
        return InstanceUnifier.TryCreate(generalisation, term, out _);
    }

    /// <summary>
    /// Checks whether this term is a generalisation of another.
    /// </summary>
    /// <param name="term">The potential generalisation.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>A value indicating whether this term is an generalisation of the instance.</returns>
    public static bool IsGeneralisationOf(this Term term, Term instance)
    {
        return InstanceUnifier.TryCreate(term, instance, out _);
    }

    /// <summary>
    /// <para>
    /// Sentence transformation that converts all variable identifiers to the integer value of their order in a depth-first traversal of the sentence.
    /// </para>
    /// <para>
    /// Useful in e.g. indexing, where we want to make the original identifier irrelevant when comparing sentences/clauses/terms.
    /// </para>
    /// </summary>
    private class VariableOrdinalisation : RecursiveSentenceTransformation
    {
        private readonly Dictionary<object, VariableDeclaration> variableIdMap = new();

        /// <inheritdoc/>
        public override VariableDeclaration ApplyTo(VariableDeclaration variable)
        {
            if (!variableIdMap.TryGetValue(variable.Identifier, out var declaration))
            {
                declaration = variableIdMap[variable.Identifier] = new VariableDeclaration(variableIdMap.Count);
            }

            return declaration;
        }
    }
}
