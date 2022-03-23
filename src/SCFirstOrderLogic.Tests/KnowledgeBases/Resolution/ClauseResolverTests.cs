using FluentAssertions;
using FlUnit;
using SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm;
using System;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.KnowledgeBases.Resolution
{
    public static partial class ClauseResolverTests
    {
        private static Term C => new Constant(nameof(C));
        private static Term D => new Constant(nameof(D));
        private static Sentence S(Term t) => new Predicate(nameof(S), new[] { t });
        private static Sentence T(Term t) => new Predicate(nameof(T), new[] { t });
        private static Sentence U(Term t) => new Predicate(nameof(U), new[] { t });

        private record TestCase(CNFClause Clause1, CNFClause Clause2, params CNFClause[] ExpectedResolvents);

        public static Test ResolutionOfResolvableClauses => TestThat
            .GivenEachOf(() => new TestCase[]
            {
                // Modus Ponens resolution with a constant
                new(
                    Clause1: new CNFClause(Or(Not(S(C)), T(C))), // S(C) => T(C)
                    Clause2: new CNFClause(S(C)),
                    ExpectedResolvents: new CNFClause(T(C))),

                // Modus Ponens resolution on a globally quantified variable & a constant
                new(
                    Clause1: new CNFClause(Or(Not(S(X)), T(X))), // ∀X, S(X) => T(X)
                    Clause2: new CNFClause(S(C)),
                    ExpectedResolvents: new CNFClause(T(C))), // {X/C}

                // Modus Ponens resolution on a constant & a globally quantified variable
                new(
                    Clause1: new CNFClause(Or(Not(S(C)), T(C))), // S(C) => T(C)
                    Clause2: new CNFClause(S(X)),
                    ExpectedResolvents: new CNFClause(T(C))), // {X/C}

                // Modus Ponens resolution on a globally quantified variable
                new(
                    Clause1: new CNFClause(Or(Not(S(X)), T(X))), // ∀X, S(X) => T(X)
                    Clause2: new CNFClause(S(Y)),
                    ExpectedResolvents: new CNFClause(T(X))), // {X/Y} .. Or {Y/X}

                // More complicated - with a constant
                new(
                    Clause1: new CNFClause(Or(Not(S(C)), T(C))), // ¬S(C) ∨ T(C)
                    Clause2: new CNFClause(Or(S(C), U(C))), // S(C) ∨ U(C)
                    ExpectedResolvents: new CNFClause(Or(T(C), U(C)))),

                // Complementary unit clauses
                new(
                    Clause1: new CNFClause(S(C)),
                    Clause2: new CNFClause(Not(S(C))),
                    ExpectedResolvents: CNFClause.Empty),

                // Unresolvable clauses
                // TODO: Probably more test cases where it trivially looks like there should be resolvents, but no resolver will work. E.g. (but prob others too) occurs check failure.
                new(
                    Clause1: new CNFClause(S(C)),
                    Clause2: new CNFClause(T(C)),
                    ExpectedResolvents: Array.Empty<CNFClause>()),

                // Multiply-resolvable clauses
                // There's probably a better (more intuitive) human-language example, here
                new(
                    // S(D) ⇒ ¬T(X). In human, e.g.: "If SnowShoeHater is wearing snowshoes, no-one is wearing a T-shirt"
                    Clause1: new CNFClause(Or(Not(S(D)), Not(T(X)))), 
                    // ¬T(C) ⇒ S(Y). In human e.g.: "If TShirtLover is not wearing a T-shirt, everyone is wearing a snowshoes"
                    Clause2: new CNFClause(Or(T(C), S(Y))),
                    ExpectedResolvents: new[]
                    {
                        // {X/C} gives ∀Y, S(Y) ∨ ¬S(D) (that is, S(D) ⇒ S(Y)). If D is S, everything is. (If snowshoehater is wearing snowshoes, everyone is)
                        // NB: becomes obvious by forward chaining Clause1 to Clause2.
                        new CNFClause(Or(S(Y), Not(S(D)))), 
                        // {Y/D} gives ∀X, T(C) ∨ ¬T(X) (that is, T(X) ⇒ T(C)). If anything is T, C is. (If anyone is wearing a T-shirt, TShirtLover is)
                        // NB: becomes obvious by forward chaining contrapositive of Clause1 to contrapositive of Clause2.
                        new CNFClause(Or(T(C), Not(T(X)))), 
                    }),

                // Multiple trivially true resolvents
                new(
                    Clause1: new CNFClause(Or(S(C), Not(T(C)))),
                    Clause2: new CNFClause(Or(Not(S(C)), T(C))),
                    ExpectedResolvents: new[]
                    {
                        // Both of these resolvents are trivially true - so largely useless - TODO: the expectation should probably be no resolvents in this case?
                        new CNFClause(Or(S(C), Not(S(C)))),
                        new CNFClause(Or(T(C), Not(T(C))))
                    }),
            })
            .When(g => ClauseResolver.Resolve(g.Clause1, g.Clause2))
            .ThenReturns((g, r) => r.Should().BeEquivalentTo(g.ExpectedResolvents));
    }
}
