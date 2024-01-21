// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="ILabeller{T}"/> of <see cref="StandardisedVariableIdentifier"/> that outputs the
    /// (ToString of the) original identifier of the standardised variable, along with a subscript numeric suffix, as required
    /// for uniqueness.
    /// </para>
    /// <para>
    /// NB: Doesn't have any specific handling if the identifier's ToString already ends in a numeric subscript. So this could
    /// result in confusing suffixes (though there is no risk to uniqueness).
    /// </para>
    /// </summary>
    public class SubscriptSuffixLabeller : ILabeller<StandardisedVariableIdentifier>
    {
        /// <inheritdoc />
        public ILabellingScope<StandardisedVariableIdentifier> MakeLabellingScope() => new LabellingScope();

        private class LabellingScope : ILabellingScope<StandardisedVariableIdentifier>
        {
            private static readonly char[] SuffixDigits = new[] { '₀', '₁', '₂', '₃', '₄', '₅', '₆', '₇', '₈', '₉' };
            private readonly Dictionary<StandardisedVariableIdentifier, string> labelsByIdentifier = new();

            /// <inheritdoc />
            public string GetLabel(StandardisedVariableIdentifier identifier)
            {
                if (labelsByIdentifier.TryGetValue(identifier, out var label))
                {
                    return label;
                }
                else
                {
                    int suffix = 1;
                    label = identifier.OriginalIdentifier.ToString() + ToSubscriptString(suffix);
                    while (labelsByIdentifier.ContainsValue(label))
                    {
                        suffix++;
                        label = identifier.OriginalIdentifier.ToString() + ToSubscriptString(suffix);
                    }

                    return labelsByIdentifier[identifier] = label;
                }
            }

            private static string ToSubscriptString(int value)
            {
                List<char> subscriptChars = new();

                while (value > 0)
                {
                    subscriptChars.Add(SuffixDigits[value % 10]);
                    value /= 10;
                }

                subscriptChars.Reverse();
                return new string(subscriptChars.ToArray());
            }
        }
    }
}
