// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.FormulaManipulation.Substitution;

/// <summary>
/// A <see cref="VariableSubstitution"/> that can be modified in-place.
/// </summary>
public class MutableVariableSubstitution : VariableSubstitution
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MutableVariableSubstitution"/> class that is empty.
    /// </summary>
    public MutableVariableSubstitution()
        : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MutableVariableSubstitution"/> class that uses a given set of bindings.
    /// </summary>
    /// <param name="bindings">The bindings to use.</param>
    public MutableVariableSubstitution(IEnumerable<KeyValuePair<VariableReference, Term>> bindings)
        : base(bindings)
    {
    }

    /// <summary>
    /// Adds a binding.
    /// </summary>
    /// <param name="variable">A reference to the variable to be substituted out.</param>
    /// <param name="term">The term to be substituted in.</param>
    public void AddBinding(VariableReference variable, Term term)
    {
        bindings.Add(variable, term);
    }
}
