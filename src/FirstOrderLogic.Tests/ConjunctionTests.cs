using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic
{
    public static class ConjunctionTests
    {
        private static Predicate A => new (nameof(A));
        private static Predicate B => new (nameof(B));
        private static Predicate C => new (nameof(C));

        public static Test CloneComparison => TestThat
            .When(() => new
            {
                Representation1 = new Conjunction(A, B),
                Representation2 = new Conjunction(A, B)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());

        public static Test CommutationComparison => TestThat
            .When(() => new
            {
                Representation1 = new Conjunction(A, B),
                Representation2 = new Conjunction(B, A)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());

        // Difficult to do this without normalisation, so we don't.
        // Given this, is there any real value in accounting for commutations..?
        public static Test OrderChangeComparison => TestThat
            .When(() => new
            {
                Representation1 = new Conjunction(new Conjunction(A, B), C),
                Representation2 = new Conjunction(A, new Conjunction(B, C))
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().NotBe(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeFalse())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeFalse());
    }
}
