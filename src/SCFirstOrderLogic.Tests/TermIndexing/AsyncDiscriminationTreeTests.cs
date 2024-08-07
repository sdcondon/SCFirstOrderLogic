﻿using FluentAssertions;
using FlUnit;
using System.Collections.Generic;
using System.Linq;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;
using FunctionNodeKey = SCFirstOrderLogic.TermIndexing.DiscriminationTreeFunctionNodeKey;
using INodeKey = SCFirstOrderLogic.TermIndexing.IDiscriminationTreeNodeKey;
using VariableNodeKey = SCFirstOrderLogic.TermIndexing.DiscriminationTreeVariableNodeKey;

namespace SCFirstOrderLogic.TermIndexing;

public static class AsyncDiscriminationTreeTests
{
    private static readonly Function C1 = new(nameof(C1));
    private static readonly Function C2 = new(nameof(C2));

    private static Function F(params Term[] a) => new(nameof(F), a);

    // Discrimination trees are a well-known data structure - so I'm asserting that asserting on the internal structure is valid. Probably.
    public static Test AddBehaviour_Positive => TestThat
        .GivenEachOf<PositiveAddTestCase>(() =>
        [
            new(
                CurrentTerms: [],
                NewTerm: C1,
                ExpectedRootChildren: new()
                {
                    [new FunctionNodeKey("C1", 0)] = new { Value = C1 },
                }),

            new(
                CurrentTerms: [F(X)],
                NewTerm: F(C1),
                ExpectedRootChildren: new()
                {
                    [new FunctionNodeKey("F", 1)] = new
                    {
                        Children = new Dictionary<INodeKey, object>
                        {
                            [new VariableNodeKey(0)] = new { Value = F(X) },
                            [new FunctionNodeKey("C1", 0)] = new { Value = F(C1) },
                        }
                    },
                }),

            new(
                CurrentTerms: [F(X, C2)],
                NewTerm: F(X, C1),
                ExpectedRootChildren: new()
                {
                    [new FunctionNodeKey("F", 2)] = new
                    {
                        Children = new Dictionary<INodeKey, object>
                        {
                            [new VariableNodeKey(0)] = new
                            { 
                                Children = new Dictionary<INodeKey, object>
                                {
                                    [new FunctionNodeKey("C1", 0)] = new { Value = F(X, C1) },
                                    [new FunctionNodeKey("C2", 0)] = new { Value = F(X, C2) },
                                }
                            },
                        }
                    }
                }),

            new(
                CurrentTerms: [],
                NewTerm: F(F(C1), F(C2)),
                ExpectedRootChildren: new()
                {
                    [new FunctionNodeKey("F", 2)] = new
                    {
                        Children = new Dictionary<INodeKey, object>
                        {
                            [new FunctionNodeKey("F", 1)] = new
                            {
                                Children = new Dictionary<INodeKey, object>
                                {
                                    [new FunctionNodeKey("C1", 0)] = new
                                    {
                                        Children = new Dictionary<INodeKey, object>
                                        {
                                            [new FunctionNodeKey("F", 1)] = new
                                            {
                                                Children = new Dictionary<INodeKey, object>
                                                {
                                                    [new FunctionNodeKey("C2", 0)] = new { Value = F(F(C1), F(C2)) },
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
                CurrentTerms: [F()],
                NewTerm: F(C1),
                ExpectedRootChildren: new()
                {
                    [new FunctionNodeKey("F", 0)] = new
                    {
                        Value = new Function("F")
                    },
                    [new FunctionNodeKey("F", 1)] = new
                    {
                        Children = new Dictionary<INodeKey, object>
                        {
                            [new FunctionNodeKey("C1", 0)] = new { Value = new Function("F", C1) }
                        }
                    },
                }),
        ])
        .WhenAsync(async tc =>
        {
            var root = new AsyncDiscriminationTreeDictionaryNode<Term>();
            var tree = new AsyncDiscriminationTree(root, tc.CurrentTerms);
            await tree.AddAsync(tc.NewTerm);
            return GetChildren(root);

            static Dictionary<INodeKey, object> GetChildren(IAsyncDiscriminationTreeNode<Term> node)
            {
                return new(node.GetChildren().ToListAsync().AsTask().GetAwaiter().GetResult().Select(kvp =>
                {
                    var children = GetChildren(kvp.Value);
                    object comparisonObject = children.Count > 0 ? new { Children = children } : new { kvp.Value.Value };
                    return KeyValuePair.Create(kvp.Key, comparisonObject);
                }));
            }
        })
        .ThenReturns((tc, rv) => rv.Should().BeEquivalentTo(tc.ExpectedRootChildren));

    public static Test AddBehaviour_Negative => TestThat
        .GivenEachOf<NegativeAddTestCase>(() =>
        [
            new(
                CurrentTerms: [C1],
                NewTerm: C1),

            new(
                CurrentTerms: [F(X)],
                NewTerm: F(X)),

            new(
                CurrentTerms: [F(C1)],
                NewTerm: F(C1)),
        ])
        .WhenAsync(async tc =>
        {
            var tree = new AsyncDiscriminationTree(new AsyncDiscriminationTreeDictionaryNode<Term>(), tc.CurrentTerms);
            await tree.AddAsync(tc.NewTerm);
        })
        .ThenThrows();

    public static Test ContainsBehaviour => TestThat
        .GivenEachOf<ContainsTestCase>(() =>
        [
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
        ])
        .WhenAsync(async tc =>
        {
            var tree = new AsyncDiscriminationTree(new AsyncDiscriminationTreeDictionaryNode<Term>(), tc.StoredTerms);
            return await tree.ContainsAsync(tc.QueryTerm);
        })
        .ThenReturns()
        .And((tc, rv) => rv.Should().Be(tc.ExpectedReturnValue));

    public static Test GetInstancesBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
            new( // Exact match
                StoredTerms: [C1, C2, X],
                QueryTerm: C1,
                ExpectedReturnValue: [C1]),

            new( // Get everything
                StoredTerms: [C1, X, F(X), F(F(X, C1))],
                QueryTerm: X,
                ExpectedReturnValue: [C1, X, F(X), F(F(X, C1))]),

            new( // Get all instances of top-level function
                StoredTerms: [F(C1), F(C2), F(F(C1)), F(C1, C2), C1],
                QueryTerm: F(X),
                ExpectedReturnValue: [F(C1), F(C2), F(F(C1))]),

            new( // Get all instances of top-level function with any args
                StoredTerms: [F(C1, C1), F(C2, C2), F(C1, C2)],
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
        ])
        .When(async tc =>
        {
            var tree = new AsyncDiscriminationTree(new AsyncDiscriminationTreeDictionaryNode<Term>(), tc.StoredTerms);
            return await tree.GetInstances(tc.QueryTerm).ToListAsync();
        })
        .ThenReturns()
        .And((tc, rv) => rv.Result.Should().BeEquivalentTo(tc.ExpectedReturnValue));

    public static Test GetGeneralisationsBehaviour => TestThat
        .GivenEachOf<GetTestCase>(() =>
        [
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
        ])
        .When(async tc =>
        {
            var tree = new AsyncDiscriminationTree(new AsyncDiscriminationTreeDictionaryNode<Term>(), tc.StoredTerms);
            return await tree.GetGeneralisations(tc.QueryTerm).ToListAsync();
        })
        .ThenReturns()
        .And((tc, rv) => rv.Result.Should().BeEquivalentTo(tc.ExpectedReturnValue));

    private record PositiveAddTestCase(Term[] CurrentTerms, Term NewTerm, Dictionary<INodeKey, object> ExpectedRootChildren);

    private record NegativeAddTestCase(Term[] CurrentTerms, Term NewTerm);

    private record ContainsTestCase(Term[] StoredTerms, Term QueryTerm, bool ExpectedReturnValue);

    private record GetTestCase(Term[] StoredTerms, Term QueryTerm, Term[] ExpectedReturnValue);
}
