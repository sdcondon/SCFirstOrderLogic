// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceFormatting;

/// <summary>
/// An implementation of <see cref="ILabeller"/> that just uses a given set of labels - failing if this is exhausted.
/// </summary>
public class LabelSetLabeller : ILabeller
{
    private readonly IEnumerable<string> labelSet;

    /// <summary>
    /// Initialises a new instances of the <see cref="LabelSetLabeller"/> class.
    /// </summary>
    /// <param name="labelSet">The set of labels to use.</param>
    public LabelSetLabeller(IEnumerable<string> labelSet) => this.labelSet = labelSet;

    /// <inheritdoc />
    public ILabellingScope MakeLabellingScope(IDictionary<object, string> labelsByIdentifier) => new LabellingScope(labelSet.GetEnumerator(), labelsByIdentifier);

    private class LabellingScope : ILabellingScope
    {
        private readonly IEnumerator<string> labelEnumerator;
        private readonly IDictionary<object, string> labelsByIdentifier;

        public LabellingScope(IEnumerator<string> labelEnumerator, IDictionary<object, string> labelsByIdentifier)
        {
            this.labelEnumerator = labelEnumerator;
            this.labelsByIdentifier = labelsByIdentifier;
        }

        /// <inheritdoc />
        public string GetLabel(object identifier)
        {
            if (labelsByIdentifier.TryGetValue(identifier, out var label))
            {
                return label;
            }
            else if (labelEnumerator.MoveNext())
            {
                return labelsByIdentifier[identifier] = labelEnumerator.Current;
            }
            else
            {
                // I suppose we *could* fall back on the ToString of the underlying variable identifier here.
                // But obviously then we lose the unique representation guarantee, and it should be relatively
                // easy to use essentially infinite label sets - so I'd rather just fail.
                // Consumers can always create their own labellers with more sophisticated behaviour.
                throw new InvalidOperationException("Label set is exhausted");
            }
        }
    }
}
