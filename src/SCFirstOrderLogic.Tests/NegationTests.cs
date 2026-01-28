using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class NegationTests
{
    private static Predicate A => new(nameof(A));

    // Equality should have value semantics
    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = new Negation(A),
            Representation2 = new Negation(A),
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
}
