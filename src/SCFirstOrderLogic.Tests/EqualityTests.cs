using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic
{
    public static class EqualityTests
    {
        private static Constant A => new (nameof(A));
        private static Constant B => new(nameof(B));

        public static Test CloneComparison => TestThat
            .When(() => new
            {
                Representation1 = new Equality(A, B),
                Representation2 = new Equality(A, B)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());

        public static Test CommutationComparison => TestThat
            .When(() => new
            {
                Representation1 = new Equality(A, B),
                Representation2 = new Equality(B, A)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());
    }
}
