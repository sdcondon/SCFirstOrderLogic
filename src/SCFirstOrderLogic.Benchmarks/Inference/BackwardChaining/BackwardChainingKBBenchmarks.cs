using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using SCFirstOrderLogic.Inference;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;

namespace SCFirstOrderLogic.Inference.BackwardChaining
{
    [MemoryDiagnoser]
    [InProcess]
    public class BackwardChainingKBBenchmarks
    {
        [Benchmark(Baseline = true)]
        public static bool CrimeExample_SimpleBackwardChainingKnowledgeBase()
        {
            var kb = new BackwardChainingKnowledgeBase(new DictionaryClauseStore());
            kb.TellAsync(Axioms).Wait();
            return kb.AskAsync(IsCriminal(ColonelWest)).GetAwaiter().GetResult();
        }

        [Benchmark]
        public static bool CrimeExample_BackwardChainingKB_WithoutClauseStore()
        {
            var kb = new BackwardChainingKB_WithoutClauseStore();
            kb.TellAsync(Axioms).Wait();
            return kb.AskAsync(IsCriminal(ColonelWest)).GetAwaiter().GetResult();
        }

        [Benchmark]
        public static bool CrimeExample_BackwardChainingKB_FromAIaMA()
        {
            var kb = new BackwardChainingKB_FromAIaMA();
            kb.TellAsync(Axioms).Wait();
            return kb.AskAsync(IsCriminal(ColonelWest)).GetAwaiter().GetResult();
        }
    }
}
