using System.Collections.Generic;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter9
{
    //// The curiousity and the cat example from section 9.5 of
    //// Artificial Intelligence: A Modern Approach, Global Edition by Stuart Russel and Peter Norvig.

    public static class CuriousityAndTheCatDomain
    {
        public static IReadOnlyCollection<Sentence> Axioms { get; } = new List<Sentence>()
        {
            // Everyone who loves all animals is loved by someone.
            // ∀x [∀y Animal(y) ⇒ Loves(x, y)] ⇒ [∃y Loves(y, x)]
            ForAll(X, If(
                ForAll(Y, If(IsAnimal(Y), Loves(X, Y))),
                ThereExists(Y, Loves(Y, X)))),

            // Anyone who kills an animal is loved by no one.
            // ∀x [∃z Animal(z) ∧ Kills(x, z)] ⇒ [∀y ¬Loves(y, x)]
            ForAll(X, If(
                ThereExists(Z, And(IsAnimal(Z), Kills(X, Z))),
                ForAll(Y, Not(Loves(Y, X))))),

            // Jack loves all animals.
            // ∀x Animal(x) ⇒ Loves(Jack, x)
            ForAll(X, If(IsAnimal(X), Loves(Jack, X))),

            // Either Jack or Curiosity killed the cat, who is named Tuna.
            // Kills(Jack, Tuna) ∨ Kills(Curiosity, Tuna)
            // Cat(Tuna)
            Or(Kills(Jack, Tuna), Kills(Curiousity, Tuna)),
            IsCat(Tuna),

            // Cats are animals.
            // ∀x Cat(x) ⇒ Animal(x)
            ForAll(X, If(IsCat(X), IsAnimal(X))),

        }.AsReadOnly();

        public static Constant Jack { get; } = new Constant(nameof(Jack));
        public static Constant Tuna { get; } = new Constant(nameof(Jack));
        public static Constant Curiousity { get; } = new Constant(nameof(Jack));

        public static Predicate IsAnimal(Term subject) => new Predicate(nameof(IsAnimal), subject);
        public static Predicate IsCat(Term subject) => new Predicate(nameof(IsCat), subject);
        public static Predicate Loves(Term subject, Term @object) => new Predicate(nameof(Loves), subject, @object);
        public static Predicate Kills(Term subject, Term @object) => new Predicate(nameof(Kills), subject, @object);
    }
}
