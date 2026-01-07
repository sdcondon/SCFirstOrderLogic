// Copyright (c) 2021-2026 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.FormulaFormatting;

/// <summary>
/// Composite <see cref="ILabeller"/> implementation that can use a different labeller
/// for different identifier types. Falls back to <see cref="object.ToString"/> for types
/// for which no labeller is registered.
/// </summary>
public class ByTypeLabeller : ILabeller
{
    private readonly IReadOnlyDictionary<Type, ILabeller> labellersByIdentifierType;

    /// <summary>
    /// Initialises a new instance of the <see cref="ByTypeLabeller"/> class.
    /// </summary>
    /// <param name="labellersByIdentifierType">A mapping from identifier type to the labeller to use for identifiers of that type.</param>
    public ByTypeLabeller(IReadOnlyDictionary<Type, ILabeller> labellersByIdentifierType)
    {
        this.labellersByIdentifierType = labellersByIdentifierType;
    }

    /// <inheritdoc/>
    public ILabellingScope MakeLabellingScope(IDictionary<object, string> labelsByIdentifier)
    {
        return new LabellingScope(labellersByIdentifierType.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.MakeLabellingScope(labelsByIdentifier)));
    }

    private class LabellingScope : ILabellingScope
    {
        private readonly IReadOnlyDictionary<Type, ILabellingScope> labellingScopesByIdentifierType;

        public LabellingScope(IReadOnlyDictionary<Type, ILabellingScope> labellingScopesByIdentifierType)
        {
            this.labellingScopesByIdentifierType = labellingScopesByIdentifierType;
        }

        /// <inheritdoc />
        public string GetLabel(object identifier)
        {
            if (labellingScopesByIdentifierType.TryGetValue(identifier.GetType(), out var labellingScope))
            {
                return labellingScope.GetLabel(identifier);
            }

            return identifier.ToString() ?? throw new ArgumentException($"Cannot create label for identifier because no labeller is registered for its type ({identifier.GetType().Name}), and its ToString returned null", nameof(identifier));
        }
    }
}
