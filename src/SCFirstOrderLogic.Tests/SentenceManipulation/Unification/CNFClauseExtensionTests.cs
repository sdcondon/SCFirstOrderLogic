using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.Unification;

public static class CNFClauseExtensionTests
{
    public static Test UnifiesWithAnyOfBehaviour => TestThat
        .GivenEachOf(() => new UnifiesWithAnyOfTestCase[]
        {
            new (
                Clause: new CNFClause(P(X, Y)),
                Clauses: new CNFClause[] { },
                ExpectedResult: false),

            new (
                Clause: new CNFClause(P(X, Y)),
                Clauses: new CNFClause[] { new(P(A, B)) },
                ExpectedResult: true),

            new (
                Clause: new CNFClause(P(X, Y) | Q(X, Y)),
                Clauses: new CNFClause[] { new CNFClause(P(A, B) | Q(A, B)) },
                ExpectedResult: true),
        })
        .When(tc => tc.Clause.UnifiesWithAnyOf(tc.Clauses))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.ExpectedResult));

    private static OperablePredicate P(Term x, Term y) => new(nameof(P), x, y);
    private static OperablePredicate Q(Term x, Term y) => new(nameof(Q), x, y);

    private record UnifiesWithAnyOfTestCase(CNFClause Clause, IEnumerable<CNFClause> Clauses, bool ExpectedResult);
}
