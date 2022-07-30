using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using SCFirstOrderLogic.Inference;
using SCFirstOrderLogic.Inference.BackwardChaining;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Benchmarks.Inference.BackwardChaining
{
    public static class AltBackwardChainingKnowledgeBase_FromAIaMATests
    {
        public static Test CrimeDomainExample => TestThat
            .Given(() =>
            {
                var kb = new AltBackwardChainingKnowledgeBase_FromAIaMA();
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

                var kb = new AltBackwardChainingKnowledgeBase_FromAIaMA();
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
    }
}
