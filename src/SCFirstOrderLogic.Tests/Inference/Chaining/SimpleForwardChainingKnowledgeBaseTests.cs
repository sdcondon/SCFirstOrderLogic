﻿using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.SentenceFactory;
using System.Collections.Generic;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.SentenceFactory.CrimeDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    public static class SimpleForwardChainingKnowledgeBaseTests
    {
        public static Test PositiveScenarios => TestThat
            .GivenTestContext()
            .AndEachOf(() => new SimpleForwardChainingQuery[]
            {
                // trivial
                MakeQuery(
                    query: IsKing(John),
                    kb: new Sentence[]
                    {
                        IsKing(John)
                    }),

                // single conjunct, single step
                MakeQuery(
                    query: IsEvil(John),
                    kb: new Sentence[]
                    {
                        IsGreedy(John),
                        AllGreedyAreEvil
                    }),

                // two conjuncts, single step
                MakeQuery(
                    query: IsEvil(John),
                    kb: new Sentence[]
                    {
                        IsGreedy(John),
                        IsKing(John),
                        AllGreedyKingsAreEvil
                    }),

                // Two applicable rules, each with two conjuncts, single step
                MakeQuery(
                    query: IsEvil(X),
                    kb: new Sentence[]
                    {
                        IsKing(John),
                        IsGreedy(Mary),
                        IsQueen(Mary),
                        AllGreedyKingsAreEvil,
                        AllGreedyQueensAreEvil,
                    }),

                // Multiple possible substitutions
                MakeQuery(
                    query: IsKing(X),
                    kb: new Sentence[]
                    {
                        IsKing(John),
                        IsKing(Richard),
                    }),

                // Uses same var twice in same proof
                MakeQuery(
                    query: Knows(John, Mary),
                    kb: new Sentence[]
                    {
                        AllGreedyAreEvil,
                        AllEvilKnowEachOther,
                        IsGreedy(John),
                        IsGreedy(Mary),
                    }),

                // More complex - Crime example domain
                MakeQuery(
                    query: IsCriminal(West),
                    kb: CrimeDomain.Axioms),
            })
            .When((cxt, query) => query.Execute())
            .ThenReturns()
            .And((_, _, rv) => rv.Should().BeTrue())
            .And((_, query, _) => query.Result.Should().BeTrue())
            .And((cxt, query, _) => cxt.WriteOutput(query.ResultExplanation));

        public static Test NegativeScenarios => TestThat
            .GivenEachOf(() => new SimpleForwardChainingQuery[]
            {
                // no matching clause
                MakeQuery(
                    query: IsEvil(X),
                    kb: new Sentence[]
                    {
                        IsKing(John),
                        IsGreedy(John),
                    }),

                // clause with not all conjuncts satisfied
                MakeQuery(
                    query: IsEvil(X),
                    kb: new Sentence[]
                    {
                        IsKing(John),
                        AllGreedyKingsAreEvil,
                    }),

                // no unifier will work - x is either John or Richard - it can't be both:
                MakeQuery(
                    query: IsEvil(X),
                    kb: new Sentence[]
                    {
                        IsKing(John),
                        IsGreedy(Richard),
                        AllGreedyKingsAreEvil,
                    }),
            })
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeFalse())
            .And((query, _) => query.Result.Should().BeFalse());

        private static SimpleForwardChainingQuery MakeQuery(Sentence query, IEnumerable<Sentence> kb)
        {
            var knowledgeBase = new SimpleForwardChainingKnowledgeBase();
            knowledgeBase.Tell(kb);
            return knowledgeBase.CreateQuery(query);
        }
    }
}
