# Getting Started

NB: This guide assumes some familiarity with first-order logic. It won't explain first-order logic concepts - it'll just explain how to use FoL via this library.

## Writing Sentences

Obviously the first challenge is to write sentences. This can be done in several ways. This section will use the following sentence as an example:

```
∀ g, c, IsGrandparentOf(g, c) ⇔ [∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]
```

..that is, the definition of a grandparent. We use this example because its very straightforward but includes a decent cross-section of FoL elements.

### Writing Sentences Manually

The most basic way to express this is to manually compose a bunch of instances of the types found in the top-level `SCFirstOrderLogic` namespace, like this:

```csharp
using static SCFirstOrderLogic;

..

// Helper methods for your predicates (and functions) are recommended:
Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

..

var g = new VariableDeclaration("g");
var c = new VariableDeclaration("c");
var p = new VariableDeclaration("p");
var equivalenceLHS = IsGrandparent(g, c);
var equivalenceRHS = new ExistentialQuantification(p, new Conjunction(IsParent(g, p), IsParent(p, c)));
Sentence grandparentDefn = new UniversalQuantification(g, new UniversalQuantification(c, new Equivalence(equivalenceLHS, equivalenceRHS));
```

Things to notice:
* This is very simple in that it involves nothing other than the sentence types themselves, but is obviously far too verbose to be workable. Hence the alternatives.

### Writing Sentences with SentenceFactory

First, we have the `SentenceFactory` static class in the `SentenceCreation` namespace. It is intended to be used via a `using static` directive. Here's how the example looks with this one:

```csharp
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

..

// Helper methods for your predicates (and functions) are recommended:
Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

..

var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, And(IsParent(G, P), IsParent(P, C)))));
```

Things to notice about this one:
* The factory provides ForAll and ThereExists methods for creating quantifications. There are overloads for declaring multiple variables at once.
* The factory provides `A` through `Z` as properties that return variable definitions with these letters as their symbol.
* The factory provides methods for conjunctions (`And`), disjunctions (`Or`) and negations (`Not`). See the next two examples if you really want to use C# operators for these.

### Writing Sentences with OperableSentenceFactory

Next, you'll also find `OperableSentenceFactory` in `SentenceCreation`. It works similarly to `SentenceFactory`, but lets you use operators:

```csharp
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

..

// Helper methods for your predicates (and functions) are recommended:
OperablePredicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
OperablePredicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

..

var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, IsParent(G, P) & IsParent(P, C))));
```

Things to notice about this one:
* We've only used & (for a conjunction) here. | works for disjunctions, and ! for negations.
* The only proviso is that the supporting methods for domain specific elements now need to use `Operable..` as their return type - which is easy as these types are 
implicitly convertible from the normal equivalents.

### Writing Sentences with LanguageIntegration

Finally, there are the types to be found in the `LanguageIntegration` namespace. The `SentenceFactory` in this namespace is based on the idea of
modelling the domain as an IEnumerable&lt;T&gt;, then expressing our sentence as a LINQ expression. Like this:

```csharp
using SCFirstOrderLogic.LanguageIntegration;
using static SCFirstOrderLogic.LanguageIntegration.Operators; // Contains Iff an If methods..

..

// The helper methods recommended for the other approaches become full interfaces when language integration is used:
interface IPerson
{
    bool IsParentOf(IPerson person);
    bool IsGrandparentOf(IPerson person);
}

..

SentenceFactory.Create<IPerson>(d => d.All((g, c) => Iff(g.IsGrandparentOf(c), d.Any(p => g.IsParentOf(p) && p.IsParentOf(c)))));

```

Things to notice about this one:
* This is obviously non-trivial - more info can be found on the [language integration](./language-integration.md) page.

## Storing Knowledge and Making Inferences

Once you have some sentences, storing them and making inferences is done with the aid of the types in the `Inference` namespace.
The most important type here is the `IKnowledgeBase` interface. The library includes a few very simple knowledge bases - one that
uses forward chaining, one that uses backward chaining, and one that uses resolution. Some examples follow, but first, here's our domain,
taken from section 9.3 of 'Artificial Intelligence: A Modern Approach':

```csharp
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

..

Constant America = new Constant(nameof(America));
Constant Nono = new Constant(nameof(Nono));
Constant West = new Constant(nameof(West));

Predicate IsAmerican(Term t) => new Predicate(nameof(IsAmerican), t);
Predicate IsHostile(Term t) => new Predicate(nameof(IsHostile), t);
Predicate IsCriminal(Term t) => new Predicate(nameof(IsCriminal), t);
Predicate IsWeapon(Term t) => new Predicate(nameof(IsWeapon), t);
Predicate IsMissile(Term t) => new Predicate(nameof(IsMissile), t);
Predicate Owns(Term owner, Term owned) => new Predicate(nameof(Owns), owner, owned);
Predicate Sells(Term seller, Term item, Term buyer) => new Predicate(nameof(Sells), seller, item, buyer);
Predicate IsEnemyOf(Term t, Term other) => new Predicate(nameof(IsEnemyOf), t, other);

..

var rules = new Sentence[]
{
    // "... it is a crime for an American to sell weapons to hostile nations":
    // American(x) ∧ Weapon(y) ∧ Sells(x, y, z) ∧ Hostile(z) ⇒ Criminal(x).
    ForAll(X, Y, Z, If(And(IsAmerican(X), IsWeapon(Y), Sells(X, Y, Z), IsHostile(Z)), IsCriminal(X))),

    // "Nono... has some missiles."
    // ∃x Missile(x) ∧ Owns(Nono, x)
    ThereExists(X, And(IsMissile(X), Owns(Nono, X))),

    // "All of its missiles were sold to it by Colonel West":
    // Missile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)
    ForAll(X, If(And(IsMissile(X), Owns(Nono, X)), Sells(West, X, Nono))),

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

Using forward chaining:

```csharp
using SCFirstOrderLogic.Inference.Chaining;

var kb = new SimpleForwardChainingKnowledgeBase();
kb.Tell(rules);
var result = kb.Ask(IsCriminal(West)); // will be true

// Or, to get an explanation:
var query = kb.CreateQuery(IsCriminal(West));
query.Execute();
Console.WriteLine(query.Result); // true
Console.WriteLine(query.ResultExplanation) // A human-readable walkthrough of the proof tree 
```

Using backward chaining:

```csharp
using SCFirstOrderLogic.Inference.Chaining;

var kb = new SimpleBackwardChainingKnowledgeBase();
kb.Tell(rules);
var result = kb.Ask(IsCriminal(West)); // will be true
// ..Or can get an explanation in the same way as above
```

Using resolution:

```csharp
using SCFirstOrderLogic.Inference.Resolution;

var kb = new new SimpleResolutionKnowledgeBase(
    new SimpleClauseStore(),
    SimpleResolutionKnowledgeBase.Filters.None,
    SimpleResolutionKnowledgeBase.PriorityComparisons.UnitPreference);

kb.Tell(rules);
var result = kb.Ask(IsCriminal(West)); // will be true
// ..Or can get an explanation in the same way as above
```

Some things to note:
* The `Tell`, `Ask`, `CreateQuery` and `Execute` methods used above are actually extension methods that are are synchronous wrappers around underlying async versions. The library has deep async support - because "real-world" KBs will tend to need to do IO. At the time of writing, the only implementation that currently supports this meaningfully is the resolution one, though.

## Examples

For some usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.

## Beyond Getting Started

There are a number of things we've not touched on here, but are worth noting:

* **Equality:** The top-level namespace `SCFirstOrderLogic` includes [`EqualitySymbol`](../../src/SCFirstOrderLogic/EqualitySymbol.cs), intended to be used as the symbol for the equality predicate.
The various sentence creation methods make use of this in created sentences where appropriate.
None of the knowledge bases here use particular techniques (e.g. demodulation) to handle equality. However, the `Inference` namespace does include [`EqualityAxiomisingKnowledgeBase`](../../src/SCFirstOrderLogic/Inference/EqualityAxiomisingKnowledgeBase.cs), which is a decorator applied to an inner knowledge base - and adds rules pertaining to equality as knowledge is added.
* **Sentence Formatting:** There is some sentence formatting logic to be found in the `SentenceFormatting` namespace - which includes support for ensuring unique labelling of symbols for standardised variables and Skolem functions across a set of sentences.
The sets of labels used can be specified by the caller (but defaults do exist).
* **Sentence Manipulation and CNF:** While it wasn't explicity mentioned above, the knowledge bases referenced above do of course make use of conjunctive normal form where appropriate. Classes for conversion to and representation of CNF can be found in the `SentenceManipulation` namespace, alongside interfaces and base classes for sentence visitor logic