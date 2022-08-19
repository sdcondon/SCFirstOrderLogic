using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;
using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.Inference.Chaining
{
    public static class SimpleForwardChainingKnowledgeBaseTests
    {
        // An enumeration of queries that are expected to succeed
        public static IEnumerable<SimpleForwardChainingQuery> BasicPositiveQueries()
        {
            static OperablePredicate IsKing(OperableTerm term) => new Predicate(nameof(IsKing), term);
            static OperablePredicate IsGreedy(OperableTerm term) => new Predicate(nameof(IsGreedy), term);
            static OperablePredicate IsEvil(OperableTerm term) => new Predicate(nameof(IsEvil), term);
            OperableConstant john = new OperableConstant(nameof(john));
            OperableConstant richard = new OperableConstant(nameof(richard));

            // trivial
            var kb = new SimpleForwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            yield return kb.CreateQuery(IsKing(john));

            // single conjunct, single step
            kb = new SimpleForwardChainingKnowledgeBase();
            kb.Tell(IsGreedy(john));
            kb.Tell(ForAll(X, If(IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(john));

            // two conjuncts, single step
            kb = new SimpleForwardChainingKnowledgeBase();
            kb.Tell(IsGreedy(john));
            kb.Tell(IsKing(john));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(john));
        }

        public static Test PositiveTestCases => TestThat
            .GivenTestContext()
            .AndEachOf(BasicPositiveQueries)
            .When((cxt, query) => query.Execute())
            .ThenReturns()
            .And((_, _, rv) => rv.Should().BeTrue())
            .And((_, query, _) => query.Result.Should().BeTrue())
            .And((cxt, query, _) => cxt.WriteOutput(query.ResultExplanation));

        // An enumeration of queries that are expected to fail
        public static IEnumerable<SimpleForwardChainingQuery> BasicNegativeQueries()
        {
            static OperablePredicate IsKing(OperableTerm term) => new Predicate(nameof(IsKing), term);
            static OperablePredicate IsGreedy(OperableTerm term) => new Predicate(nameof(IsGreedy), term);
            static OperablePredicate IsEvil(OperableTerm term) => new Predicate(nameof(IsEvil), term);
            OperableConstant john = new OperableConstant(nameof(john));
            OperableConstant richard = new OperableConstant(nameof(richard));

            // no matching clause
            var kb = new SimpleForwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            kb.Tell(IsGreedy(john));
            yield return kb.CreateQuery(IsEvil(X));

            // clause with not all conjuncts satisfied
            kb = new SimpleForwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(X));

            // no unifier will work - x is either John or Richard - it can't be both:
            kb = new SimpleForwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            kb.Tell(IsGreedy(richard));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(X));
        }

        public static Test NegativeTestCases => TestThat
            .GivenEachOf(BasicNegativeQueries)
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeFalse())
            .And((query, _) => query.Result.Should().BeFalse());

        public static Test CrimeDomainExample => TestThat
            .Given(cxt =>
            {
                var kb = new SimpleForwardChainingKnowledgeBase();
                kb.Tell(CrimeDomain.Axioms);

                return (cxt, query: kb.CreateQuery(IsCriminal(West)));
            })
            .When(given => given.query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeTrue())
            .And((given, _) => given.query.Result.Should().BeTrue())
            .And((given, _) => given.cxt.WriteOutput(given.query.ResultExplanation));
    }
}
