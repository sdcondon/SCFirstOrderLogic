# Getting Started

NB: This guide assumes some familiarity with first-order logic. It won't explain first-order logic concepts - it'll just explain how to use FoL via this library.

## Writing Sentences

The first challenge is to write sentences. This can be done in several ways. This section will use the following sentence as an example:

`∀ g, c, IsGrandparentOf(g, c) ⇔ [∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]`

..that is, the definition of a grandparent. We use this example because its very straightforward but includes a decent cross-section of FoL elements.

### Writing Sentences Directly

The most basic way to express sentences is to directly compose instances of the types found in the top-level `SCFirstOrderLogic` namespace into a tree structure, like this:

```
using SCFirstOrderLogic;

// Helper methods for creating your predicates (and functions) are recommended:
Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

// Now the sentence itself, with some intermediate variables so that it isn't completely unreadable:
var g = new VariableDeclaration("g");
var c = new VariableDeclaration("c");
var p = new VariableDeclaration("p");
var equivalenceLHS = IsGrandparent(g, c);
var equivalenceRHS = new ExistentialQuantification(p, new Conjunction(IsParent(g, p), IsParent(p, c)));
Sentence grandparentDefn = new UniversalQuantification(g, new UniversalQuantification(c, new Equivalence(equivalenceLHS, equivalenceRHS));
```

Notice that:

* This is very simple in that it involves nothing other than the sentence types themselves, but is obviously far too verbose to be workable. Hence the alternatives.

### Writing Sentences with SentenceFactory

First, we have the `SentenceFactory` static class in the `SentenceCreation` namespace. It is intended to be used via a `using static` directive,
and includes a number of static methods and properties to assist with succinct sentence creation. Here's how the example looks with this one:

```
using SCFirstOrderLogic;
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

// Helper methods for your predicates (and functions) are also recommended with this approach:
Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

// Using the factory, the sentence itself is a little less verbose:
var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, And(IsParent(G, P), IsParent(P, C)))));
```

Notice that:

* The factory provides `ForAll` and `ThereExists` methods for creating quantifications. There are overloads for declaring multiple variables at once.
* The factory provides `If` and `Iff` methods for creating implications and equivalences, respectively.
* The factory provides methods for conjunctions (`And`), disjunctions (`Or`) and negations (`Not`). See the next two examples if you really want to use C# operators for these.
* The factory provides `A` through `Z` as properties that return variable declarations with these letters as their symbol.

### Writing Sentences with OperableSentenceFactory

Next, you'll also find `OperableSentenceFactory` in `SentenceCreation`. It works similarly to `SentenceFactory`, but lets you use operators:

```
using SCFirstOrderLogic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

// Helper methods for your predicates (and functions) are also recommended with this approach:
OperablePredicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
OperablePredicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

// This is probably the most succinct of the approaches:
var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, IsParent(G, P) & IsParent(P, C))));
```

Notice that:

* We've only used `&` (for a conjunction) above, but `|` works for disjunctions, and `!` for negations. In addition, `==` (applied to terms rather than sentences, of course) works for the equality predicate.
* Other aspects of this factory are the same as `SentenceFactory` - it also offers `ThereExists`, `ForAll`, `Iff`, `If` and single-letter variable declaration properties.
* The only proviso is that the supporting methods for domain specific elements now need to use `Operable..` as their return type - which is easy as these types are implicitly convertible from the normal equivalents.

### Writing Sentences with LanguageIntegration

Finally, there are the types to be found in the `LanguageIntegration` namespace. The `SentenceFactory` in this namespace is based on the idea of
modelling the domain as an IEnumerable&lt;T&gt;, then expressing our sentence as a boolean-valued LINQ expression. Like this:

```
using SCFirstOrderLogic.LanguageIntegration;
using static SCFirstOrderLogic.LanguageIntegration.Operators; // Contains Iff an If methods

// The helper methods recommended for the other approaches become full interfaces
// when language integration is used (no implementation is needed):
interface IPerson
{
    bool IsParentOf(IPerson person);
    bool IsGrandparentOf(IPerson person);
}

// Now the sentence itself looks like this:
var grandparentDefn = 
    SentenceFactory.Create&lt;IPerson&gt;(d => d.All((g, c) => Iff(g.IsGrandparentOf(c), d.Any(p => g.IsParentOf(p) && p.IsParentOf(c)))));
```

Notice that:

* This is obviously non-trivial - more information can be found on the [language integration](beyond-getting-started/language-integration.md) page.

### (Not) Writing Sentences as Strings

Being able to point a parser at "∀ g, c, IsGrandparentOf(g, c) ⇔ [∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]" would be great,
but I've not gotten around to that just yet. At some point in the future I may take a look at this (shouldn't be too tough, especially if I make use of something like ANTLR)

## Storing Knowledge and Making Inferences

Once you have some sentences, storing them and making inferences is done with the aid of the types in the `Inference` namespace.
The most important type here is the `IKnowledgeBase` interface. The library includes a few very simple knowledge bases - one that
uses forward chaining, one that uses backward chaining, and one that uses resolution. Some examples follow, but first, here's our domain,
taken from section 9.3 of 'Artificial Intelligence: A Modern Approach'. Note that it consists only of definite clauses, so we can use
forward and backward chaining on it:

```
using SCFirstOrderLogic;
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

Constant America = new Constant(nameof(America));
Constant Nono = new Constant(nameof(Nono));
Constant West = new Constant(nameof(West));

OperablePredicate IsAmerican(Term t) => new Predicate(nameof(IsAmerican), t);
OperablePredicate IsHostile(Term t) => new Predicate(nameof(IsHostile), t);
OperablePredicate IsCriminal(Term t) => new Predicate(nameof(IsCriminal), t);
OperablePredicate IsWeapon(Term t) => new Predicate(nameof(IsWeapon), t);
OperablePredicate IsMissile(Term t) => new Predicate(nameof(IsMissile), t);
OperablePredicate Owns(Term owner, Term owned) => new Predicate(nameof(Owns), owner, owned);
OperablePredicate Sells(Term seller, Term item, Term buyer) => new Predicate(nameof(Sells), seller, item, buyer);
OperablePredicate IsEnemyOf(Term t, Term other) => new Predicate(nameof(IsEnemyOf), t, other);

var rules = new Sentence[]
{
    // "... it is a crime for an American to sell weapons to hostile nations":
    // American(x) ∧ Weapon(y) ∧ Sells(x, y, z) ∧ Hostile(z) ⇒ Criminal(x).
    ForAll(X, Y, Z, If(IsAmerican(X) & IsWeapon(Y) & Sells(X, Y, Z) & IsHostile(Z), IsCriminal(X))),

    // "Nono... has some missiles."
    // ∃x Missile(x) ∧ Owns(Nono, x)
    ThereExists(X, IsMissile(X) & Owns(Nono, X)),

    // "All of its missiles were sold to it by Colonel West":
    // Missile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)
    ForAll(X, If(IsMissile(X) & Owns(Nono, X), Sells(West, X, Nono))),

    // We will also need to know that missiles are weapons: 
    // Missile(x) ⇒ Weapon(x)
    ForAll(X, If(IsMissile(X), IsWeapon(X))),

    // And we must know that an enemy of America counts as “hostile”:
    // Enemy(x, America) ⇒ Hostile(x)
    ForAll(X, If(IsEnemyOf(X, America), IsHostile(X))),

    // "West, who is American..":
    // American(West)
    IsAmerican(West),

    // "The country Nono, an enemy of America..":
    // Enemy(Nono, America)
    IsEnemyOf(Nono, America),
};
```

### Using Forward Chaining

```
using SCFirstOrderLogic.Inference; // For the "Tell" and "Ask" extension methods - IKnowledgeBase is very async
using SCFirstOrderLogic.Inference.ForwardChaining;

// .. paste the domain listing here ..

// Note that the knowledge base ctor has a "clause store" parameter.
// The clause store takes responsibility for the storage and lookup of
// individual clauses. The package provides only SimpleClauseStore, which stores
// things in memory. This is an extension point - you can create your own implementation
// of IClauseStore to use secondary storage and/or customised indexing, for example.
var kb = new SimpleForwardChainingKnowledgeBase(new SimpleClauseStore());
kb.Tell(rules);
var querySentence = IsCriminal(West);

// Succinct way to get a true/false result:
Console.WriteLine(kb.Ask(querySentence)); // "True"

// Or, to get an explanation:
var query = kb.CreateQuery(querySentence);
query.Execute();
Console.WriteLine(query.Result); // "True"
Console.WriteLine(query.ResultExplanation); // A human-readable walkthrough of the proof tree(s)
// ..note that there are also other properties that facilitate programmatic exploration of the proof tree(s)
```

### Using Backward Chaining

```
using SCFirstOrderLogic.Inference; // For the "Tell" and "Ask" extension methods
using SCFirstOrderLogic.Inference.BackwardChaining;

// .. paste the domain listing here ..

// As with forward chaining, backward chaining KB uses a clause store
// (note that while it has the same name, it is a different class in a
// different namespace - this was perhaps a mistake, but is good for brevity..)
var kb = new SimpleBackwardChainingKnowledgeBase(new SimpleClauseStore());
kb.Tell(rules);
var result = kb.Ask(IsCriminal(West)); // == true
// ..Or can get an explanation in the same way as above
```

### Using Resolution

```
using SCFirstOrderLogic.Inference; // For the "Tell" and "Ask" extension methods
using SCFirstOrderLogic.Inference.Resolution;

// .. paste the domain listing here ..

// The resolution KB has a little more configurability/extensibility than the other two:
var kb = new new SimpleResolutionKnowledgeBase(
    new SimpleClauseStore(),
    SimpleResolutionKnowledgeBase.Filters.None,
    SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);

kb.Tell(rules);
var result = kb.Ask(IsCriminal(West)); // == true
// ..Or can get an explanation in the same way as above
```

Notice that:

* Formatting of sentences and query result explanations includes the appropriate symbols (∀, ⇔, ∃, ∧ and so on).
  Depending on your environment, you might need to take action so that these are outputted properly.
  E.g. For running on Windows it might be worth adding `Console.OutputEncoding = Encoding.Unicode;` to your application start-up.
* The `Tell`, `Ask`, `CreateQuery` and `Execute` methods used above are actually extension methods that are are synchronous wrappers around underlying async versions.
  The library has deep async support - because "real-world" KBs will tend to need to do IO.

## More Examples

For some more examples, see the [example domains](https://github.com/sdcondon/SCFirstOrderLogic/tree/main/src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](https://github.com/sdcondon/SCFirstOrderLogic/tree/main/src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.</p>

## Where Next?

Take a look at [beyond getting started](beyond-getting-started).
