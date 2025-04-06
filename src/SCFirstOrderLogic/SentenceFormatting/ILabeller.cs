// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceFormatting;

/// <summary>
/// Interface for types capable of creating labels for identifiers that are unique (within a specified scope).
/// </summary>
public interface ILabeller
{
    /// <summary>
    /// Create a new <see cref="ILabellingScope"/>.
    /// </summary>
    /// <returns>A new labelling scope.</returns>
    ILabellingScope MakeLabellingScope(IDictionary<object, string> labelsByIdentifier);

    /// <summary>
    /// Create a new <see cref="ILabellingScope"/>.
    /// </summary>
    /// <returns>A new labelling scope.</returns>
    ILabellingScope MakeLabellingScope()
    {
        return MakeLabellingScope(new Dictionary<object, string>());
    }
}
