// Copyright (c) 2021-2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// An implementation of <see cref="ILabeller{T}"/> that just uses a given set of labels - failing if this is exhausted.
    /// </summary>
    public class LabelSetLabeller<T> : ILabeller<T>
        where T : class
    {
        private readonly IEnumerable<string> labelSet;

        /// <summary>
        /// Initialises a new instances of the <see cref="LabelSetLabeller{T}"/> class.
        /// </summary>
        /// <param name="labelSet">The set of labels to use.</param>
        public LabelSetLabeller(IEnumerable<string> labelSet) => this.labelSet = labelSet;

        /// <inheritdoc />
        public ILabellingScope<T> MakeLabellingScope() => new LabelSetLabellingScope(labelSet.GetEnumerator());

        private class LabelSetLabellingScope : ILabellingScope<T>
        {
            private readonly IEnumerator<string> labelEnumerator;
            private readonly Dictionary<T, string> labelsBySymbol = new();

            public LabelSetLabellingScope(IEnumerator<string> labelEnumerator) => this.labelEnumerator = labelEnumerator;

            /// <inheritdoc />
            public string GetLabel(T symbol)
            {
                if (labelsBySymbol.TryGetValue(symbol, out var label))
                {
                    return label;
                }
                else if (labelEnumerator.MoveNext())
                {
                    return labelsBySymbol[symbol] = labelEnumerator.Current;
                }
                else
                {
                    // I suppose we *could* fall back on the ToString of the underlying variable symbol here.
                    // But obviously then we lose the unique representation guarantee, and it should be relatively
                    // easy to use essentially infinite label sets - so I'd rather just fail.
                    // Consumers can always create their own labellers with more sophisticated behaviour.
                    throw new InvalidOperationException("Label set is exhausted");
                }
            }
        }
    }
}
