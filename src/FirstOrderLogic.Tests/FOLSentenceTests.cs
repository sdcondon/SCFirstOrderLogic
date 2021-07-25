using FluentAssertions;
using System.Linq;
using Xunit;

namespace LinqToKB.FirstOrderLogic
{
    public class FOLSentenceTests
    {
        [Fact]
        public void Smoke()
        {
            FOLSentence<string>.TryCreate(d => d.Any(x => x == "Hello"), out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new FOLExistentialQuantification<string>(
                new FOLVariableTerm<string>("x"),
                new FOLEquality<string>(
                    new FOLVariableTerm<string>("x"),
                    new FOLConstantTerm<string>("Hello"))));
        }

        [Fact]
        public void Smoke2()
        {
            FOLSentence<string>.TryCreate(d => d.All(x => d.Any(y => x == y)), out var sentence).Should().BeTrue();

            sentence.Should().BeEquivalentTo(new FOLUniversalQuantification<string>(
                new FOLVariableTerm<string>("x"),
                new FOLExistentialQuantification<string>(
                    new FOLVariableTerm<string>("y"),
                    new FOLEquality<string>(
                        new FOLVariableTerm<string>("x"),
                        new FOLVariableTerm<string>("y")))));
        }
    }
}
