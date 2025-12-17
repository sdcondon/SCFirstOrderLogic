using System;
using System.Linq;
using static SCFirstOrderLogic.FormulaCreation.Specialised.GenericDomainOperableFormulaFactory;

namespace SCFirstOrderLogic.TestUtilities;

public static class CNFClauseHelper
{
    public static CNFClause MakeRandomClause()
    {
        return new CNFClause(Enumerable
            .Range(0, Random.Shared.Next(1, 2))
            .Select(i => new Literal(MakeRandomLiteral())));

        Formula MakeRandomLiteral()
        {
            return Random.Shared.Next(1, 12) switch
            {
                1 => P(),
                2 => !P(),
                3 => Q(),
                4 => !Q(),
                5 => P(MakeRandomTerm()),
                6 => !P(MakeRandomTerm()),
                7 => P(MakeRandomTerm(), MakeRandomTerm()),
                8 => !P(MakeRandomTerm(), MakeRandomTerm()),
                9 => Q(MakeRandomTerm()),
                10 => !Q(MakeRandomTerm()),
                11 => Q(MakeRandomTerm(), MakeRandomTerm()),
                12 => !Q(MakeRandomTerm(), MakeRandomTerm()),
                _ => throw new Exception()
            };
        }

        Term MakeRandomTerm()
        {
            return Random.Shared.Next(1, 14) switch
            {
                1 => C,
                2 => D,
                3 => F(),
                4 => G(),
                5 => U,
                6 => V,
                7 => W,
                8 => X,
                9 => Y,
                10 => Z,
                11 => F(MakeRandomTerm()),
                12 => F(MakeRandomTerm(), MakeRandomTerm()),
                13 => G(MakeRandomTerm()),
                14 => H(MakeRandomTerm(), MakeRandomTerm()),
                _ => throw new Exception()
            };
        }
    }
}
