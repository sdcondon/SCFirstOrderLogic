#if false
using FluentAssertions;
using FlUnit;

namespace SCFirstOrderLogic.SentenceCreation
{
    public static class SentenceParserTests
    {
        public static Test ParseBehaviour => TestThat
            .GivenEachOf(() => new ParseTestCase[]
            {
                new(
                    Sentence: "P()",
                    ExpectedResult: new Predicate("P")),

                new( // also FOR-ALL? or maybe just ALL?
                    Sentence: "∀x, P(x)",
                    ExpectedResult: new UniversalQuantification(new("x"), new Predicate("P", new VariableReference("x")))),

                new( // also THERE-EXISTS? or maybe just EXISTS?
                    Sentence: "∃x, y, P(x, y)",
                    ExpectedResult: new ExistentialQuantification(new("y"), new Predicate("P", new VariableReference("x"), new VariableReference("y")))),

                new( // also AND?
                    Sentence: "P(x) ∧ Q(y)",
                    ExpectedResult: new Conjunction(new Predicate("P", new Constant("x")), new Predicate("Q", new Constant("y")))),

                new( // also OR? also NOT?
                    Sentence: "P(x) ∨ ¬Q(y)",
                    ExpectedResult: new Disjunction(new Predicate("P", new Constant("x")), new Negation(new Predicate("Q", new Constant("y"))))),

                new( // also => or -> ?
                    Sentence: "P(x) ⇒ Q(y)",
                    ExpectedResult: new Implication(new Predicate("P", new Constant("x")), new Predicate("Q", new Constant("y")))),

                new( // also <=> or <-> ?
                    Sentence: "P(x) ⇔ Q(y)",
                    ExpectedResult: new Equivalence(new Predicate("P", new Constant("x")), new Predicate("Q", new Constant("y")))),

                new(
                    Sentence: "F1(x) = F2(y, z)",
                    ExpectedResult: new Predicate(EqualitySymbol.Instance, new Function("F1", new Constant("x"), new Function("F2", new Constant("y"), new Constant("z"))))),
            })
            .When(tc => Sentence.Parse(tc.Sentence))
            .ThenReturns()
            .And((ParseTestCase tc, Sentence rv) => rv.Should().Be(tc.ExpectedResult));

        private record ParseTestCase(string Sentence, Sentence ExpectedResult);
    }
}
#endif