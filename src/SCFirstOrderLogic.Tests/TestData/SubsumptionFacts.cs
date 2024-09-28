using System.Collections.Generic;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.TestData;

public static class SubsumptionFacts
{
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
}
