using FluentAssertions;
using FluentAssertions.Equivalency;
using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.TestUtilities
{
    public static class EquivalencyOptions
    {
        private static readonly Func<Function, bool> isSkolemFunction = f => f.Symbol is SkolemFunctionSymbol;

        public static EquivalencyAssertionOptions<Sentence> UsingOnlyConsistencyForVariables(this EquivalencyAssertionOptions<Sentence> opts)
        {
            return opts
                .RespectingRuntimeTypes()
                .ComparingByMembers<Sentence>()
                .ComparingByMembers<Term>()
                .UsingJustAConsistencyCheckFor<Sentence, VariableDeclaration>()
                .UsingJustAConsistencyCheckFor<Sentence, VariableReference>();
        }

        public static EquivalencyAssertionOptions<Sentence> UsingOnlyConsistencyForVariablesAndSkolemFunctions(this EquivalencyAssertionOptions<Sentence> opts)
        {
            return opts
                .RespectingRuntimeTypes()
                .ComparingByMembers<Sentence>()
                .ComparingByMembers<Term>()
                .UsingJustAConsistencyCheckFor<Sentence, VariableDeclaration>()
                .UsingJustAConsistencyCheckFor<Sentence, VariableReference>()
                .UsingJustAConsistencyCheckFor<Sentence, Function>(isSkolemFunction)
                .WithTracing();
        }

        /// <summary>
        /// Applies equivalency for a given type based purely on consistency. That is, we don't care about the specifics of the
        /// actual object, as long as we encounter a matching actual object wherever the expected object occurs in the expectation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static EquivalencyAssertionOptions<TRoot> UsingJustAConsistencyCheckFor<TRoot, TType>(this EquivalencyAssertionOptions<TRoot> opts, Func<TType, bool>? filter = null)
            where TType : class
        {
            Dictionary<(string root, TType expectation), TType> actualByExpectation = new();

            return opts
                .Using<TType>(ctx =>
                {
                    // We use this for comparing collections. When we do so, we want to allow the mapping of expected to actual
                    // to differ for different elements of the collection. Yes, this is rather hacky.
                    var root = ctx.SelectedNode.RootIsCollection ? ctx.SelectedNode.Path.Split(".")[0] : string.Empty;

                    if (filter?.Invoke(ctx.Subject) ?? true)
                    {
                        if (actualByExpectation.TryGetValue((root, ctx.Expectation), out var actual))
                        {
                            ctx.Subject.Should().Be(actual);
                        }
                        else
                        {
                            actualByExpectation[(root, ctx.Expectation)] = ctx.Subject;
                        }
                    }
                    else
                    {
                        ctx.Subject.Should().Be(ctx.Expectation);
                    }
                })
                .WhenTypeIs<TType>();
        }
    }
}
