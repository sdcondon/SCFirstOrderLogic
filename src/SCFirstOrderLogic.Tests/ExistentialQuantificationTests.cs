using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class ExistentialQuantificationTests
{
    private static VariableDeclaration X => new(nameof(X));
    private static Predicate F(Term t) => new(nameof(F), t);

    // Equality should have value semantics
    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = new ExistentialQuantification(X, F(X)),
            Representation2 = new ExistentialQuantification(X, F(X))
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
}
