using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.FormulaCreation.Specialised.GenericDomainOperableFormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation.Substitution;

public class VariableIdAgnosticEqualityComparerTests
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

            new(
                X: P(X, Y, Z),
                Y: P(Y, Z, X)),
        ])
        .When(tc =>
        {
            var comparer = new VariableIdAgnosticEqualityComparer();
            return (
                XEqualsY: comparer.Equals(tc.X, tc.Y),
                YEqualsX: comparer.Equals(tc.Y, tc.X),
                HashCodeEquality: comparer.GetHashCode(tc.X) == comparer.GetHashCode(tc.Y));
        })
        .ThenReturns()
        .And((_, outcome) => outcome.XEqualsY.Should().BeTrue())
        .And((_, outcome) => outcome.YEqualsX.Should().BeTrue())
        .And((_, outcome) => outcome.HashCodeEquality.Should().BeTrue());

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
            var comparer = new VariableIdAgnosticEqualityComparer();
            return (XEqualsY: comparer.Equals(tc.X, tc.Y), YEqualsX: comparer.Equals(tc.Y, tc.X));
        })
        .ThenReturns()
        .And((_, outcome) => outcome.XEqualsY.Should().BeFalse())
        .And((_, outcome) => outcome.YEqualsX.Should().BeFalse());

    private record EqualityTestCase(CNFClause X, CNFClause Y)
    {
        public EqualityTestCase(Formula X, Formula Y)
            : this(new CNFClause(X), new CNFClause(Y)) { }

        public EqualityTestCase(CNFClause X, Formula Y)
            : this(X, new CNFClause(Y)) { }

        public EqualityTestCase(Formula X, CNFClause Y)
            : this(new CNFClause(X), Y) { }
    }
}
