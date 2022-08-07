using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    public static class SimpleBackwardChainingKnowledgeBaseTests
    {
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

        public static Test BasicExample => TestThat
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

        public record NegativeTestCase(SimpleBackwardChainingKnowledgeBase KB, Predicate Query);

        public static Test NegativeExample => TestThat
            .GivenEachOf(() => new NegativeTestCase[]
            {
            })
            .When(tc =>
            {
                var query = tc.KB.CreateQuery(tc.Query);
                query.Execute();
                return query;
            })
            .ThenReturns()
            .And((_, rv) => rv.Should().BeFalse())
            .And((query, _) => query.Result.Should().BeFalse())
            .And((query, _) => query.Substitutions.Select(s => s.Bindings).Should().BeEquivalentTo(new Dictionary<VariableReference, Term>[]
            {
                new() { [new VariableReference("x")] = new Constant("John") },
                new() { [new VariableReference("x")] = new Constant("Richard") },
            }, opts => opts.RespectingRuntimeTypes()));
    }
}
