// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic.ClauseIndexing;

/// <summary>
/// An implementation of <see cref="IFeatureVectorIndexNode{TValue}"/> that stores its child nodes using a <see cref="SortedDictionary{TKey, TValue}"/>.
/// </summary>
/// <typeparam name="TValue">The type of value attached for each term.</typeparam>
// TODO: actually assume sorted list will do?
public class FeatureVectorIndexDictionaryNode<TValue> : IFeatureVectorIndexNode<TValue>
{
}
