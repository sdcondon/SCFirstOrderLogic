using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class PredicateTests
{
    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = new Predicate("A"),
            Representation2 = new Predicate("A"),
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
}
