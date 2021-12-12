using FluentAssertions;
using FlUnit;
using static LinqToKB.FirstOrderLogic.Sentence;

namespace LinqToKB.FirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = false },
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = true },
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = true },
                                    new { AtomicSentence = new { Sentence = Q }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = R }, IsNegated = false },
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = Q }, IsNegated = true }
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = R }, IsNegated = true }
                                }
                            }
                        }
                    }
                },
                new
                {
                    //// NB: We don't remove the trivially true clauses (e.g. P ∨ ¬P) - none of the source material
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = true },
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
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = Q }, IsNegated = true },
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
                                    new { AtomicSentence = new { Sentence = Q }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = P }, IsNegated = true }
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
                                    new { AtomicSentence = new { Sentence = Q }, IsNegated = false },
                                    new { AtomicSentence = new { Sentence = Q }, IsNegated = true }
                                }
                            }
                        }
                    }
                },

            })
            .When(tc => new CNFSentence(tc.Sentence))
            .ThenReturns((tc, retVal) => retVal.Should().BeEquivalentTo(tc.ExpectedCNF));

        ////[Fact]
        ////public void ResolutionOfResolvableClauses()
        ////{
        ////    var resolvents = CNFClause<MyModel>.Resolve(
        ////        new CNFExpression<MyModel>(m => !m.P || m.Q).Clauses.Single(),
        ////        new CNFExpression<MyModel>(m => m.P).Clauses.Single());
        ////    Assert.Single(resolvents, new CNFExpression<MyModel>(m => m.Q).Clauses.Single());
        ////}

        ////[Fact]
        ////public void ResolutionOfResolvableClauses2()
        ////{
        ////    var resolvents = CNFClause<MyModel>.Resolve(
        ////        new CNFExpression<MyModel>(m => !m.P || m.Q).Clauses.Single(),
        ////        new CNFExpression<MyModel>(m => m.P || m.R).Clauses.Single());
        ////    Assert.Single(resolvents, new CNFExpression<MyModel>(m => m.Q || m.R).Clauses.Single());
        ////}

        ////[Fact]
        ////public void ResolutionOfUnresolvableClauses()
        ////{
        ////    var resolvents = CNFClause<MyModel>.Resolve(
        ////        new CNFExpression<MyModel>(m => m.P).Clauses.Single(),
        ////        new CNFExpression<MyModel>(m => m.Q).Clauses.Single());
        ////    Assert.Empty(resolvents);
        ////}

        ////[Fact]
        ////public void ResolutionOfComplementaryUnitClauses()
        ////{
        ////    var resolvents = CNFClause<MyModel>.Resolve(
        ////        new CNFExpression<MyModel>(m => m.P).Clauses.Single(),
        ////        new CNFExpression<MyModel>(m => !m.P).Clauses.Single());
        ////    Assert.Single(resolvents, CNFClause<MyModel>.Empty);
        ////}

        ////[Fact]
        ////public void ResolutionOfMultiplyResolvableClauses()
        ////{
        ////    var resolvents = CNFClause<MyModel>.Resolve(
        ////        new CNFExpression<MyModel>(m => m.P || m.Q).Clauses.Single(),
        ////        new CNFExpression<MyModel>(m => !m.P || !m.Q).Clauses.Single());

        ////    // Both of these resolvents are trivially true - so largely useless - should the method return no resolvents in this case?
        ////    // Are all cases where more than one resolvent would be returned useless? Should the method return a (potentially null) clause instead of a enumerable?
        ////    resolvents.Should().BeEquivalentTo(new[]
        ////    {
        ////        new CNFExpression<MyModel>(m => m.P || !m.P).Clauses.Single(),
        ////        new CNFExpression<MyModel>(m => m.Q || !m.Q).Clauses.Single()
        ////    });
        ////}
    }
}
