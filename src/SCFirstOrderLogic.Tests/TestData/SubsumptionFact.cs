using SCFirstOrderLogic.FormulaManipulation.Normalisation;
using System.Linq;

namespace SCFirstOrderLogic.TestData;

public record SubsumptionFact(CNFClause X, CNFClause Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
{
    public SubsumptionFact(Formula X, Formula Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
        : this(new CNFClause(X), new CNFClause(Y), IsXSubsumedByY, IsYSubsumedByX) { }

    public SubsumptionFact(CNFClause X, Formula Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
        : this(X, new CNFClause(Y), IsXSubsumedByY, IsYSubsumedByX) { }

    public SubsumptionFact(Formula X, CNFClause Y, bool IsXSubsumedByY, bool IsYSubsumedByX)
        : this(new CNFClause(X), Y, IsXSubsumedByY, IsYSubsumedByX) { }
}
