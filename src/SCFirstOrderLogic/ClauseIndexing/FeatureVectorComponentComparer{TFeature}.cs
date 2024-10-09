// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

internal class FeatureVectorComponentComparer<TFeature> : IComparer<FeatureVectorComponent<TFeature>>
{
    private readonly IComparer<TFeature> featureComparer;

    public FeatureVectorComponentComparer(IComparer<TFeature> featureComparer)
    {
        this.featureComparer = featureComparer;
    }

    public int Compare(FeatureVectorComponent<TFeature> x, FeatureVectorComponent<TFeature> y)
    {
        var featureComparison = featureComparer.Compare(x.Feature, y.Feature);

        if (featureComparison != 0)
        {
            return featureComparison;
        }

        return Comparer<int>.Default.Compare(x.Magnitude, y.Magnitude);
    }
}
