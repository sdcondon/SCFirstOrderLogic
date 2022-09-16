using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.SentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.SentenceFactory.CrimeDomain;

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
