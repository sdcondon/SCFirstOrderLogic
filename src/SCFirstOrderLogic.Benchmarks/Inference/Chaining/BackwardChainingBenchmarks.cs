using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

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
            return kb.AskAsync(IsCriminal(West)).Result;
        }

        [Benchmark]
        public static bool CrimeExample_BackwardChainingKnowledgeBase_FromAIaMA()
        {
            var kb = new AltBackwardChainingKnowledgeBase_FromAIaMA();
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).Result;
        }
    }
}
