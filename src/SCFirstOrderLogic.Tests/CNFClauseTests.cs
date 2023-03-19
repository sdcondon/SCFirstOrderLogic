using FluentAssertions;
using FlUnit;
using System;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic
{
    public static class CNFClauseTests
    {
        private static OperablePredicate P => new("P");
        private static OperablePredicate Q => new("Q");
        private static OperablePredicate R => new("R");

        private static OperablePredicate HashCodeCollisionX => new(new MyHashCodeCollision("X"));
        private static OperablePredicate HashCodeCollisionY => new(new MyHashCodeCollision("Y"));

        private record EqualityTestCase(CNFClause X, CNFClause Y, bool ExpectedEquality);

        public static Test EqualityBehaviour => TestThat
            .GivenEachOf(() => new EqualityTestCase[]
            {
                new(
                    X: new(Array.Empty<Literal>()),
                    Y: new(Array.Empty<Literal>()),
                    ExpectedEquality: true),

                new(
                    X: new(P),
                    Y: new(P),
                    ExpectedEquality: true),

                new(
                    X: new(P | Q),
                    Y: new(Q | P),
                    ExpectedEquality: true),

                new(
                    X: new(HashCodeCollisionX | HashCodeCollisionY),
                    Y: new(HashCodeCollisionY | HashCodeCollisionX),
                    ExpectedEquality: true),

                new(
                    X: new(P | Q),
                    Y: new(P | Q | R),
                    ExpectedEquality: false),

                new(
                    X: new(P | Q | R),
                    Y: new(P | Q),
                    ExpectedEquality: false),
            })
            .When(tc => (Equality: tc.X.Equals(tc.Y), HashCodeEquality: tc.X.GetHashCode() == tc.Y.GetHashCode()))
            .ThenReturns()
            .And((tc, rv) => rv.Equality.Should().Be(tc.ExpectedEquality))
            .And((tc, rv) => rv.HashCodeEquality.Should().Be(tc.ExpectedEquality)); // <- yeah yeah, strictly speaking not the right thing to be asserting, but..

        private class MyHashCodeCollision
        {
            private readonly string symbol;

            public MyHashCodeCollision(string symbol) => this.symbol = symbol;

            public override int GetHashCode() => 0;

            public override bool Equals(object? obj) => obj is MyHashCodeCollision mhcc && symbol.Equals(mhcc.symbol);
        }
    }
}
