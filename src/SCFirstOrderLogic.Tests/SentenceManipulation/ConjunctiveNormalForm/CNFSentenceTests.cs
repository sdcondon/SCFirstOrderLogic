using FluentAssertions;
using FlUnit;
using System.Linq;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public static class CNFSentenceTests
    {
        private static Sentence P => new Predicate("P");
        private static Sentence Q => new Predicate("Q");
        private static Sentence R => new Predicate("R");

        public static Test Construction => TestThat
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
                    // TODO: We don't remove the trivially true clauses (e.g. P ∨ ¬P) - none of the source material
                    // references this as being part of the normalisation process. But we probably should..
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
            .When(tc => new CNFSentence(tc.Sentence))
            .ThenReturns((tc, retVal) => retVal.Should().BeEquivalentTo(tc.ExpectedCNF));
    }
}
