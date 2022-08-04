﻿using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    [MemoryDiagnoser]
    [InProcess]
    public class ForwardChainingBenchmarks
    {
        ////[Benchmark(Baseline = true)]
        ////public static bool CrimeExample_SimpleForwardChainingKnowledgeBase()
        ////{
        ////    var kb = new SimpleForwardChainingKnowledgeBase();
        ////    kb.TellAsync(CrimeDomain.Axioms).Wait();
        ////    return kb.AskAsync(IsCriminal(West)).Result;
        ////}

        [Benchmark]
        public static bool CrimeExample_ForwardChainingKnowledgeBase_FromAIaMA()
        {
            var kb = new ForwardChainingKnowledgeBase_FromAIaMA();
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).Result;
        }
    }
}
