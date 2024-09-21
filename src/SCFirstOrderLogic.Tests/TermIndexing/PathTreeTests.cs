using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using static SCFirstOrderLogic.TestProblems.GenericDomainOperableSentenceFactory;

namespace SCFirstOrderLogic.TermIndexing;

public static class PathTreeTests
{
    // Path trees are a well-known data structure - so I'm asserting that asserting on the internal structure is valid. Probably.
    public static Test AddBehaviour_Positive => TestThat
        .GivenEachOf<PositiveAddTestCase>(() =>
        [
            new(
                CurrentTerms: [],
                NewTerm: C,
                ExpectedRootChildren: new()
                {
                    [new PathTreeFunctionNodeKey("C", 0)] = new
                    {
                        Values = new[] { KeyValuePair.Create(C, C) }
                    },
                }),

            new(
                CurrentTerms: [F(X)],
                NewTerm: F(C),
                ExpectedRootChildren: new()
                {
                    [new PathTreeFunctionNodeKey("F", 1)] = new
                    {
                        Children = new object[]
                        {
                            new
                            {
                                Children = new Dictionary<IPathTreeArgumentNodeKey, object>
                                {
                                    [new PathTreeVariableNodeKey(0)] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(Var(0)), F(X)) }
                                    },
                                    [new PathTreeFunctionNodeKey("C", 0)] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(C), F(C)) }
                                    },
                                }
                            },
                        }
                    },
                }),

            new(
                CurrentTerms: [F(X, C)],
                NewTerm: F(Y, D),
                ExpectedRootChildren: new()
                {
                    [new PathTreeFunctionNodeKey("F", 2)] = new
                    {
                        Children = new object[]
                        {
                            new
                            {
                                Children = new Dictionary<IPathTreeArgumentNodeKey, object>
                                {
                                    [new PathTreeVariableNodeKey(0)] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(Var(0), C), F(X, C)), KeyValuePair.Create(F(Var(0), D), F(Y, D)) }
                                    },
                                }
                            },
                            new
                            {
                                Children = new Dictionary<IPathTreeArgumentNodeKey, object>
                                {
                                    [new PathTreeFunctionNodeKey("C", 0)] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(Var(0), C), F(X, C)) }
                                    },
                                    [new PathTreeFunctionNodeKey("D", 0)] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(Var(0), D), F(Y, D)) }
                                    },
                                }
                            },
                        }
                    }
                }),

            // Same function identifier with different arg count shouldn't cause problems:
            new(
                CurrentTerms: [F()],
                NewTerm: F(C),
                ExpectedRootChildren: new()
                {
                    [new PathTreeFunctionNodeKey("F", 0)] = new
                    {
                        Values = new[] { KeyValuePair.Create(new Function("F"), new Function("F")) }
                    },
                    [new PathTreeFunctionNodeKey("F", 1)] = new
                    {
                        Children = new object[]
                        {
                            new
                            {
                                Children = new Dictionary<IPathTreeArgumentNodeKey, object>
                                {
                                    [new PathTreeFunctionNodeKey("C", 0)] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(new Function("F", C), new Function("F", C)) }
                                    }
                                }
                            }
                        }
                    },
                }),
        ])
        .When(tc =>
        {
            var rootNode = new PathTreeDictionaryNode<Term>();
            var tree = new PathTree(rootNode, tc.CurrentTerms);
            tree.Add(tc.NewTerm);
            return rootNode.Children;
        })
        .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedRootChildren));

    public static Test AddBehaviour_Negative => TestThat
        .GivenEachOf<NegativeAddTestCase>(() =>
        [
            new(
                CurrentTerms: [C],
                NewTerm: C),

            new(
                CurrentTerms: [F(X)],
                NewTerm: F(X)),

            new(
                CurrentTerms: [F(C)],
                NewTerm: F(C)),
        ])
        .When(tc =>
        {
            var tree = new PathTree(tc.CurrentTerms);
            tree.Add(tc.NewTerm);
        })
        .ThenThrows();

    public static Test ContainsBehaviour => TestThat
        .GivenEachOf<ContainsTestCase>(() =>
        [
            new(
                StoredTerms: [C, D, X],
                QueryTerm: C,
                ExpectedReturnValue: true),

            new(
                StoredTerms: [C, D, X],
                QueryTerm: X,
                ExpectedReturnValue: true),

            new( // variable identifier shouldn't matter
                StoredTerms: [C, D, X],
                QueryTerm: Y,
                ExpectedReturnValue: true),

            new( // variable identifier shouldn't matter #2
                StoredTerms: [F(X, Y)],
                QueryTerm: F(Y, X),
                ExpectedReturnValue: true),

            new( // variable ordinal should matter
                StoredTerms: [F(X, Y)],
                QueryTerm: F(X, X),
                ExpectedReturnValue: false),

            new(
                StoredTerms: [F(X), F(D)],
                QueryTerm: F(C),
                ExpectedReturnValue: false),

            new(
                StoredTerms: [F(C, C), F(D, D), F(C, D)],
                QueryTerm: F(X, X),
                ExpectedReturnValue: false),

            new(
                StoredTerms: [F(X, D)],
                QueryTerm: F(C, Y),
                ExpectedReturnValue: false),
        ])
        .When(tc =>
        {
            var tree = new PathTree(tc.StoredTerms);
            return tree.Contains(tc.QueryTerm);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    public static Test GetInstancesBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new( // Exact match
                StoredTerms: [C, D, X],
                QueryTerm: C,
                ExpectedReturnValue: [C]),

            new( // Get everything
                StoredTerms: [C, X, F(X), F(F(X, C))],
                QueryTerm: Y,
                ExpectedReturnValue: [C, X, F(X), F(F(X, C))]),

            new( // Get all instances of top-level function
                StoredTerms: [F(C), F(D), F(F(C)), F(C, D), C],
                QueryTerm: F(X),
                ExpectedReturnValue: [F(C), F(D), F(F(C))]),

            new( // Get all instances of top-level function with (possibly) different args
                StoredTerms: [F(C, C), F(D, D), F(C, D), F(C)],
                QueryTerm: F(X, Y),
                ExpectedReturnValue: [F(C, C), F(D, D), F(C, D)]),

            new( // Get all instances of top-level function with repeated arg
                StoredTerms: [F(C, C), F(C, D), F(F(X), F(X)), F(F(X), F(Y))],
                QueryTerm: F(X, X),
                ExpectedReturnValue: [F(C, C), F(F(X), F(X))]),

            new(
                StoredTerms: [F(X, D)],
                QueryTerm: F(C, Y),
                ExpectedReturnValue: []),
        ])
        .When(tc =>
        {
            var tree = new PathTree(tc.StoredTerms);
            return tree.GetInstances(tc.QueryTerm);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedReturnValue));

    public static Test GetGeneralisationsBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new(
                StoredTerms: [C, D, X],
                QueryTerm: C,
                ExpectedReturnValue: [C, X]),

            new(
                StoredTerms: [F(X), F(C, D)],
                QueryTerm: F(C),
                ExpectedReturnValue: [F(X)]),

            new(
                StoredTerms: [F(X), F(C, D)],
                QueryTerm: F(Y),
                ExpectedReturnValue: [F(X)]),

            new(
                StoredTerms: [F(X), F(C), F(F(X)), F(C, D)],
                QueryTerm: F(F(C)),
                ExpectedReturnValue: [F(X), F(F(X))]),

            new(
                StoredTerms: [F(X, X), F(X, Y)],
                QueryTerm: F(C, D),
                ExpectedReturnValue: [F(X, Y)]),

            new(
                StoredTerms: [F(X, X), F(X, Y)],
                QueryTerm: F(C, C),
                ExpectedReturnValue: [F(X, X), F(X, Y)]),

            new(
                StoredTerms: [F(X, D)],
                QueryTerm: F(C, Y),
                ExpectedReturnValue: []),
        ])
        .When(tc =>
        {
            var tree = new PathTree(tc.StoredTerms);
            return tree.GetGeneralisations(tc.QueryTerm);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedReturnValue));

    private record PositiveAddTestCase(Term[] CurrentTerms, Term NewTerm, Dictionary<IPathTreeArgumentNodeKey, object> ExpectedRootChildren);

    private record NegativeAddTestCase(Term[] CurrentTerms, Term NewTerm);

    private record ContainsTestCase(Term[] StoredTerms, Term QueryTerm, bool ExpectedReturnValue);

    private record GetTestCase(Term[] StoredTerms, Term QueryTerm, Term[] ExpectedReturnValue);
}
