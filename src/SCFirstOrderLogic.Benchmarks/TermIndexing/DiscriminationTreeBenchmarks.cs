using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.TermIndexing
{
    [MemoryDiagnoser]
    [InProcess]
    public class DiscriminationTreeBenchmarks
    {
        private static readonly Constant A = new("A");
        private static readonly Constant B = new("B");
        private static readonly Constant C = new("C");
        private static readonly Constant D = new("D");

        private static Function F(Term x, Term y) => new(nameof(F), x, y);
        private static Function G(Term x, Term y) => new(nameof(G), x, y);
        private static Function H(Term x) => new(nameof(H), x);

        // Example from https://rg1-teaching.mpi-inf.mpg.de/autrea-ws19/script-6.2-7.4.pdf
        private static readonly DiscriminationTree tree = new DiscriminationTree(new[]
        {
            F(G(D, X), C),
            G(B, H(C)),
            F(G(X, C), C),
            F(B, G(C, B)),
            F(B, G(X, B)),
            F(X, C),
            F(X, G(C, B))
        });

        private readonly Consumer consumer = new Consumer();

        [Benchmark]
        public static bool Contains()
        {
            return tree.Contains(F(X, G(C, B)));
        }

        [Benchmark]
        public void GetInstances()
        {
            tree.GetInstances(F(X, C)).Consume(consumer);
        }

        [Benchmark]
        public void GetGeneralisations()
        {
            tree.GetGeneralisations(F(B, G(C, B))).Consume(consumer);
        }
    }
}
