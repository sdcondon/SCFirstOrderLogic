using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.FromAIaMA.Chapter9.UsingSentenceFactory.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Resolution
{
    [MemoryDiagnoser]
    [InProcess]
    public class ResolutionBenchmarks
    {
        [Benchmark(Baseline = true)]
        public static bool CrimeExample_SimpleResolutionKnowledgeBase()
        {
            var kb = new SimpleResolutionKnowledgeBase(new HashSetClauseStore(), SimpleResolutionKnowledgeBase.Filters.None, SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        }

        [Benchmark]
        public static bool CrimeExample_SimplerResolutionKB_WithoutClauseStore()
        {
            var kb = new ResolutionKB_WithoutClauseStore(ResolutionKB_WithoutClauseStore.Filters.None, ResolutionKB_WithoutClauseStore.PriorityComparisons.UnitPreference);
            foreach (var axiom in CrimeDomain.Axioms)
            {
                kb.Tell(axiom);
            }
            return kb.Ask(IsCriminal(West));
        }
    }
}
