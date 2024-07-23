using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    private static OperableFunction C => new Function(nameof(C));
    private static OperableFunction D => new Function(nameof(D));
    private static OperableFunction E => new Function(nameof(E));
    private static OperableFunction F(params Term[] arguments) => new Function(nameof(F), arguments);
    private static OperableFunction G(params Term[] arguments) => new Function(nameof(G), arguments);
    private static OperableFunction H(params Term[] arguments) => new Function(nameof(H), arguments);
    private static OperablePredicate P(params Term[] arguments) => new Predicate(nameof(P), arguments);
    private static OperablePredicate Q(params Term[] arguments) => new Predicate(nameof(Q), arguments);
    private static OperablePredicate R(params Term[] arguments) => new Predicate(nameof(R), arguments);
    private static OperableVariableReference X => new VariableReference(nameof(X));
    private static OperableVariableReference Y => new VariableReference(nameof(Y));
    private static OperableVariableReference Z => new VariableReference(nameof(Z));

    private record GetTestCase(IEnumerable<Sentence> Content, Sentence Query, IEnumerable<Sentence> Expected);

    public static Test GetSubsumedBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new( // Empty index
                Content: [],
                Query: P(),
                Expected: []),

            new( // Trivial exact match
                Content: [P()],
                Query: P(),
                Expected: [P()]),

            new( // Single matching predicate for which all args are instances of query arg
                Content: [P(C, Y)],
                Query: P(X, Y),
                Expected: [P(C, Y)]),

            new( // variable identifiers don't matter
                Content: [P(C, Y)],
                Query: P(Y, Z),
                Expected: [P(C, Y)]),

            new( // variable binding consistency does matter
                Content: [P(C, Y)],
                Query: P(X, X),
                Expected: []),

            new( // Extra literal
                Content: [P(C) | Q(D)],
                Query: P(X),
                Expected: [P(C) | Q(D)]),

            new( // Missing literal
                Content: [P(C)],
                Query: P(X) | Q(D),
                Expected: []),
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex(c => null, tc.Content.Select(s => new CNFClause(s)));
            return index.GetSubsumedClauses(tc.Query.ToCNF().Clauses.Single());
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected.Select(s => s.ToCNF().Clauses.Single())));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex(c => null, tc.Content.Select(s => new CNFClause(s)));
            return index.GetSubsumingClauses(tc.Query.ToCNF().Clauses.Single());
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));
}
