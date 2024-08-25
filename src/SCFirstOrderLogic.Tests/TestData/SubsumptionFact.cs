using SCFirstOrderLogic.SentenceManipulation.Normalisation;
using System.Linq;

namespace SCFirstOrderLogic.TestData;

public record SubsumptionFact(CNFClause X, CNFClause Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
{
    public SubsumptionFact(Sentence X, Sentence Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
        : this(new CNFClause(X), new CNFClause(Y), IsXSubsumedByY, IsYSubsumedByX) { }

    public SubsumptionFact(CNFClause X, Sentence Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
        : this(X, new CNFClause(Y), IsXSubsumedByY, IsYSubsumedByX) { }

    public SubsumptionFact(Sentence X, CNFClause Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
        : this(new CNFClause(X), Y, IsXSubsumedByY, IsYSubsumedByX) { }
}
