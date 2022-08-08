using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference;
using SCFirstOrderLogic.Inference.Chaining;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Alternatives.Inference.Chaining
{
    public static class BackwardChainingKnowledgeBase_FromAIaMATests
    {
        public static Test CrimeDomainExample => TestThat
            .Given(() =>
            {
                var kb = new BackwardChainingKnowledgeBase_FromAIaMA();
                kb.Tell(CrimeDomain.Axioms);

                return kb.CreateQueryAsync(IsCriminal(West)).Result;
            })
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeTrue())
            .And((query, _) => query.Result.Should().BeTrue());

        public static Test BasicExample => TestThat
            .Given(() =>
            {
                static Predicate IsPerson(Term term) => new Predicate(nameof(IsPerson), term);

                var kb = new BackwardChainingKnowledgeBase_FromAIaMA();
                kb.Tell(IsPerson(new Constant("John")));
                kb.Tell(IsPerson(new Constant("Richard")));

                return kb.CreateQueryAsync(IsPerson(new VariableReference("x"))).Result;
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

        // An enumeration of queries that are expected to fail
        public static IEnumerable<SimpleBackwardChainingQuery> NegativeQueries()
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
            kb.Tell(IsEvil(richard));
            kb.Tell(ForAll(X, If(IsKing(X) & IsGreedy(X), IsEvil(X))));
            yield return kb.CreateQuery(IsEvil(X));
        }

        public static Test NegativeTestCases => TestThat
            .GivenEachOf(NegativeQueries)
            .When(query => query.Execute())
            .ThenReturns()
            .And((_, rv) => rv.Should().BeFalse())
            .And((query, _) => query.Result.Should().BeFalse());
    }
}
