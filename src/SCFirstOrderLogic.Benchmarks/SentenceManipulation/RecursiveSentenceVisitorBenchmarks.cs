using BenchmarkDotNet.Attributes;
using System.Collections.Generic;
using System;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;
using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic.Benchmarks.SentenceManipulation
{
    [MemoryDiagnoser]
    [InProcess]
    public class RecursiveSentenceVisitorBenchmarks
    {
        private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
        private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

        private static Sentence NonTrivialSentence { get; } = ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X))));

        public record TestCase(string Label, bool DoSomething, Sentence Sentence)
        {
            public override string ToString() => Label;
        }

        public static IEnumerable<TestCase> TestCases { get; } = new TestCase[]
        {
            new(
                Label: "Non-Trivial NO-OP",
                DoSomething: false,
                Sentence: NonTrivialSentence),

            new(
                Label: "Non-Trivial ALL-LEAFS-OP",
                DoSomething: true,
                Sentence: NonTrivialSentence),
        };

        [ParamsSource(nameof(TestCases))]
        public TestCase? CurrentTestCase { get; set; }

        [Benchmark(Baseline = true)]
        public void CurrentImpl() => new NullVisitor(CurrentTestCase!.DoSomething).Visit(CurrentTestCase!.Sentence);

        [Benchmark]
        public void Enumerators() => new NullVisitor_Enumerators(CurrentTestCase!.DoSomething).Visit(CurrentTestCase!.Sentence);

        private class NullVisitor : RecursiveSentenceVisitor
        {
            private readonly bool doSomething;

            public NullVisitor(bool doSomething) => this.doSomething = doSomething;
        }

        private class NullVisitor_Enumerators : RecursiveSentenceVisitor_Enumerators
        {
            private readonly bool doSomething;

            public NullVisitor_Enumerators(bool doSomething) => this.doSomething = doSomething;
        }
    }
}
