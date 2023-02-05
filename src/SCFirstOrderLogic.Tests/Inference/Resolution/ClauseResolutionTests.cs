using FluentAssertions;
using FlUnit;
using System;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.Inference.Resolution
{
    public static partial class ClauseResolutionTests
    {
        private static Term C => new Constant(nameof(C));
        private static Term D => new Constant(nameof(D));
        private static OperablePredicate S(Term t) => new Predicate(nameof(S), t);
        private static OperablePredicate T(Term t) => new Predicate(nameof(T), t);
        private static OperablePredicate U(Term t) => new Predicate(nameof(U), t);
        private static OperablePredicate V(Term t, Term u) => new Predicate(nameof(V), t, u);

        private record TestCase(CNFClause Clause1, CNFClause Clause2, params CNFClause[] ExpectedResolvents);

        public static Test Resolution => TestThat
            .GivenEachOf<TestCase>(() => new TestCase[]
            {
                // Modus Ponens resolution with a constant
                new(
                    Clause1: new CNFClause(!S(C) | T(C)), // S(C) => T(C)
                    Clause2: new CNFClause(S(C)),
                    ExpectedResolvents: new CNFClause(T(C))),

                // Modus Ponens resolution on a globally quantified variable & a constant
                new(
                    Clause1: new CNFClause(!S(X) | T(X)), // ∀X, S(X) => T(X)
                    Clause2: new CNFClause(S(C)),
                    ExpectedResolvents: new CNFClause(T(C))), // {X/C}

                // Modus Ponens resolution on a constant & a globally quantified variable
                new(
                    Clause1: new CNFClause(!S(C) | T(C)), // S(C) => T(C)
                    Clause2: new CNFClause(S(X)),
                    ExpectedResolvents: new CNFClause(T(C))), // {X/C}

                // Modus Ponens resolution on a globally quantified variable
                new(
                    Clause1: new CNFClause(!S(X) | T(X)), // ∀X, S(X) => T(X)
                    Clause2: new CNFClause(S(Y)),
                    ExpectedResolvents: new CNFClause(T(Y))), // {Y/X} .. Or {X/Y}, giving T(X). Should really accept either..

                // More complicated - with a constant
                new(
                    Clause1: new CNFClause(!S(C) | T(C)), // ¬S(C) ∨ T(C)
                    Clause2: new CNFClause(S(C) | U(C)), // S(C) ∨ U(C)
                    ExpectedResolvents: new CNFClause(T(C) | U(C))),

                // Complementary unit clauses
                new(
                    Clause1: new CNFClause(S(C)),
                    Clause2: new CNFClause(!S(C)),
                    ExpectedResolvents: CNFClause.Empty),

                // Multiply-resolvable clauses
                // There's probably a better (more intuitive) human-language example, here
                new(
                    // S(D) ⇒ ¬T(X). In human, e.g.: "If SnowShoeHater is wearing snowshoes, no-one is wearing a T-shirt"
                    Clause1: new CNFClause(!S(D) | !T(X)), 
                    // ¬T(C) ⇒ S(Y). In human e.g.: "If TShirtLover is not wearing a T-shirt, everyone is wearing a snowshoes"
                    Clause2: new CNFClause(T(C) | S(Y)),
                    ExpectedResolvents: new[]
                    {
                        // {X/C} gives ∀Y, S(Y) ∨ ¬S(D) (that is, S(D) ⇒ S(Y)). If D is S, everything is. (If snowshoehater is wearing snowshoes, everyone is)
                        // NB: becomes obvious by forward chaining Clause1 to Clause2.
                        new CNFClause(S(Y) | !S(D)), 
                        // {Y/D} gives ∀X, T(C) ∨ ¬T(X) (that is, T(X) ⇒ T(C)). If anything is T, C is. (If anyone is wearing a T-shirt, TShirtLover is)
                        // NB: becomes obvious by forward chaining contrapositive of Clause1 to contrapositive of Clause2.
                        new CNFClause(T(C) | !T(X)),
                    }),

                // Variable chain (y=x/x=d) - ordering shouldn't matter
                new(
                    Clause1: new CNFClause(!V(Y, D) | !V(C, Y)), // e.g. ¬Equals(C, y) ∨ ¬Equals(D, y)
                    Clause2: new CNFClause(V(X, X)), // e.g. Equals(x, x)       
                    ExpectedResolvents: new[]
                    {
                        new CNFClause(!V(C, D)), // ¬Equals(C, D) 
                        new CNFClause(!V(C, D)), // ¬Equals(C, D) - don't mind that its returned twice. 
                    }),

                // Variable chain - ordering shouldn't matter
                ////new(
                ////    Clause1: new CNFClause(V(X, X)), // e.g. Equals(x, x)     
                ////    Clause2: new CNFClause(!V(Y, D) | !V(C, Y)), // e.g. ¬Equals(C, y) ∨ ¬Equals(D, y)
                ////    ExpectedResolvents: new[]
                ////    {
                ////        new CNFClause(!V(C, D)), // ¬Equals(C, D) 
                ////        new CNFClause(!V(C, D)), // ¬Equals(C, D) - don't mind that its returned twice. 
                ////    }),

                // Unresolvable - different predicates only
                new(
                    Clause1: new CNFClause(S(C)),
                    Clause2: new CNFClause(T(C)),
                    ExpectedResolvents: Array.Empty<CNFClause>()),

                // Unresolvable - Multiple trivially true resolvents
                new(
                    Clause1: new CNFClause(S(C) | !T(C)),
                    Clause2: new CNFClause(!S(C) | T(C)),
                    ExpectedResolvents: new CNFClause[]
                    {
                        // Both of these resolvents are trivially true - we expect them to not be returned
                        //new CNFClause(S(C) | !S(C)),
                        //new CNFClause(T(C) | !T(C))
                    }),

                // Unresolvable - Multiple trivially true resolvents (with variables..)
                new(
                    Clause1: new CNFClause(V(X, Y) | !V(Y, X)),
                    Clause2: new CNFClause(V(X, Y) | !V(Y, X)),
                    ExpectedResolvents: new CNFClause[]
                    {
                        // Both of these resolvents are trivially true - we expect them to not be returned
                        //new CNFClause(V(X, X) | !V(X, X)),
                        //new CNFClause(V(Y, Y) | !V(Y, Y))
                    }),
            })
            .When(g => ClauseResolution.Resolve(g.Clause1, g.Clause2))
            .ThenReturns(((g, r) => r.Select(u => u.Resolvent).Should().BeEquivalentTo(g.ExpectedResolvents)));
    }
}
