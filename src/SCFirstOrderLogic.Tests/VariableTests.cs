using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class VariableReferenceTests
{
    public static Test IsGroundTermValue => TestThat
        .When(() => new VariableReference("X").IsGroundTerm)
        .ThenReturns(isGroundTerm => isGroundTerm.Should().Be(false));

    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = new VariableReference("X"),
            Representation2 = new VariableReference("X")
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
}
