using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.OperableSentenceFactory.KinshipDomain;

namespace SCFirstOrderLogic.SentenceManipulation.Unification
{
    public static class CNFClauseExtensionTests
    {
        private record UnifiesWithAnyOfTestCase(CNFClause Clause, IEnumerable<CNFClause> Clauses, bool ExpectedResult);

        public static Test UnifiesWithAnyOfBehaviour => TestThat
            .GivenEachOf(() => new UnifiesWithAnyOfTestCase[]
            {
                ////new (
                ////    Clause: new CNFClause(IsParent(P, C)),
                ////    Clauses: new CNFClause[] { },
                ////    ExpectedResult: false),

                ////new (
                ////    Clause: new CNFClause(IsParent(P, C)),
                ////    Clauses: new CNFClause[] { new CNFClause(IsParent(X, Y)) },
                ////    ExpectedResult: true),

                new (
                    Clause: new CNFClause(IsParent(X, Y) | IsChild(X, Y)),
                    Clauses: new CNFClause[] { new CNFClause(IsParent(A, B) | IsChild(A, B)) },
                    ExpectedResult: true),
            })
            .When(tc => tc.Clause.UnifiesWithAnyOf(tc.Clauses))
            .ThenReturns((tc, rv) => rv.Should().Be(tc.ExpectedResult));
    }
}
