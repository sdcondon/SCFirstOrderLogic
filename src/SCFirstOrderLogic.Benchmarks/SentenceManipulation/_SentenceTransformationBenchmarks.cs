﻿using BenchmarkDotNet.Attributes;
using static SCFirstOrderLogic.SentenceManipulation.SentenceFactory;

namespace SCFirstOrderLogic.SentenceManipulation
{
    [MemoryDiagnoser]
    [InProcess]
    public class _SentenceTransformationBenchmarks
    {
        private static Predicate IsAnimal(Term term) => new(nameof(IsAnimal), term);
        private static Predicate Loves(Term term1, Term term2) => new(nameof(Loves), term1, term2);

        private static Sentence NonTrivialSentence { get; } = ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X))));

        [Benchmark]
        public static Sentence DoCNFConversion_ProductionVersion()
        {
            var sentence = ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X))));

            return new CNFConversion().ApplyTo(sentence);
        }

        [Benchmark]
        public static Sentence DoCNFConversion_WithoutTypeSwitch()
        {
            var sentence = ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X))));

            return CNFConversion_WithoutTypeSwitch.ApplyTo(sentence);
        }

        [Benchmark]
        public static CNFSentence DoCNFCtor_ProductionVersion()
        {
            return new CNFSentence(NonTrivialSentence);
        }

        [Benchmark]
        public static CNFSentence_WithoutTypeSwitch DoCNFCtor_WithoutTypeSwitch()
        {
            return new CNFSentence_WithoutTypeSwitch(NonTrivialSentence);
        }
    }
}
