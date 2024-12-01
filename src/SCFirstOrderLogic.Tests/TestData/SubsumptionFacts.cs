using SCFirstOrderLogic.SentenceManipulation.VariableManipulation;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.TestData;

public static class SubsumptionFacts
{
    /// <summary>
    /// Gets a set of subsumption facts.
    /// </summary>
    public static readonly IReadOnlyList<SubsumptionFact> All =
    [
        new (X: P(),             Y: P(),         IsXSubsumedByY: true,  IsYSubsumedByX: true),
        new (X: P(U),            Y: P(U),        IsXSubsumedByY: true,  IsYSubsumedByX: true),
        new (X: P(U),            Y: P(C),        IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(U),            Y: P(C) | P(D), IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(U),            Y: P(C) | Q(U), IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(C),            Y: P(C),        IsXSubsumedByY: true,  IsYSubsumedByX: true),
        new (X: P(C),            Y: P(C) | P(D), IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(U) | Q(V),     Y: P(C) | Q(D), IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(U) | Q(V),     Y: P(C) | Q(U), IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(U) | Q(U),     Y: P(C) | Q(C), IsXSubsumedByY: false, IsYSubsumedByX: true),
        new (X: P(U) | Q(U),     Y: P(C) | Q(D), IsXSubsumedByY: false, IsYSubsumedByX: false),
        new (X: P(U),            Y: Q(C),        IsXSubsumedByY: false, IsYSubsumedByX: false),
        new (X: P(U) | Q(V),     Y: P(C),        IsXSubsumedByY: false, IsYSubsumedByX: false),
        new (X: CNFClause.Empty, Y: P(),         IsXSubsumedByY: false, IsYSubsumedByX: false),
    ];

    /// <summary>
    /// Gets all of the (distinct, accounting for variable renames) non-empty clauses that appear in <see cref="All"/>.
    /// </summary>
    public static IEnumerable<CNFClause> AllNonEmptyClauses => All
        .SelectMany(f => new[] { f.X, f.Y })
        .Except([CNFClause.Empty])
        .Distinct(new VariableIdIgnorantEqualityComparer());

    /// <summary>
    /// Given a clause that appears in at least one of the facts in <see cref="All"/>, gets the  (distinct, accounting
    /// for variable renames) clauses that are subsumed by it, according to the facts in <see cref="All"/>.
    /// </summary>
    /// <param name="subsumingClause">The subsuming clause.</param>
    /// <returns>The clauses that are subsumed by the given clause, according to the facts in <see cref="All"/>.</returns>
    public static CNFClause[] GetSubsumedClauses(CNFClause subsumingClause)
    {
        var xClauses = All
            .Where(f => f.X.Equals(subsumingClause) && f.IsYSubsumedByX)
            .Select(f => f.Y);

        var yClauses = All
            .Where(f => f.Y.Equals(subsumingClause) && f.IsXSubsumedByY)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    /// <summary>
    /// Given a clause that appears in at least one of the facts in <see cref="All"/>, gets the  (distinct, accounting
    /// for variable renames) clauses that are not subsumed by it, according to the facts in <see cref="All"/>.
    /// </summary>
    /// <param name="subsumingClause">The non-subsuming clause.</param>
    /// <returns>The clauses that are not subsumed by the given clause, according to the facts in <see cref="All"/>.</returns>
    public static CNFClause[] GetNonSubsumedClauses(CNFClause subsumingClause)
    {
        var xClauses = All
            .Where(f => f.X.Equals(subsumingClause) && !f.IsYSubsumedByX)
            .Select(f => f.Y);

        var yClauses = All
            .Where(f => f.Y.Equals(subsumingClause) && !f.IsXSubsumedByY)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    /// <summary>
    /// Given a clause that appears in at least one of the facts in <see cref="All"/>, gets the  (distinct, accounting
    /// for variable renames) clauses that subsume it, according to the facts in <see cref="All"/>.
    /// </summary>
    /// <param name="subsumingClause">The subsumed clause.</param>
    /// <returns>The clauses that subsume the given clause, according to the facts in <see cref="All"/>.</returns>
    public static CNFClause[] GetSubsumingClauses(CNFClause subsumedClause)
    {
        var xClauses = All
            .Where(f => f.X.Equals(subsumedClause) && f.IsXSubsumedByY)
            .Select(f => f.Y);

        var yClauses = All
            .Where(f => f.Y.Equals(subsumedClause) && f.IsYSubsumedByX)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }

    /// <summary>
    /// Given a clause that appears in at least one of the facts in <see cref="All"/>, gets the  (distinct, accounting
    /// for variable renames) clauses that do not subsume it, according to the facts in <see cref="All"/>.
    /// </summary>
    /// <param name="subsumingClause">The non subsumed clause.</param>
    /// <returns>The clauses that do not subsume the given clause, according to the facts in <see cref="All"/>.</returns>
    public static CNFClause[] GetNonSubsumingClauses(CNFClause subsumedClause)
    {
        var xClauses = All
            .Where(f => f.X.Equals(subsumedClause) && !f.IsXSubsumedByY)
            .Select(f => f.Y);

        var yClauses = All
            .Where(f => f.Y.Equals(subsumedClause) && !f.IsYSubsumedByX)
            .Select(f => f.X);

        return xClauses
            .Concat(yClauses)
            .Except([CNFClause.Empty])
            .Distinct(new VariableIdIgnorantEqualityComparer())
            .ToArray();
    }
}
