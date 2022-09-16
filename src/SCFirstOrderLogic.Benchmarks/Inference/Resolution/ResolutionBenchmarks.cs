using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.SentenceFactory;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.SentenceFactory.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Resolution
{
    [MemoryDiagnoser]
    [InProcess]
    public class ResolutionBenchmarks
    {
        [Benchmark(Baseline = true)]
        public static bool CrimeExample_SimpleResolutionKnowledgeBase()
        {
            var kb = new SimpleResolutionKnowledgeBase(new SimpleClauseStore(), SimpleResolutionKnowledgeBase.Filters.None, SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).GetAwaiter().GetResult();
        }

        [Benchmark]
        public static bool CrimeExample_SimplerResolutionKnowledgeBase()
        {
            var kb = new ResolutionKnowledgeBase_Simpler(ResolutionKnowledgeBase_Simpler.Filters.None, ResolutionKnowledgeBase_Simpler.PriorityComparisons.UnitPreference);
            foreach (var axiom in CrimeDomain.Axioms)
            {
                kb.Tell(axiom);
            }
            return kb.Ask(IsCriminal(West));
        }
    }
}
