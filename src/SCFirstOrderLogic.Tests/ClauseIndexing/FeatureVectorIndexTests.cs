using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.ClauseIndexing;

public static class FeatureVectorIndexTests
{
    private static readonly IEnumerable<Sentence> IndexContent =
    [
        P(),
        P(C),
        P(X),
        P(C) | P(D),
        P(C) | Q(C),
        P(C) | Q(D),
        P(C) | Q(X),
        P(X) | Q(Y),
        P(X) | Q(X),
        Q(C),
    ];

    private static OperableFunction C => new Function(nameof(C));
    private static OperableFunction D => new Function(nameof(D));
    private static OperablePredicate P(params Term[] arguments) => new Predicate(nameof(P), arguments);
    private static OperablePredicate Q(params Term[] arguments) => new Predicate(nameof(Q), arguments);
    private static OperableVariableReference X => new VariableReference(nameof(X));
    private static OperableVariableReference Y => new VariableReference(nameof(Y));
    private static OperableVariableReference Z => new VariableReference(nameof(Z));

    private record GetTestCase(Sentence Query, IEnumerable<Sentence> Expected);

    // TODO: shouldn't be allowed to add empty clause

    public static Test GetSubsumedBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new(Query: P(), Expected: [P()]),
            new(Query: P(C), Expected: [P(C), P(C) | P(D), P(C) | Q(C), P(C) | Q(D), P(C) | Q(X)]),
            new(Query: P(X), Expected: [P(C), P(X), P(C) | P(D), P(C) | Q(C), P(C) | Q(D), P(C) | Q(X), P(X) | Q(Y), P(X) | Q(X) ]),
            new(Query: P(C) | P(D), Expected: [P(C) | P(D)]),
            new(Query: P(C) | Q(X), Expected: [P(C) | Q(X), P(C) | Q(D), P(C) | Q(C)]),
            new(Query: P(X) | Q(Y), Expected: [P(X) | Q(Y), P(C) | Q(D), P(X) | Q(X), P(C) | Q(C)]),
            new(Query: P(C) | Q(D), Expected: [P(C) | Q(D)]),
            new(Query: P(X) | Q(X), Expected: [P(X) | Q(X), P(C) | Q(C)]),
            new(Query: P(C) | Q(C), Expected: [P(C) | Q(C)]),
            new(Query: Q(C), Expected: [Q(C), P(C) | Q(C)]),
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex(c => null, IndexContent.Select(s => new CNFClause(s)));
            return index.GetSubsumed(tc.Query.ToCNF().Clauses.Single());
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected.Select(s => s.ToCNF().Clauses.Single())));

    public static Test GetSubsumingBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new(Query: P(), Expected: [P(C)]),
            new(Query: P(X), Expected: []),
            new(Query: P(C), Expected: []),
            new(Query: P(C) | P(D), Expected: []),
            new(Query: P(C) | Q(X), Expected: []),
            new(Query: P(X) | Q(Y), Expected: []),
            new(Query: P(C) | Q(D), Expected: []),
            new(Query: P(X) | Q(X), Expected: []),
            new(Query: P(C) | Q(C), Expected: []),
            new(Query: Q(C), Expected: []),
        ])
        .When(tc =>
        {
            var index = new FeatureVectorIndex(c => null, IndexContent.Select(s => new CNFClause(s)));
            return index.GetSubsuming(tc.Query.ToCNF().Clauses.Single());
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.Expected));
}
