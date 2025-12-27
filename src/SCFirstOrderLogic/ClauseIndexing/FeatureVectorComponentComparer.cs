// Copyright © 2023-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

internal class FeatureVectorComponentComparer : IComparer<FeatureVectorComponent>
{
    private readonly IComparer featureComparer;

    public FeatureVectorComponentComparer(IComparer featureComparer)
    {
        this.featureComparer = featureComparer;
    }

    public int Compare(FeatureVectorComponent x, FeatureVectorComponent y)
    {
        var featureComparison = featureComparer.Compare(x.Feature, y.Feature);

        if (featureComparison != 0)
        {
            return featureComparison;
        }

        return x.Magnitude.CompareTo(y.Magnitude);
    }
}
