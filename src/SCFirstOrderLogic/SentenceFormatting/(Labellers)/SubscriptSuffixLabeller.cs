// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceFormatting;

/// <summary>
/// <para>
/// An implementation of <see cref="ILabeller"/> of that outputs the ToString of the identifier,
/// along with a subscript numeric suffix, as required for uniqueness.
/// </para>
/// <para>
/// NB: Doesn't have any specific handling if the identifier's ToString already ends in a numeric subscript. So this could
/// result in confusing suffixes (though there is no risk to uniqueness).
/// </para>
/// </summary>
public class SubscriptSuffixLabeller : ILabeller
{
    /// <inheritdoc />
    public ILabellingScope MakeLabellingScope(IDictionary<object, string> labelsByIdentifier) => new LabellingScope(labelsByIdentifier);

    private class LabellingScope : ILabellingScope
    {
        private static readonly char[] SuffixDigits = new[] { '₀', '₁', '₂', '₃', '₄', '₅', '₆', '₇', '₈', '₉' };
        private readonly IDictionary<object, string> labelsByIdentifier;

        public LabellingScope(IDictionary<object, string> labelsByIdentifier)
        {
            this.labelsByIdentifier = labelsByIdentifier;
        }

        /// <inheritdoc />
        public string GetLabel(object identifier)
        {
            if (labelsByIdentifier.TryGetValue(identifier, out var label))
            {
                return label;
            }
            else
            {
                int suffix = 1;
                label = identifier.ToString() + ToSubscriptString(suffix);
                while (labelsByIdentifier.Values.Contains(label))
                {
                    suffix++;
                    label = identifier.ToString() + ToSubscriptString(suffix);
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
