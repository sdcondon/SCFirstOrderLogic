using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    private static Function C => new Function(nameof(C));
    private static Function D => new Function(nameof(D));
    private static Function E => new Function(nameof(E));
    private static Function F(params Term[] arguments) => new Function(nameof(F), arguments);
    private static Function G(params Term[] arguments) => new Function(nameof(G), arguments);
    private static Function H(params Term[] arguments) => new Function(nameof(H), arguments);
    private static Predicate P(params Term[] arguments) => new Predicate(nameof(P), arguments);
    private static Predicate Q(params Term[] arguments) => new Predicate(nameof(Q), arguments);
    private static Predicate R(params Term[] arguments) => new Predicate(nameof(R), arguments);
    private static VariableReference X => new VariableReference(nameof(X));
    private static VariableReference Y => new VariableReference(nameof(Y));
    private static VariableReference Z => new VariableReference(nameof(Z));

    private record GetTestCase(IEnumerable<CNFClause> Content, CNFClause Query, IEnumerable<CNFClause> Expected);

    public static Test GetSubsumedBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new( // Empty index
                Content: [],
                Query: new(P()),
                Expected: []),

            new( // Trivial exact match
                Content: [new(P())],
                Query: new(P()),
                Expected: [new(P())]),

            new( // Single matching predicate for which all args are instances of query arg
                Content: [new(P(C, Y))],
                Query: new(P(X, Y)),
                Expected: [new(P(C, Y))]),

            new( // variable identifiers don't matter
                Content: [new(P(C, Y))],
                Query: new(P(Y, Z)),
                Expected: [new(P(C, Y))]),

            new( // variable binding consistency does matter
                Content: [new(P(C, Y))],
                Query: new(P(X, X)),
                Expected: []),

            new( // Single matching predicate for which all args are instances of query arg
                Content: [new(P(C, Y))],
                Query: new(P(X, Y)),
                Expected: [new(P(C, Y))])
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex(c => null, tc.Content);
            return index.GetSubsumedClauses(tc.Query);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex(c => null, tc.Content);
            return index.GetSubsumingClauses(tc.Query);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));
}
