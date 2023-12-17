using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.TermIndexing
{
    using static SCFirstOrderLogic.ExampleDomains.FromMpg.TermIndexingExample;

    [MemoryDiagnoser]
    [InProcess]
    public class PathTreeBenchmarks
    {
        private static readonly PathTree tree = new(ExampleTerms);
        private static readonly PathTree_WOVarBinding<Term> tree_withoutVarBinding = new(ExampleTerms.Select(t => KeyValuePair.Create(t, t)));
        private static readonly PathTree_GradualVarBinding<Term> tree_gradualVarBinding = new(ExampleTerms.Select(t => KeyValuePair.Create(t, t)));

        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public static bool Contains() => tree.Contains(F(X, G(C, B)));

        [Benchmark]
        public void GetInstances() => tree.GetInstances(F(X, C)).Consume(consumer);

        [Benchmark]
        public void GetGeneralisations() => tree.GetGeneralisations(F(B, G(C, B))).Consume(consumer);

        [Benchmark]
        public static bool GradualVarBinding_Contains() => tree_gradualVarBinding.TryGetExact(F(X, G(C, B)), out var _);

        [Benchmark]
        public void GradualVarBinding_GetInstances() => tree_gradualVarBinding.GetInstances(F(X, C)).Consume(consumer);

        [Benchmark]
        public void GradualVarBinding_GetGeneralisations() => tree_gradualVarBinding.GetGeneralisations(F(B, G(C, B))).Consume(consumer);

        [Benchmark]
        public static bool WOVarBinding_Contains() => tree_withoutVarBinding.TryGetExact(F(X, G(C, B)), out var _);

        [Benchmark]
        public void WOVarBinding_GetInstances() => tree_withoutVarBinding.GetInstances(F(X, C)).Consume(consumer);

        [Benchmark]
        public void WOVarBinding_GetGeneralisations() => tree_withoutVarBinding.GetGeneralisations(F(B, G(C, B))).Consume(consumer);
    }
}
