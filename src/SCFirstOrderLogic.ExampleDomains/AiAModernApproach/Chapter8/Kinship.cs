using System.Collections.Generic;
using static SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.Kinship.Domain;
using static SCFirstOrderLogic.Sentence;

namespace SCFirstOrderLogic.ExampleDomains.AiAModernApproach.Chapter8.Kinship
{
    public static class Domain
    {
        //// Unary predicates:
        public static Predicate IsMale(Term subject) => new Predicate(nameof(IsMale), subject);
        public static Predicate IsFemale(Term subject) => new Predicate(nameof(IsMale), subject);

        //// Binary predicates:
        public static Predicate IsParent(Term subject, Term @object) => new Predicate(nameof(IsParent), subject, @object);
        public static Predicate IsSibling(Term subject, Term @object) => new Predicate(nameof(IsSibling), subject, @object);
        public static Predicate IsBrother(Term subject, Term @object) => new Predicate(nameof(IsBrother), subject, @object);
        public static Predicate IsSister(Term subject, Term @object) => new Predicate(nameof(IsSister), subject, @object);
        public static Predicate IsChild(Term subject, Term @object) => new Predicate(nameof(IsChild), subject, @object);
        public static Predicate IsDaughter(Term subject, Term @object) => new Predicate(nameof(IsDaughter), subject, @object);
        public static Predicate IsSon(Term subject, Term @object) => new Predicate(nameof(IsSon), subject, @object);
        public static Predicate IsSpouse(Term subject, Term @object) => new Predicate(nameof(IsSpouse), subject, @object);
        public static Predicate IsWife(Term subject, Term @object) => new Predicate(nameof(IsWife), subject, @object);
        public static Predicate IsHusband(Term subject, Term @object) => new Predicate(nameof(IsHusband), subject, @object);
        public static Predicate IsGrandparent(Term subject, Term @object) => new Predicate(nameof(IsGrandparent), subject, @object);
        public static Predicate IsGrandchild(Term subject, Term @object) => new Predicate(nameof(IsGrandchild), subject, @object);
        public static Predicate IsCousin(Term subject, Term @object) => new Predicate(nameof(IsCousin), subject, @object);
        public static Predicate IsAunt(Term subject, Term @object) => new Predicate(nameof(IsAunt), subject, @object);
        public static Predicate IsUncle(Term subject, Term @object) => new Predicate(nameof(IsUncle), subject, @object);

        //// Unary functions:
        public static Function Mother(Term t) => new Function(nameof(Mother), t);
        public static Function Father(Term t) => new Function(nameof(Father), t);
    }

    /// <summary>
    /// Container for fundamental knowledge about the kinship domain.
    /// </summary>
    public static class KinshipKnowledge
    {
        // Usage (note the separation of concerns for knowledge base implementation and the domain):
        //
        // var kb = new ResolutionKnowledgeBase(); // ..or a different KB implementation - none implemented yet
        // kb.Tell(KinshipKnowledge.Axioms);
        // kb.Tell(..facts about the specific problem..);
        // kb.Ask(..my query..);
        public static IReadOnlyCollection<Sentence> Axioms { get; } = new List<Sentence>()
        {
            // One's mother is one's female parent:
            ForAll(M, C, Iff(AreEqual(Mother(C), M), And(IsFemale(M), IsParent(M, C)))),

            // Ones' husband is one's male spouse:
            ForAll(W, H, Iff(IsHusband(H, W), And(IsMale(H), IsSpouse(H, W)))),

            // Male and female are disjoint categories:
            ForAll(X, Iff(IsMale(X), Not(IsFemale(X)))),

            // Parent and child are inverse relations:
            ForAll(P, C, Iff(IsParent(P, C), IsChild(C, P))),

            // A grandparent is a parent of one's parent:
            ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, And(IsParent(G, P), IsParent(P, C))))),

            // A sibling is another child of one's parents:
            ForAll(X, Y, Iff(IsSibling(X, Y), And(Not(AreEqual(X, Y)), ThereExists(P, And(IsParent(P, X), IsParent(P, Y)))))),

        }.AsReadOnly();

        // Theorems are derivable from axioms, but might be useful for performance...
        public static IReadOnlyCollection<Sentence> Theorems { get; } = new List<Sentence>()
        {
            // Siblinghood is commutative:
            ForAll(X, Y, Iff(IsSibling(X, Y), IsSibling(Y, X))),

        }.AsReadOnly();
    }
}
