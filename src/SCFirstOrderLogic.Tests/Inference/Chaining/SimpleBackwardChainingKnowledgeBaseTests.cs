using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    public static class SimpleBackwardChainingKnowledgeBaseTests
    {
        // An enumeration of simple queries that are expected to succeed
        public static IEnumerable<SimpleBackwardChainingQuery> BasicPositiveQueries()
        {
            static OperablePredicate IsKing(OperableTerm term) => new Predicate(nameof(IsKing), term);
            static OperablePredicate IsGreedy(OperableTerm term) => new Predicate(nameof(IsGreedy), term);
            static OperablePredicate IsEvil(OperableTerm term) => new Predicate(nameof(IsEvil), term);
            OperableConstant john = new OperableConstant(nameof(john));
            OperableConstant richard = new OperableConstant(nameof(richard));

            // trivial
            var kb = new SimpleBackwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            yield return kb.CreateQuery(IsKing(john));

            // single conjunct, single step
            kb = new SimpleBackwardChainingKnowledgeBase();
            kb.Tell(IsGreedy(john));
            kb.Tell(ForAll(X, If(IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(john));

            // two conjuncts, single step
            kb = new SimpleBackwardChainingKnowledgeBase();
            kb.Tell(IsGreedy(john));
            kb.Tell(IsKing(john));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(john));
        }

        public static Test BasicPositiveTestCases => TestThat
            .GivenTestContext()
            .AndEachOf(BasicPositiveQueries)
            .When((cxt, query) => query.Execute())
            .ThenReturns()
            .And((_, _, rv) => rv.Should().BeTrue())
            .And((_, query, _) => query.Result.Should().BeTrue());

        // An enumeration of queries that are expected to fail
        public static IEnumerable<SimpleBackwardChainingQuery> BasicNegativeQueries()
        {
            static OperablePredicate IsKing(OperableTerm term) => new Predicate(nameof(IsKing), term);
            static OperablePredicate IsGreedy(OperableTerm term) => new Predicate(nameof(IsGreedy), term);
            static OperablePredicate IsEvil(OperableTerm term) => new Predicate(nameof(IsEvil), term);
            OperableConstant john = new OperableConstant(nameof(john));
            OperableConstant richard = new OperableConstant(nameof(richard));

            // no matching clause
            var kb = new SimpleBackwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            kb.Tell(IsGreedy(john));
            yield return kb.CreateQuery(IsEvil(X));

            // clause with not all conjuncts satisfied
            kb = new SimpleBackwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(X));

            // no unifier will work - x is either John or Richard - it can't be both:
            kb = new SimpleBackwardChainingKnowledgeBase();
            kb.Tell(IsKing(john));
            kb.Tell(IsGreedy(richard));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(X));
        }

        public static Test BasicNegativeTestCases => TestThat
            .GivenEachOf(BasicNegativeQueries)
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeFalse())
            .And((query, _) => query.Result.Should().BeFalse());

        public static Test CrimeDomainExample => TestThat
            .Given(() =>
            {
                var kb = new SimpleBackwardChainingKnowledgeBase();
                kb.Tell(CrimeDomain.Axioms);

                return kb.CreateQuery(IsCriminal(West));
            })
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeTrue())
            .And((query, _) => query.Result.Should().BeTrue());

        public static Test MultipleSubstitutions => TestThat
            .Given(() =>
            {
                static Predicate IsPerson(Term term) => new Predicate(nameof(IsPerson), term);

                var kb = new SimpleBackwardChainingKnowledgeBase();
                kb.Tell(IsPerson(new Constant("John")));
                kb.Tell(IsPerson(new Constant("Richard")));

                return kb.CreateQuery(IsPerson(new VariableReference("x")));
            })
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeTrue())
            .And((query, _) => query.Result.Should().BeTrue())
            .And((query, _) => query.Substitutions.Select(s => s.Bindings).Should().BeEquivalentTo(new Dictionary<VariableReference, Term>[]
            {
                new() { [new VariableReference("x")] = new Constant("John") },
                new() { [new VariableReference("x")] = new Constant("Richard") },
            }, opts => opts.RespectingRuntimeTypes()));
    }
}
