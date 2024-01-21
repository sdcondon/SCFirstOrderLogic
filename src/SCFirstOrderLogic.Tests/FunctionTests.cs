using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class FunctionTests
{
    private static Constant C => new(nameof(C));
    private static VariableReference X => new(nameof(X));
    private static Function F(params Term[] t) => new(nameof(F), t);

    public static Test IsGroundTermValue => TestThat
        .GivenEachOf(() => new[]
        {
            new { Function = F(), IsGroundTermExpectation = true },
            new { Function = F(C), IsGroundTermExpectation = true },
            new { Function = F(F(C)), IsGroundTermExpectation = true },
            new { Function = F(X), IsGroundTermExpectation = false },
            new { Function = F(F(X)), IsGroundTermExpectation = false },
        })
        .When(tc => tc.Function.IsGroundTerm)
        .ThenReturns((tc, isGroundTerm) => isGroundTerm.Should().Be(tc.IsGroundTermExpectation));

    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = new Function("F", F(X)),
            Representation2 = new Function("F", F(X))
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
}
