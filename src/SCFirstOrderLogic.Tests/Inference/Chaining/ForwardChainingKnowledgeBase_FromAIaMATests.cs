using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference;
using System.Collections.Generic;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.TestUtilities.GreedyKingsDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    public static class ForwardChainingKnowledgeBase_FromAIaMATests
    {
        public static Test PositiveScenarios => TestThat
            .GivenTestContext()
            .AndEachOf(() => new ForwardChainingKnowledgeBase_FromAIaMA.Query[]
            {
                // Trivial
                // Commented out because it actually fails given the book listing.. Don't want to deviate from the reference implementation though,
                // so just commenting out the test. See SimpleForwardChainingKnowledgeBase for the fix..
                ////MakeQuery(
                ////    query: IsKing(John),
                ////    kb: new Sentence[]
                ////    {
                ////        IsKing(John)
                ////    }),
                
                // Trivial - with multiple substitutions
                // Commented out because it actually fails given the book listing.. Don't want to deviate from the reference implementation though,
                // so just commenting out the test. See SimpleForwardChainingKnowledgeBase for the fix..
                ////MakeQuery(
                ////    query: IsKing(X),
                ////    kb: new Sentence[]
                ////    {
                ////        IsKing(John),
                ////        IsKing(Richard),
                ////    }),

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

                // More complex - Crime example domain
                MakeQuery(
                    query: IsCriminal(West),
                    kb: Axioms),
            })
            .When((cxt, query) => query.Execute())
            .ThenReturns()
            .And((_, _, rv) => rv.Should().BeTrue())
            .And((_, query, _) => query.Result.Should().BeTrue());

        public static Test NegativeScenarios => TestThat
            .GivenEachOf(() => new ForwardChainingKnowledgeBase_FromAIaMA.Query[]
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

        private static ForwardChainingKnowledgeBase_FromAIaMA.Query MakeQuery(Sentence query, IEnumerable<Sentence> kb)
        {
            var knowledgeBase = new ForwardChainingKnowledgeBase_FromAIaMA();
            knowledgeBase.Tell(kb);
            return (ForwardChainingKnowledgeBase_FromAIaMA.Query)knowledgeBase.CreateQuery(query);
        }
    }
}
