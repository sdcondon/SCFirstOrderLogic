using FluentAssertions;
using FluentAssertions.Equivalency;
using FlUnit;
using SCFirstOrderLogic.TestUtilities;
using static SCFirstOrderLogic.FormulaCreation.FormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

public static partial class CNFConversionTests
{
    private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
    private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

    private static Function F(Term term) => new("Skm:F", term);
    private static Function G(Term term) => new("Skm:G", term);
    private static VariableReference StdX => new("Std:X");

    // Incidentally, its really annoying when the principal example given in textbooks is wrong..
    public static Test BookExample => TestThat
        // Given ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
        .Given(() => ForAll(X, If(
            ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
            ThereExists(Y, Loves(Y, X)))))
        // When converted to CNF..
        .When(formula => CNFConversion.ApplyTo(formula))
        // Then gives [Animal(F(x)) ∨ Loves(G(x), x)] ∧ [¬Loves(x, F(x)) ∨ Loves(G(x), x)]
        .ThenReturns((_, formula) =>
        {
            formula.Should().BeEquivalentTo(
                expectation: And(
                    Or(IsAnimal(F(StdX)), Loves(G(StdX), StdX)),
                    Or(Not(Loves(StdX, F(StdX))), Loves(G(StdX), StdX))),
                config: EquivalencyOptions.UsingOnlyConsistencyForVariablesAndSkolemFunctions);
        });

    private static EquivalencyAssertionOptions<Formula> CNFEquivalencyOpts(EquivalencyAssertionOptions<Formula> opts)
    {
        // We don't particularly care about the details (i.e. identifiers) of the
        // actual skolem functions and standardised variables, but they should
        // be the "same" everywhere we expect them to be
        // (and distinct from everything else - but we don't test that, yet anyway).
        Function? fActual = null;
        Function? gActual = null;
        VariableReference? stdXActual = null;

        static void ShouldBeConsistentWith<T>(IAssertionContext<T> ctx, ref T? actual)
        {
            if (actual != null)
            {
                ctx.Subject.Should().Be(actual);
            }
            else
            {
                actual = ctx.Subject;
            }
        }

        return opts
            .RespectingRuntimeTypes()
            .ComparingByMembers<Formula>()
            .Using<Function>(ctx =>
            {
                if (ctx.Expectation.Identifier.Equals("Skm:F"))
                    ShouldBeConsistentWith(ctx, ref fActual);
                else if (ctx.Expectation.Identifier.Equals("Skm:G"))
                    ShouldBeConsistentWith(ctx, ref gActual);
                else
                    ctx.Subject.Should().BeEquivalentTo(ctx.Expectation);
            })
            .WhenTypeIs<Function>()
            .Using<VariableReference>(ctx =>
            {
                if (ctx.Expectation.Declaration.Identifier.Equals("Std:X"))
                    ShouldBeConsistentWith(ctx, ref stdXActual);
                else
                    ctx.Subject.Should().BeEquivalentTo(ctx.Expectation);
            })
            .WhenTypeIs<VariableReference>()
            .WithTracing();
    }
}
