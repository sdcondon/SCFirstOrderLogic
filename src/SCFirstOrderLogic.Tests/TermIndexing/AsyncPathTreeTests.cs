using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

namespace SCFirstOrderLogic.TermIndexing;

public static class AsyncPathTreeTests
{
    private static readonly Constant C1 = new("C1");
    private static readonly Constant C2 = new("C2");

    private static Function F(params Term[] a) => new(nameof(F), a);

    // Path trees are a well-known data structure - so I'm asserting that asserting on the internal structure is valid. Probably.
    public static Test AddBehaviour_Positive => TestThat
        .GivenEachOf(() => new PositiveAddTestCase[]
        {
            new(
                CurrentTerms: [],
                NewTerm: C1,
                ExpectedRootChildren: new()
                {
                    [new PathTreeConstantNodeKey("C1")] = new
                    {
                        Values = new[] { KeyValuePair.Create(C1, C1) }
                    },
                }),

            new(
                CurrentTerms: [F(X)],
                NewTerm: F(C1),
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
                                    [new PathTreeConstantNodeKey("C1")] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(C1), F(C1)) }
                                    },
                                }
                            },
                        }
                    },
                }),

            new(
                CurrentTerms: [F(X, C1)],
                NewTerm: F(Y, C2),
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
                                        Values = new[] { KeyValuePair.Create(F(Var(0), C1), F(X, C1)), KeyValuePair.Create(F(Var(0), C2), F(Y, C2)) }
                                    },
                                }
                            },
                            new
                            {
                                Children = new Dictionary<IPathTreeArgumentNodeKey, object>
                                {
                                    [new PathTreeConstantNodeKey("C1")] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(Var(0), C1), F(X, C1)) }
                                    },
                                    [new PathTreeConstantNodeKey("C2")] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(F(Var(0), C2), F(Y, C2)) }
                                    },
                                }
                            },
                        }
                    }
                }),

            // Same function identifier with different arg count shouldn't cause problems:
            new(
                CurrentTerms: [F()],
                NewTerm: F(C1),
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
                                    [new PathTreeConstantNodeKey("C1")] = new
                                    {
                                        Values = new[] { KeyValuePair.Create(new Function("F", C1), new Function("F", C1)) }
                                    }
                                }
                            }
                        }
                    },
                }),
        })
        .When(tc =>
        {
            var rootNode = new AsyncPathTreeDictionaryNode<Term>();
            var tree = new AsyncPathTree(rootNode, tc.CurrentTerms);
            tree.AddAsync(tc.NewTerm).GetAwaiter().GetResult();
            return GetParameterNodeChildren(rootNode);

            static Dictionary<IPathTreeArgumentNodeKey, object> GetParameterNodeChildren(IAsyncPathTreeParameterNode<Term> node)
            {
                return new(node.GetChildren().ToListAsync().GetAwaiter().GetResult().Select(kvp =>
                {
                    var children = GetArgumentNodeChildren(kvp.Value);
                    object comparisonObject = children.Count() > 0 ? new { Children = children } : new { Values = kvp.Value.GetValues().ToArrayAsync().GetAwaiter().GetResult() };
                    return KeyValuePair.Create(kvp.Key, comparisonObject);
                }));
            }

            static IEnumerable<object> GetArgumentNodeChildren(IAsyncPathTreeArgumentNode<Term> node)
            {
                return node.GetChildren().ToListAsync().GetAwaiter().GetResult().Select(n =>
                {
                    return new { Children = GetParameterNodeChildren(n) };
                });
            }
        })
        .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedRootChildren));

    public static Test AddBehaviour_Negative => TestThat
        .GivenEachOf(() => new NegativeAddTestCase[]
        {
            new(
                CurrentTerms: [C1],
                NewTerm: C1),

            new(
                CurrentTerms: [F(X)],
                NewTerm: F(X)),

            new(
                CurrentTerms: [F(C1)],
                NewTerm: F(C1)),
        })
        .When(tc =>
        {
            var tree = new AsyncPathTree(new AsyncPathTreeDictionaryNode<Term>(), tc.CurrentTerms);
            tree.AddAsync(tc.NewTerm).GetAwaiter().GetResult();
        })
        .ThenThrows();

    public static Test ContainsBehaviour => TestThat
        .GivenEachOf(() => new ContainsTestCase[]
        {
            new(
                StoredTerms: [C1, C2, X],
                QueryTerm: C1,
                ExpectedReturnValue: true),

            new(
                StoredTerms: [C1, C2, X],
                QueryTerm: X,
                ExpectedReturnValue: true),

            new( // variable identifier shouldn't matter
                StoredTerms: [C1, C2, X],
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
                StoredTerms: [F(X), F(C2)],
                QueryTerm: F(C1),
                ExpectedReturnValue: false),

            new(
                StoredTerms: [F(C1, C1), F(C2, C2), F(C1, C2)],
                QueryTerm: F(X, X),
                ExpectedReturnValue: false),

            new(
                StoredTerms: [F(X, C2)],
                QueryTerm: F(C1, Y),
                ExpectedReturnValue: false),
        })
        .When(tc =>
        {
            var tree = new AsyncPathTree(new AsyncPathTreeDictionaryNode<Term>(), tc.StoredTerms);
            return tree.ContainsAsync(tc.QueryTerm).GetAwaiter().GetResult();
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    public static Test GetInstancesBehaviour => TestThat
        .GivenEachOf(() => new GetTestCase[]
        {
            new( // Exact match
                StoredTerms: [C1, C2, X],
                QueryTerm: C1,
                ExpectedReturnValue: [C1]),

            new( // Get everything
                StoredTerms: [C1, X, F(X), F(F(X, C1))],
                QueryTerm: Y,
                ExpectedReturnValue: [C1, X, F(X), F(F(X, C1))]),

            new( // Get all instances of top-level function
                StoredTerms: [F(C1), F(C2), F(F(C1)), F(C1, C2), C1],
                QueryTerm: F(X),
                ExpectedReturnValue: [F(C1), F(C2), F(F(C1))]),

            new( // Get all instances of top-level function with (possibly) different args
                StoredTerms: [F(C1, C1), F(C2, C2), F(C1, C2), F(C1)],
                QueryTerm: F(X, Y),
                ExpectedReturnValue: [F(C1, C1), F(C2, C2), F(C1, C2)]),

            new( // Get all instances of top-level function with repeated arg
                StoredTerms: [F(C1, C1), F(C1, C2), F(F(X), F(X)), F(F(X), F(Y))],
                QueryTerm: F(X, X),
                ExpectedReturnValue: [F(C1, C1), F(F(X), F(X))]),

            new(
                StoredTerms: [F(X, C2)],
                QueryTerm: F(C1, Y),
                ExpectedReturnValue: []),
        })
        .When(tc =>
        {
            var tree = new AsyncPathTree(new AsyncPathTreeDictionaryNode<Term>(), tc.StoredTerms);
            return tree.GetInstances(tc.QueryTerm).ToListAsync().GetAwaiter().GetResult(); ;
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedReturnValue));

    public static Test GetGeneralisationsBehaviour => TestThat
        .GivenEachOf(() => new GetTestCase[]
        {
            new(
                StoredTerms: [C1, C2, X],
                QueryTerm: C1,
                ExpectedReturnValue: [C1, X]),

            new(
                StoredTerms: [F(X), F(C1, C2)],
                QueryTerm: F(C1),
                ExpectedReturnValue: [F(X)]),

            new(
                StoredTerms: [F(X), F(C1, C2)],
                QueryTerm: F(Y),
                ExpectedReturnValue: [F(X)]),

            new(
                StoredTerms: [F(X), F(C1), F(F(X)), F(C1, C2)],
                QueryTerm: F(F(C1)),
                ExpectedReturnValue: [F(X), F(F(X))]),

            new(
                StoredTerms: [F(X, X), F(X, Y)],
                QueryTerm: F(C1, C2),
                ExpectedReturnValue: [F(X, Y)]),

            new(
                StoredTerms: [F(X, X), F(X, Y)],
                QueryTerm: F(C1, C1),
                ExpectedReturnValue: [F(X, X), F(X, Y)]),

            new(
                StoredTerms: [F(X, C2)],
                QueryTerm: F(C1, Y),
                ExpectedReturnValue: []),
        })
        .When(tc =>
        {
            var tree = new AsyncPathTree(new AsyncPathTreeDictionaryNode<Term>(), tc.StoredTerms);
            return tree.GetGeneralisations(tc.QueryTerm).ToListAsync().GetAwaiter().GetResult();
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedReturnValue));

    private record PositiveAddTestCase(Term[] CurrentTerms, Term NewTerm, Dictionary<IPathTreeArgumentNodeKey, object> ExpectedRootChildren);

    private record NegativeAddTestCase(Term[] CurrentTerms, Term NewTerm);

    private record ContainsTestCase(Term[] StoredTerms, Term QueryTerm, bool ExpectedReturnValue);

    private record GetTestCase(Term[] StoredTerms, Term QueryTerm, Term[] ExpectedReturnValue);
}
