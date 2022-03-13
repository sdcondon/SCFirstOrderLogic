using BenchmarkDotNet.Attributes;
using FluentAssertions;
using SCFirstOrderLogic.SentenceManipulation;
using System.Collections.Generic;
using AltSentenceUnifier = SCFirstOrderLogic.Benchmarks.AlternativeImplementations.FromAiAModernApproach.SentenceUnifier;

namespace SCFirstOrderLogic.Benchmarks
{
    [MemoryDiagnoser]
    [InProcess]
    public class SentenceUnification
    {
        private static Function Mother(Term child) => new("Mother", child);
        private static Predicate Knows(Term knower, Term known) => new("Knows", knower, known);
        private static readonly Constant john = new("John");
        private static readonly Constant jane = new("Jane");
        private static readonly VariableDeclaration x = new("x");
        private static readonly VariableDeclaration y = new("y");

        [Benchmark]
        public void Unify1()
        {
            var sentence1 = Knows(john, x);
            var sentence2 = Knows(john, jane);

            new SentenceUnifier().TryUnify(sentence1, sentence2, out var unifier);

            unifier
                .Should()
                .Equal(new Dictionary<VariableReference, Term>()
                {
                    [x] = jane,
                });
        }

        [Benchmark]
        public void AltUnify1()
        {
            var sentence1 = Knows(john, x);
            var sentence2 = Knows(john, jane);

            AltSentenceUnifier
                .Unify(sentence1, sentence2, null)
                .Mapping
                .Should()
                .Equal(new Dictionary<object, object>()
                {
                    [(VariableReference)x] = jane,
                });
        }

        [Benchmark]
        public void Unify2()
        {
            var sentence1 = Knows(john, x);
            var sentence2 = Knows(y, jane);

            new SentenceUnifier().TryUnify(sentence1, sentence2, out var unifier);

            unifier
                .Should()
                .Equal(new Dictionary<VariableReference, Term>()
                {
                    [x] = jane,
                    [y] = john,
                });
        }

        [Benchmark]
        public void AltUnify2()
        {
            var sentence1 = Knows(john, x);
            var sentence2 = Knows(y, jane);

            AltSentenceUnifier
                .Unify(sentence1, sentence2, null)
                .Mapping
                .Should()
                .Equal(new Dictionary<object, object>()
                {
                    [(VariableReference)x] = jane,
                    [(VariableReference)y] = john,
                });
        }

        [Benchmark]
        public void Unify3()
        {
            var sentence1 = Knows(john, x);
            var sentence2 = Knows(y, Mother(y));

            new SentenceUnifier().TryUnify(sentence1, sentence2, out var unifier);

            unifier
                .Should()
                .Equal(new Dictionary<VariableReference, Term>()
                {
                    // Book says that x should be Mother(john), but that's not what the algorithm they give
                    // produces. Easy enough to resolve (after all, y is john), but waiting and seeing how it pans out through usage..
                    [x] = Mother(y),
                    [y] = john,
                });
        }

        [Benchmark]
        public void AltUnify3()
        {
            var sentence1 = Knows(john, x);
            var sentence2 = Knows(y, Mother(y));

            AltSentenceUnifier
                .Unify(sentence1, sentence2, null)
                .Mapping
                .Should()
                .Equal(new Dictionary<object, object>()
                {
                    // Book says that x should be Mother(john), but that's not what the algorithm they give
                    // produces. Easy enough to resolve (after all, y is john), but waiting and seeing how it pans out through usage..
                    [(VariableReference)x] = Mother(y),
                    [(VariableReference)y] = john,
                });
        }
    }
}
