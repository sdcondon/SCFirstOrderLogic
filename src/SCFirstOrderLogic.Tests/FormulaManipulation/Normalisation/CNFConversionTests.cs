using FluentAssertions;
using FlUnit;
using static SCFirstOrderLogic.FormulaCreation.Specialised.GenericDomainFormulaFactory;

namespace SCFirstOrderLogic.FormulaManipulation.Normalisation;

public static partial class CNFConversionTests
{
    public static Test VariablesStandardisedAcrossFormulas => TestThat
        .Given(() => new
        {
            // NB these normalise to just P(X) and Q(X) respectively
            CNFFormula1 = CNFConversion.ApplyTo(ForAll(X, P(X))),
            CNFFormula2 = CNFConversion.ApplyTo(ForAll(X, Q(X)))
        })
        .When(g => ((Predicate)g.CNFFormula1).Arguments[0].Equals(((Predicate)g.CNFFormula2).Arguments[0]))
        .ThenReturns((_, retVal) => retVal.Should().BeFalse("standardised variables from different formulas shouldn't be equal, even if the underlying identifier is the same"));

    // These behaviours might be nice, but we don't do them for now at least:
    ////public static Test NormalisationOfEquivalentFormulas => TestThat
    ////    .GivenEachOf(() => new[]
    ////    {
    ////        new
    ////        {
    ////            // Order of operation for disjunctions doesn't matter
    ////            Formula1 = Or(Or(P(), Q()), R()),
    ////            Formula2 = Or(P(), Or(Q(), R()))
    ////        },
    ////        new
    ////        {
    ////            // More difficult, but MAYBE still useful - variable naming doesn't matter
    ////            Formula1 = ForAll(X, P(X)),
    ////            Formula2 = ForAll(Y, P(Y))
    ////        }
    ////    })
    ////    .When(g => new
    ////    {
    ////        CNFFormula1 = new CNFConversion().ApplyTo(g.Formula1),
    ////        CNFFormula2 = new CNFConversion().ApplyTo(g.Formula2)
    ////    })
    ////    .ThenReturns()
    ////    .And((_, retVal) => retVal.CNFFormula1.Should().Be(retVal.CNFFormula2));
}
