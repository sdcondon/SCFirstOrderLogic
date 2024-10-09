// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An individual component of a feature vector - a feature-magnitude pair.
/// </summary>
/// <typeparam name="TFeature"></typeparam>
/// <param name="Feature">The feature that is essentially the key of this component.</param>
/// <param name="Magnitude">The magnitude of this component.</param>
public record struct FeatureVectorComponent<TFeature>(TFeature Feature, int Magnitude);
