using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

public static class TermExtensionsTests
{
    public static Test OrdinaliseBehaviourTests => TestThat
        .GivenEachOf<OrdinaliseTestCase>(() =>
        [
            new(Input: F(), Expected: F()),
            new(Input: F(X), Expected: F(Var(0))),
            new(Input: F(G(X, Y), G(X, Z)), Expected: F(G(Var(0), Var(1)), G(Var(0), Var(2)))),
        ])
        .When(tc => tc.Input.Ordinalise())
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test IsInstanceOfBehaviourTests => TestThat
        .GivenEachOf<BinaryTestCase>(() =>
        [
            new(X: F(), Y: F(), Expected: true),
            new(X: F(A), Y: F(X), Expected: true),
            new(X: F(G(Y, A)), Y: F(X), Expected: true),
            new(X: F(X), Y: F(A), Expected: false),
            new(X: F(X, A), Y: F(A, X), Expected: false),
        ])
        .When(tc => tc.X.IsInstanceOf(tc.Y))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.Expected));

    public static Test IsGeneralisationOfBehaviourTests => TestThat
        .GivenEachOf<BinaryTestCase>(() =>
        [
            new(X: F(), Y: F(), Expected: true),
            new(X: F(X), Y: F(A), Expected: true),
            new(X: F(X), Y: F(G(Y, A)), Expected: true),
            new(X: F(A), Y: F(X), Expected: false),
            new(X: F(X, A), Y: F(A, X), Expected: false),
        ])
        .When(tc => tc.X.IsGeneralisationOf(tc.Y))
        .ThenReturns((tc, rv) => rv.Should().Be(tc.Expected));

    private static OperableFunction A => new Function(nameof(A));
    private static OperableFunction B => new Function(nameof(B));
    private static OperableFunction F(params Term[] arguments) => new Function(nameof(F), arguments);
    private static OperableFunction G(params Term[] arguments) => new Function(nameof(G), arguments);

    private record OrdinaliseTestCase(Term Input, Term Expected);

    private record BinaryTestCase(Term X, Term Y, bool Expected);
}
