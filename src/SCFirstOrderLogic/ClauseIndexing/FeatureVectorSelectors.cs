// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// A static class offering a couple of handy default feature vector selection methods to use -
/// taken from Stephan Schulz's 2013 paper.
/// </summary>
/// <seealso href="http://wwwlehre.dhbw-stuttgart.de/~sschulz/PAPERS/Schulz2013-FVI.pdf"/>
public static class FeatureVectorSelectors
{
    /// <summary>
    /// Feature vector selection logic that returns a feature vector consisting of: 
    /// positive literal count, negative literal count, occurence count of each occuring
    /// identifier among positive literals, and occurence count of each occuring identifier
    /// among negative literals.
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<KeyValuePair<object, int>> OccurenceCounts(CNFClause clause)
    {
        return null;
    }

    /// <summary>
    /// Feature vector selection logic that returns a feature vector consisting of the max depth
    /// of each occuring identifier among positive literals, and the max depth of each occuring 
    /// identifier among negative literals.
    /// </summary>
    /// <param name="clause">The clause to retrieve a feature vector for.</param>
    /// <returns>A feature vector.</returns>
    public static IEnumerable<KeyValuePair<object, int>> MaxDepths(CNFClause clause)
    {
        return null;
    }
}
