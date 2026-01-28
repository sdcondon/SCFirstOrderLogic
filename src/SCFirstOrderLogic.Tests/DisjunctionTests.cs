using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic;

public static class DisjunctionTests
{
    private static Predicate A => new(nameof(A));
    private static Predicate B => new(nameof(B));
    private static Predicate C => new(nameof(C));

    // Equality should have value semantics
    public static Test CloneComparison => TestThat
        .When(() => new
        {
            Representation1 = new Disjunction(A, B),
            Representation2 = new Disjunction(A, B)
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().Be(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeTrue())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeTrue());

    // Disjunction is commutative - so it would make sense if A ∨ B is considered equal to B ∨ A
    // And this would be relatively easy to achieve (order operands by hashcode then compare, for example).
    // However, By the same logic, disjunction formulas with different order of evaluation - e.g. (A ∨ B) ∨ C versus A ∨ (B ∨ C) - should also be considered equal.
    // This is however more difficult to achieve (without normalisation), so we don't (see below).
    // Given that, it probably doesn't make sense for commutations (only) to be considered equal - and doing so would be expensive. Hence the false expectation here.
    public static Test CommutationComparison => TestThat
        .When(() => new
        {
            Representation1 = new Disjunction(A, B),
            Representation2 = new Disjunction(B, A)
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().NotBe(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeFalse())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeFalse());

    // Difficult to do this without normalisation, so we don't.
    public static Test OrderChangeComparison => TestThat
        .When(() => new
        {
            Representation1 = new Disjunction(new Disjunction(A, B), C),
            Representation2 = new Disjunction(A, new Disjunction(B, C))
        })
        .ThenReturns()
        .And(g => g.Representation1.GetHashCode().Should().NotBe(g.Representation2.GetHashCode()))
        .And(g => g.Representation1.Equals(g.Representation2).Should().BeFalse())
        .And(g => g.Representation2.Equals(g.Representation1).Should().BeFalse());
}
