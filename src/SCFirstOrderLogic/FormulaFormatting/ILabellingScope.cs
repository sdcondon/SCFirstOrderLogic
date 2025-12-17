// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.FormulaFormatting;

/// <summary>
/// Interface for types that represent a labelling scope - a scope within which labels
/// should be uniquely assigned to identifiers.
/// </summary>
public interface ILabellingScope
{
    /// <summary>
    /// Get the label for a given identifier.
    /// </summary>
    /// <param name="identifier">The identifier to make or retrieve the label for.</param>
    /// <returns>A label for the identifier that is unique within this scope.</returns>
    string GetLabel(object identifier);
}
