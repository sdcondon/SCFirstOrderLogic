using FluentAssertions;
using FlUnit;
using System;
using static SCFirstOrderLogic.FormulaCreation.OperableFormulaFactory;

namespace SCFirstOrderLogic.FormulaCreation;

public class OperableFormulaFactoryTests
{
    private record TestCase(OperableFormula FormulaSurrogate, Formula ExpectedFormula);

    private static OperableFunction Constant1 => new Function(nameof(Constant1));
    private static OperableFunction Constant2 => new Function(nameof(Constant2));
    private static OperablePredicate GroundPredicate1 => new Predicate(nameof(GroundPredicate1));
    private static OperablePredicate GroundPredicate2 => new Predicate(nameof(GroundPredicate2));
    private static OperablePredicate UnaryPredicate(OperableTerm term) => new Predicate(nameof(UnaryPredicate), term);
    private static OperableFunction UnaryFunction(OperableTerm term) => new Function(nameof(UnaryFunction), term);

    public static Test CreationAndCasting => TestThat
        .GivenEachOf<TestCase>(() =>
        [
            new(
                FormulaSurrogate: GroundPredicate1 & UnaryPredicate(Constant1),
                ExpectedFormula: new Conjunction(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(UnaryPredicate), new Function(nameof(Constant1))))),

            new(
                FormulaSurrogate: GroundPredicate1 | GroundPredicate2,
                ExpectedFormula: new Disjunction(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(GroundPredicate2), []))),

            new(
                FormulaSurrogate: Constant1 == Constant2,
                ExpectedFormula: new Predicate(
                    EqualityIdentifier.Instance,
                    new Function(nameof(Constant1)),
                    new Function(nameof(Constant2)))),

            new(
                FormulaSurrogate: Iff(GroundPredicate1, GroundPredicate2),
                ExpectedFormula: new Equivalence(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(GroundPredicate2), []))),

            new(
                FormulaSurrogate: ThereExists(X, UnaryFunction(X) == Constant1),
                ExpectedFormula: new ExistentialQuantification(
                    new VariableDeclaration("X"),
                    new Predicate(
                        EqualityIdentifier.Instance,
                        new Function(nameof(UnaryFunction), [ new VariableReference(new VariableDeclaration("X")) ]),
                        new Function(nameof(Constant1))))),

            new(
                FormulaSurrogate: If(GroundPredicate1, GroundPredicate2),
                ExpectedFormula: new Implication(
                    new Predicate(nameof(GroundPredicate1), []),
                    new Predicate(nameof(GroundPredicate2), []))),

            new(
                FormulaSurrogate: !GroundPredicate1,
                ExpectedFormula: new Negation(
                    new Predicate(nameof(GroundPredicate1), []))),

            new(
                FormulaSurrogate: ForAll(X, UnaryFunction(X) == Constant1),
                ExpectedFormula: new UniversalQuantification(
                    new VariableDeclaration("X"),
                    new Predicate(
                        EqualityIdentifier.Instance,
                        new Function(nameof(UnaryFunction), [ new VariableReference(new VariableDeclaration("X")) ]),
                        new Function(nameof(Constant1))))),
        ])
        .When(tc => (Formula)tc.FormulaSurrogate)
        .ThenReturns((tc, formula) =>
        {
            formula.Should().BeEquivalentTo(tc.ExpectedFormula, o => o.RespectingRuntimeTypes());
        });
}
