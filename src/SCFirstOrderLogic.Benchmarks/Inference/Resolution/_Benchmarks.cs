using BenchmarkDotNet.Attributes;
using SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9.CrimeDomain;

namespace SCFirstOrderLogic.Inference.Resolution
{
    [MemoryDiagnoser]
    [InProcess]
    public class _Benchmarks
    {
        [Benchmark]
        public static bool CrimeExample_SimplestResolutionKnowledgeBase()
        {
            var kb = new SimplestResolutionKnowledgeBase(ClausePairFilters.None, ClausePairPriorityComparers.UnitPreference);
            kb.Tell(CrimeDomain.Axioms);
            return kb.Ask(IsCriminal(West));
        }
    }
}
