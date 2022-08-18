using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference;
using SCFirstOrderLogic.Inference.Chaining;
using SCFirstOrderLogic.SentenceFormatting;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Alternatives.Inference.Chaining
{
    public static class BackwardChainingKnowledgeBase_FromAIaMATests
    {
        public static Test PositiveTestCases => TestThat
            .GivenTestContext()
            .AndEachOf(() => new BackwardChainingKnowledgeBase_FromAIaMA.Query[]
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

                // Simple multiple substitutions
                MakeQuery(
                    query: IsKing(X),
                    kb: new Sentence[]
                    {
                        IsKing(John),
                        IsKing(Richard),
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
            .And((cxt, query, _) => cxt.WriteOutputLine(query.Explain())); // Going to replace with full proof trees, so no point asserting on subs for now.

        public static Test NegativeTestCases => TestThat
            .GivenEachOf(() => new BackwardChainingKnowledgeBase_FromAIaMA.Query[]
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

        private static BackwardChainingKnowledgeBase_FromAIaMA.Query MakeQuery(Sentence query, IEnumerable<Sentence> kb)
        {
            var knowledgeBase = new BackwardChainingKnowledgeBase_FromAIaMA();
            knowledgeBase.Tell(kb);
            return knowledgeBase.CreateQueryAsync(query).GetAwaiter().GetResult();
        }
    }
}
