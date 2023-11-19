using FluentAssertions;
using FlUnit;
using System;
using System.Collections.Generic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using ConstantInfo = SCFirstOrderLogic.TermIndexing.DiscriminationTreeConstantInfo;
using FunctionInfo = SCFirstOrderLogic.TermIndexing.DiscriminationTreeFunctionInfo;
using IElementInfo = SCFirstOrderLogic.TermIndexing.IDiscriminationTreeElementInfo;
using VariableInfo = SCFirstOrderLogic.TermIndexing.DiscriminationTreeVariableInfo;

namespace SCFirstOrderLogic.TermIndexing
{
    public static class DiscriminationTreeTests
    {
        private static readonly Constant C1 = new(nameof(C1));
        private static readonly Constant C2 = new(nameof(C2));

        private static Function F(params Term[] a) => new(nameof(F), a);

        public static Test AddBehaviour_Positive => TestThat
            .GivenEachOf(() => new PositiveAddTestCase[]
            {
                new(
                    CurrentTerms: Array.Empty<Term>(),
                    NewTerm: C1,
                    ExpectedRootChildren: new()
                    {
                        [new ConstantInfo("C1")] = new { Value = C1 },
                    }),

                new(
                    CurrentTerms: new[] { F(X) },
                    NewTerm: F(C1),
                    ExpectedRootChildren: new()
                    {
                        [new FunctionInfo("F", 1)] = new
                        {
                            Children = new Dictionary<IElementInfo, object>
                            {
                                [new VariableInfo(0)] = new { Value = F(X) },
                                [new ConstantInfo("C1")] = new { Value = F(C1) },
                            }
                        },
                    }),

                new(
                    CurrentTerms: new[] { F(X, C2) },
                    NewTerm: F(X, C1),
                    ExpectedRootChildren: new()
                    {
                        [new FunctionInfo("F", 2)] = new
                        {
                            Children = new Dictionary<IElementInfo, object>
                            {
                                [new VariableInfo(0)] = new
                                { 
                                    Children = new Dictionary<IElementInfo, object>
                                    {
                                        [new ConstantInfo("C1")] = new { Value = F(X, C1) },
                                        [new ConstantInfo("C2")] = new { Value = F(X, C2) },
                                    }
                                },
                            }
                        }
                    }),

                new(
                    CurrentTerms: Array.Empty<Term>(),
                    NewTerm: F(F(C1), F(C2)),
                    ExpectedRootChildren: new()
                    {
                        [new FunctionInfo("F", 2)] = new
                        {
                            Children = new Dictionary<IElementInfo, object>
                            {
                                [new FunctionInfo("F", 1)] = new
                                {
                                    Children = new Dictionary<IElementInfo, object>
                                    {
                                        [new ConstantInfo("C1")] = new
                                        {
                                            Children = new Dictionary<IElementInfo, object>
                                            {
                                                [new FunctionInfo("F", 1)] = new
                                                {
                                                    Children = new Dictionary<IElementInfo, object>
                                                    {
                                                        [new ConstantInfo("C2")] = new { Value = F(F(C1), F(C2)) },
                                                    }
                                                },
                                            }
                                        },
                                    }
                                },
                            }
                        }
                    }),

                // Same function identifier with different arg count shouldn't cause problems:
                new( 
                    CurrentTerms: new[] { F() },
                    NewTerm: F(C1),
                    ExpectedRootChildren: new()
                    {
                        [new FunctionInfo("F", 0)] = new
                        {
                            Value = new Function("F")
                        },
                        [new FunctionInfo("F", 1)] = new
                        {
                            Children = new Dictionary<IElementInfo, object>
                            {
                                [new ConstantInfo("C1")] = new { Value = new Function("F", C1) }
                            }
                        },
                    }),
            })
            .When(tc =>
            {
                var root = new DiscriminationTreeDictionaryNode<Term>();
                var tree = new DiscriminationTree(root, tc.CurrentTerms);
                tree.Add(tc.NewTerm);
                return root.Children;
            })
            .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedRootChildren));

        public static Test AddBehaviour_Negative => TestThat
            .GivenEachOf(() => new NegativeAddTestCase[]
            {
                new(
                    CurrentTerms: new[] { C1 },
                    NewTerm: C1),

                new(
                    CurrentTerms: new[] { F(X) },
                    NewTerm: F(X)),

                new(
                    CurrentTerms: new[] { F(C1) },
                    NewTerm: F(C1)),
            })
            .When(tc =>
            {
                var tree = new DiscriminationTree(tc.CurrentTerms);
                tree.Add(tc.NewTerm);
            })
            .ThenThrows();

        public static Test ContainsBehaviour => TestThat
            .GivenEachOf(() => new ContainsTestCase[]
            {
                new(
                    StoredTerms: new Term[] { C1, C2, X },
                    QueryTerm: C1,
                    ExpectedReturnValue: true),

                new(
                    StoredTerms: new Term[] { C1, C2, X },
                    QueryTerm: X,
                    ExpectedReturnValue: true),

                new( // variable identifier shouldn't matter
                    StoredTerms: new Term[] { C1, C2, X },
                    QueryTerm: Y,
                    ExpectedReturnValue: true),

                new( // variable identifier shouldn't matter #2
                    StoredTerms: new Term[] { F(X, Y) },
                    QueryTerm: F(Y, X),
                    ExpectedReturnValue: true),

                new( // variable ordinal should matter
                    StoredTerms: new Term[] { F(X, Y) },
                    QueryTerm: F(X, X),
                    ExpectedReturnValue: false),

                new(
                    StoredTerms: new Term[] { F(X), F(C2) },
                    QueryTerm: F(C1),
                    ExpectedReturnValue: false),

                new(
                    StoredTerms: new Term[] { F(C1, C1), F(C2, C2), F(C1, C2) },
                    QueryTerm: F(X, X),
                    ExpectedReturnValue: false),

                new(
                    StoredTerms: new Term[] { F(X, C2) },
                    QueryTerm: F(C1, Y),
                    ExpectedReturnValue: false),
            })
            .When(tc =>
            {
                var tree = new DiscriminationTree(tc.StoredTerms);
                return tree.Contains(tc.QueryTerm);
            })
            .ThenReturns()
            .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

        public static Test GetInstancesBehaviour => TestThat
            .GivenEachOf<GetTestCase>(() => new GetTestCase[]
            {
                new( // Exact match
                    StoredTerms: new Term[] { C1, C2, X },
                    QueryTerm: C1,
                    ExpectedReturnValue: new Term[] { C1 }),

                new( // Get everything
                    StoredTerms: new Term[] { C1, X, F(X), F(F(X, C1)) },
                    QueryTerm: Y,
                    ExpectedReturnValue: new Term[] { C1, X, F(X), F(F(X, C1)) }),

                new( // Get all instances of top-level function
                    StoredTerms: new Term[] { F(C1), F(C2), F(F(C1)), F(C1, C2), C1 },
                    QueryTerm: F(X),
                    ExpectedReturnValue: new Term[] { F(C1), F(C2), F(F(C1)) }),

                new( // Get all instances of top-level function with any args
                    StoredTerms: new Term[] { F(C1, C1), F(C2, C2), F(C1, C2) },
                    QueryTerm: F(X, Y),
                    ExpectedReturnValue: new Term[] { F(C1, C1), F(C2, C2), F(C1, C2) }),

                new( // Get all instances of top-level function with repeated arg
                    StoredTerms: new Term[] { F(C1, C1), F(C1, C2), F(F(X), F(X)), F(F(X), F(Y)) },
                    QueryTerm: F(X, X),
                    ExpectedReturnValue: new Term[] { F(C1, C1), F(F(X), F(X)) }),

                new( // Don't return term if it's only unifiable
                    StoredTerms: new Term[] { F(X, C2) },
                    QueryTerm: F(C1, Y),
                    ExpectedReturnValue: new Term[] { }),
            })
            .When(tc =>
            {
                var tree = new DiscriminationTree(tc.StoredTerms);
                return tree.GetInstances(tc.QueryTerm);
            })
            .ThenReturns()
            .And((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedReturnValue));

        public static Test GetGeneralisationsBehaviour => TestThat
            .GivenEachOf<GetTestCase>(() => new GetTestCase[]
            {
                new(
                    StoredTerms: new Term[] { C1, C2, X },
                    QueryTerm: C1,
                    ExpectedReturnValue: new Term[] { C1, X }),

                new(
                    StoredTerms: new Term[] { F(X), F(C1, C2) },
                    QueryTerm: F(C1),
                    ExpectedReturnValue: new Term[] { F(X) }),

                new(
                    StoredTerms: new Term[] { F(X), F(C1, C2) },
                    QueryTerm: F(Y),
                    ExpectedReturnValue: new Term[] { F(X) }),

                new(
                    StoredTerms: new Term[] { F(X), F(C1), F(F(X)), F(C1, C2) },
                    QueryTerm: F(F(C1)),
                    ExpectedReturnValue: new Term[] { F(X), F(F(X)) }),

                new(
                    StoredTerms: new Term[] { F(X, X), F(X, Y) },
                    QueryTerm: F(C1, C2),
                    ExpectedReturnValue: new Term[] { F(X, Y) }),

                new(
                    StoredTerms: new Term[] { F(X, X), F(X, Y) },
                    QueryTerm: F(C1, C1),
                    ExpectedReturnValue: new Term[] { F(X, X), F(X, Y) }),

                new( // Don't return term if it's "merely" unifiable
                    StoredTerms: new Term[] { F(X, C2) },
                    QueryTerm: F(C1, Y),
                    ExpectedReturnValue: new Term[] { }),
            })
            .When(tc =>
            {
                var tree = new DiscriminationTree(tc.StoredTerms);
                return tree.GetGeneralisations(tc.QueryTerm);
            })
            .ThenReturns()
            .And((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedReturnValue));

        private record PositiveAddTestCase(Term[] CurrentTerms, Term NewTerm, Dictionary<IElementInfo, object> ExpectedRootChildren);

        private record NegativeAddTestCase(Term[] CurrentTerms, Term NewTerm);

        private record ContainsTestCase(Term[] StoredTerms, Term QueryTerm, bool ExpectedReturnValue);

        private record GetTestCase(Term[] StoredTerms, Term QueryTerm, Term[] ExpectedReturnValue);
    }
}
