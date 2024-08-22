// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using SCFirstOrderLogic.SentenceFormatting;
using SCFirstOrderLogic.SentenceManipulation;
using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using System;
using System.Collections.Generic;

namespace SCFirstOrderLogic;

/// <summary>
/// Representation of a <see cref="Sentence"/> in conjunctive normal form (CNF).
/// </summary>
public class CNFSentence : IEquatable<CNFSentence>
{
    private static readonly IEqualityComparer<HashSet<CNFClause>> ClausesEqualityComparer = HashSet<CNFClause>.CreateSetComparer();
    private readonly HashSet<CNFClause> clauses;

    /// <summary>
    /// Initialises a new instance of the <see cref="CNFSentence"/> class from an enumerable of clauses.
    /// </summary>
    /// <param name="clauses">The set of clauses to be included in the sentence.</param>
    public CNFSentence(IEnumerable<CNFClause> clauses)
        : this(new HashSet<CNFClause>(clauses))
    {
    }

    // NB: We *could* actually use an immutable type to stop unscrupulous users from making it mutable by casting,
    // but this is a very low-level class, so I've opted to be lean and mean.
    internal CNFSentence(HashSet<CNFClause> clauses) => this.clauses = clauses;

    /// <summary>
    /// Gets the collection of clauses that comprise this CNF sentence.
    /// </summary>
    public IReadOnlySet<CNFClause> Clauses => clauses;

    /// <summary>
    /// <para>
    /// Returns a string that represents the current object.
    /// </para>
    /// <para>
    /// NB: The implementation of this override creates a <see cref="SentenceFormatter"/> object and uses it to format the sentence.
    /// Note that this will not guarantee unique labelling of normalisation terms (standardised variables or Skolem functions)
    /// across multiple calls, or provide any choice as to the sets of labels used for normalisation terms. If you want either
    /// of these things, instantiate your own <see cref="SentenceFormatter"/> instance.
    /// </para>
    /// </summary>
    /// <returns>A string that represents the current object.</returns>
    public override string ToString() => new SentenceFormatter().Format(this);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is CNFSentence sentence && Equals(sentence);

    /// <inheritdoc />
    /// <remarks>
    /// Sentences that contain exactly the same collection of clauses are considered equal.
    /// </remarks>
    public bool Equals(CNFSentence? other)
    {
        return other != null && ClausesEqualityComparer.Equals(clauses, other.clauses);
    }

    /// <inheritdoc />
    /// <remarks>
    /// Sentences that contain exactly the same collection of clauses are considered equal.
    /// </remarks>
    public override int GetHashCode()
    {
        return ClausesEqualityComparer.GetHashCode(clauses);
    }
}
