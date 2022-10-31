using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;

namespace SCFirstOrderLogic.Inference.ForwardChaining
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
        ////    return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        ////}

        [Benchmark]
        public static bool CrimeExample_ForwardChainingKnowledgeBase_FromAIaMA()
        {
            var kb = new ForwardChainingKnowledgeBase_FromAIaMA();
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        }
    }
}
