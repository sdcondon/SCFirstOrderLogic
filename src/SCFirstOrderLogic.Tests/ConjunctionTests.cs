using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic
{
    public static class ConjunctionTests
    {
        private static Predicate A => new(nameof(A));
        private static Predicate B => new(nameof(B));
        private static Predicate C => new(nameof(C));

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

        // Conjunction is commutative - so it would make sense if A ∧ B is considered equal to B ∧ A.
        // And this would be relatively easy to achieve (order operands by hashcode then compare, for example).
        // However, by the same logic, conjunction sentences with different order of evaluation - e.g. (A ∧ B) ∧ C versus A ∧ (B ∧ C) - should also be considered equal.
        // This is however more difficult to achieve (without normalisation), so we don't (see below).
        // Given that, it probably doesn't make sense for commutations (only) to be considered equal - and doing so would be expensive. Hence the false expectation here.
        public static Test CommutationComparison => TestThat
            .When(() => new
            {
                Representation1 = new Conjunction(A, B),
                Representation2 = new Conjunction(B, A)
            })
            .ThenReturns()
            .And(g => g.Representation1.GetHashCode().Should().NotBe(g.Representation2.GetHashCode()))
            .And(g => g.Representation1.Equals(g.Representation2).Should().BeFalse())
            .And(g => g.Representation2.Equals(g.Representation1).Should().BeFalse());

        // Difficult to do this without normalisation, so we don't.
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
