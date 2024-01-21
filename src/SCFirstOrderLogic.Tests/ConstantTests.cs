using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class ConstantTests
{
    private static Constant A => new(nameof(A));

    public static Test IsGroundTermValue => TestThat
        .When(() => A.IsGroundTerm)
        .ThenReturns(isGroundTerm => isGroundTerm.Should().BeTrue());

    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = A,
            Representation2 = A
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
}
