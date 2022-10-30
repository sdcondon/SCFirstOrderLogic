using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Chaining
{
    [MemoryDiagnoser]
    [InProcess]
    public class BackwardChainingBenchmarks
    {
        [Benchmark(Baseline = true)]
        public static bool CrimeExample_SimpleBackwardChainingKnowledgeBase()
        {
            var kb = new SimpleBackwardChainingKnowledgeBase();
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        }

        [Benchmark]
        public static bool CrimeExample_BackwardChainingKnowledgeBase_Simpler()
        {
            var kb = new BackwardChainingKnowledgeBase_Simpler();
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        }

        [Benchmark]
        public static bool CrimeExample_BackwardChainingKnowledgeBase_FromAIaMA()
        {
            var kb = new BackwardChainingKnowledgeBase_FromAIaMA();
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        }
    }
}
