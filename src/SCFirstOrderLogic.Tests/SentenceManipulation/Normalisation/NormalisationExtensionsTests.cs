using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

public static class NormalisationExtensionsTests
{
    private static Formula P => new Predicate("P");
    private static Formula Q => new Predicate("Q");
    private static Formula R => new Predicate("R");

    public static Test ToCNFBehaviour => TestThat
        .GivenEachOf(() => new[]
        {
            new
            {
                Sentence = P,
                ExpectedCNF = new
                {
                    Clauses = new[]
                    {
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = true,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = false },
                            }
                        }
                    }
                }
            },
            new
            {
                Sentence = Not(P),
                ExpectedCNF = new
                {
                    Clauses = new[]
                    {
                        new
                        {
                            IsDefiniteClause = false,
                            IsGoalClause = true,
                            IsHornClause = true,
                            IsUnitClause = true,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = true },
                            }
                        }
                    }
                }
            },
            new
            {
                Sentence = Iff(P, Not(And(Not(Q), Not(R)))),
                ExpectedCNF = new
                {
                    Clauses = new[]
                    {
                        new
                        {
                            IsDefiniteClause = false,
                            IsGoalClause = false,
                            IsHornClause = false,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = true },
                                new { Predicate = Q, IsNegated = false },
                                new { Predicate = R, IsNegated = false },
                            }
                        },
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = false },
                                new { Predicate = Q, IsNegated = true }
                            }
                        },
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = false },
                                new { Predicate = R, IsNegated = true }
                            }
                        }
                    }
                }
            },
            new
            {
                // TODO-ZZZ: We don't remove the trivially true clauses (e.g. P ∨ ¬P), and *maybe* should.
                // The trivially true clauses don't cause any/much harm (note that you'll just never get a resolution with them
                // that tells you anything new - since the resolvent clause will always be the same as the other input one) - 
                // and checking for trivially true clauses would come at a performance cost that could well outweigh
                // the cost of having trivially true ones in there. So, leaving behaviour like this for the mo - would
                // least need to do some perf testing before changing this.
                Sentence = Or(And(P, Q), And(Not(P), Not(Q))),
                ExpectedCNF = new
                {
                    Clauses = new[]
                    {
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = false },
                                new { Predicate = P, IsNegated = true },
                            }
                        },
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = P, IsNegated = false },
                                new { Predicate = Q, IsNegated = true },
                            }
                        },
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = Q, IsNegated = false },
                                new { Predicate = P, IsNegated = true }
                            }
                        },
                        new
                        {
                            IsDefiniteClause = true,
                            IsGoalClause = false,
                            IsHornClause = true,
                            IsUnitClause = false,
                            Literals = new[]
                            {
                                new { Predicate = Q, IsNegated = false },
                                new { Predicate = Q, IsNegated = true }
                            }
                        }
                    }
                }
            },
        })
        .When(tc => tc.Sentence.ToCNF())
        .ThenReturns((tc, retVal) => retVal.Should().BeEquivalentTo(tc.ExpectedCNF));
}
