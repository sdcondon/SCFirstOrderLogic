using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using System.Linq;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public class VariableUnifyingEqualityComparerTests
{
    public static Test EqualityBehaviour_Positive => TestThat
        .GivenEachOf<EqualityTestCase>(() =>
        [
            new(
                X: CNFClause.Empty,
                Y: CNFClause.Empty),

            new(
                X: P(C, F(X)),
                Y: P(C, F(Y))),

            new(
                X: P(F(X), F(Y)),
                Y: P(F(Y), F(X))),

            new( // todo: this shouldn't pass with current implementation..?! meh, tired. tomo..
                X: P(X, Y, Z),
                Y: P(Y, Z, X)),
        ])
        .When(tc =>
        {
            var comparer = new VariableUnifyingEqualityComparer();
            return (XEqualsY: comparer.Equals(tc.X, tc.Y), YEqualsX: comparer.Equals(tc.Y, tc.X), HashCodeEquality: comparer.GetHashCode(tc.X) == comparer.GetHashCode(tc.Y));
        })
        .ThenReturns()
        .And((_, rv) => rv.XEqualsY.Should().BeTrue())
        .And((_, rv) => rv.YEqualsX.Should().BeTrue())
        .And((_, rv) => rv.HashCodeEquality.Should().BeTrue());

    public static Test EqualityBehaviour_Negative => TestThat
        .GivenEachOf<EqualityTestCase>(() =>
        [
            new(
                X: P(C, X),
                Y: P(C, D)),

            new (
                X: P(C, X),
                Y: P(Y, D)),

            new (
                X: P(C, X),
                Y: P(Y, F(Y))),

            new (
                X: P(X, X),
                Y: P(Y, C)),

            new (
                X: P(X, X),
                Y: P(X, Y)),

            new (
                X: P(X, X),
                Y: P(A, B)),

            new (
                X: P(C, X),
                Y: P(X, D)),

            new (
                X: P(C, X),
                Y: P(C, F(X))),

            new (
                X: P(X, F(X)),
                Y: P(G(Y), Y)),

            new (
                X: P(X) | Q(X),
                Y: P(X) | Q(Y)),
        ])
        .When(tc =>
        {
            var comparer = new VariableUnifyingEqualityComparer();
            return (XEqualsY: comparer.Equals(tc.X, tc.Y), YEqualsX: comparer.Equals(tc.Y, tc.X));
        })
        .ThenReturns()
        .And((tc, rv) => rv.XEqualsY.Should().BeFalse())
        .And((tc, rv) => rv.YEqualsX.Should().BeFalse());

    private record EqualityTestCase(CNFClause X, CNFClause Y)
    {
        public EqualityTestCase(Sentence X, Sentence Y)
            : this(X.ToCNF().Clauses.Single(), Y.ToCNF().Clauses.Single()) { }

        public EqualityTestCase(CNFClause X, Sentence Y)
            : this(X, Y.ToCNF().Clauses.Single()) { }

        public EqualityTestCase(Sentence X, CNFClause Y)
            : this(X.ToCNF().Clauses.Single(), Y) { }
    }
}
