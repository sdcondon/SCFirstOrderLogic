using System.Collections.Generic;

namespace SCFirstOrderLogic.SentenceFormatting
{
    /// <summary>
    /// <para>
    /// An implementation of <see cref="ILabeller{T}"/> of <see cref="StandardisedVariableSymbol"/> that outputs the
    /// (ToString of the) original symbol of the standardised variable, along with a subscript numeric suffix, as required
    /// for uniqueness.
    /// </para>
    /// <para>
    /// NB: Doesn't have any specific handling if the symbol's ToString already ends in a numeric subscript. So this could
    /// result in confusing suffixes (though there is no risk to uniqueness).
    /// </para>
    /// </summary>
    public class SubscriptSuffixLabeller : ILabeller<StandardisedVariableSymbol>
    {
        /// <inheritdoc />
        public ILabellingScope<StandardisedVariableSymbol> MakeLabellingScope() => new LabellingScope();

        private class LabellingScope : ILabellingScope<StandardisedVariableSymbol>
        {
            private static readonly char[] SuffixDigits = new[] { '₀', '₁', '₂', '₃', '₄', '₅', '₆', '₇', '₈', '₉' };
            private readonly Dictionary<StandardisedVariableSymbol, string> labelsBySymbol = new();

            /// <inheritdoc />
            public string GetLabel(StandardisedVariableSymbol symbol)
            {
                if (labelsBySymbol.TryGetValue(symbol, out var label))
                {
                    return label;
                }
                else
                {
                    int suffix = 1;
                    label = symbol.OriginalSymbol.ToString() + ToSubscriptString(suffix);
                    while (labelsBySymbol.ContainsValue(label))
                    {
                        suffix++;
                        label = symbol.OriginalSymbol.ToString() + ToSubscriptString(suffix);
                    }

                    return labelsBySymbol[symbol] = label;
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
