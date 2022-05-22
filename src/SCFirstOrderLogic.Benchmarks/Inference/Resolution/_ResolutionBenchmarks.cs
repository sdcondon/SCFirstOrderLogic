using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Resolution
{
    [MemoryDiagnoser]
    [InProcess]
    public class _ResolutionBenchmarks
    {
        [Benchmark]
        public static bool CrimeExample_SimplestResolutionKnowledgeBase()
        {
            var kb = new SimplerResolutionKnowledgeBase(ClausePairFilters.None, ClausePairPriorityComparisons.UnitPreference);
            foreach (var axiom in CrimeDomain.Axioms)
            {
                kb.Tell(axiom);
            }
            return kb.Ask(IsCriminal(West));
        }

        [Benchmark]
        public static bool CrimeExample_SimpleResolutionKnowledgeBase()
        {
            var kb = new SimpleResolutionKnowledgeBase(new ListClauseStore(), ClausePairFilters.None, ClausePairPriorityComparisons.UnitPreference);
            kb.TellAsync(CrimeDomain.Axioms).Wait();
            return kb.AskAsync(IsCriminal(West)).Result;
        }
    }
}
