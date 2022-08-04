# Getting Started

NB: This guide assumes some familiarity with first-order logic. It won't explain first-order logic concepts - it'll just explain how to use FoL via this library.

## Writing Sentences

Obviously the first challenge is to write sentences. This can be done in several ways. This section will use the following sentence as an example:

```
∀ g, c, [IsGrandparentOf(g, c) ⇔ ∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]
```

..that is, the definition of a grandparent. We use this example because its very straightforward but includes a good cross-section of FoL elements.

### Writing Sentences Manually

The most basic way to express this is to manually compose a bunch of instances of the types found in the top-level `SCFirstOrderLogic` namespace, like this:

```csharp
using static SCFirstOrderLogic;

..

Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

..

var g = new VariableDefinition("g");
var c = new VariableDefinition("c");
var p = new VariableDefinition("p");
var equivalenceLHS = IsGrandparent(g, c);
var equivalenceRHS = new ExistentialQuantification(p, new Conjunction(IsParent(g, p), IsParent(p, c)));
Sentence grandparentDefn = new UniversalQuantification(g, new UniversalQuantification(c, new Equivalence(equivalenceLHS, equivalenceRHS));
```

Things to notice:
* This is very direct, but obviously far too verbose to be workable. Hence the alternatives we discuss below.

### Writing Sentences with SentenceFactory

First, we have the `SentenceFactory` static class in the `SentenceCreation` namespace. Here's how the example looks with this one:

```csharp
using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

..

Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

..

var grandparentDefn = ForAll(G, C, Iff(IsGrandparent(G, C), ThereExists(P, And(IsParent(G, P), IsParent(P, C)))));
```

Things to notice about this one:
* The factory provides ForAll and ThereExists methods for creating quantifications. There are overloads for declaring multiple variables at once.
* The factory provides `A` through `Z` as properties that return variable definitions with these letters as their symbol.
* The factory provides methods for conjunctions (`And`). Disjunctions (`Or`) and negations (`Not`) also. See the next two examples if you really want to use C# operators for these.
* The supporting methods here are the recommended approach for Predicates (similarly for Functions and Constants).

### Writing Sentences with OperableSentenceFactory

Next, you'll also find `OperableSentenceFactory` in `SentenceCreation`. It works similarly to `SentenceFactory`, but lets you use operators:

```csharp
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

..

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

interface IPerson
{
    bool IsParentOf(IPerson person);
    bool IsGrandparentOf(IPerson person);
}

..

SentenceFactory.Create<IPerson>(d => d.All((g, c) => Iff(g.IsGrandparentOf(c), d.Any(p => g.IsParentOf(p) && p.IsParentOf(c)))));

```

Things to notice about this one:
* This is obviously non trivial - more info can be found on the [language integration](./language-integration.md) page.

## Storing Knowledge and Making Inferences

Once you have some sentences, storing them and making inferences is done with the aid of the types in the `Inference` namespace.
The lowest common denominator here is the `IKnowledgeBase` interface. The library includes a few very simple knowledge bases - one that
uses forward chaining, one that uses backward chaining, and one that uses resolution. Some examples follow, but first, here's our domain,
taken from section 9.3 of 'Artificial Intelligence: A Modern Approach':

```csharp
using static SCFirstOrderLogic.SentenceCreation.OperableSentenceFactory;

..

OperableConstant America = new Constant(nameof(America));
OperableConstant Nono = new Constant(nameof(Nono));
OperableConstant West = new Constant(nameof(West));

OperablePredicate IsAmerican(Term t) => new Predicate(nameof(IsAmerican), t);
OperablePredicate IsHostile(Term t) => new Predicate(nameof(IsHostile), t);
OperablePredicate IsCriminal(Term t) => new Predicate(nameof(IsCriminal), t);
OperablePredicate IsWeapon(Term t) => new Predicate(nameof(IsWeapon), t);
OperablePredicate IsMissile(Term t) => new Predicate(nameof(IsMissile), t);
OperablePredicate Owns(Term owner, Term owned) => new Predicate(nameof(Owns), owner, owned);
OperablePredicate Sells(Term seller, Term item, Term buyer) => new Predicate(nameof(Sells), seller, item, buyer);
OperablePredicate IsEnemyOf(Term t, Term other) => new Predicate(nameof(IsEnemyOf), t, other);

..

var axioms = new List<Sentence>()
{
    // "... it is a crime for an American to sell weapons to hostile nations":
    // American(x) ∧ Weapon(y) ∧ Sells(x, y, z) ∧ Hostile(z) ⇒ Criminal(x).
    ForAll(X, Y, Z, If(IsAmerican(X) & IsWeapon(Y) & Sells(X, Y, Z) & IsHostile(Z), IsCriminal(X))),

    // "Nono... has some missiles."
    // ∃x IsMissile(x) ∧ Owns(Nono, x)
    ThereExists(X, IsMissile(X) & Owns(Nono, X)),

    // "All of its missiles were sold to it by Colonel West":
    // Missile(x) ∧ Owns(Nono, x) ⇒ Sells(West, x, Nono)
    ForAll(X, If(IsMissile(X) & Owns(Nono, X), Sells(West, X, Nono))),

    // We will also need to know that missiles are weapons: 
    ForAll(X, If(IsMissile(X), IsWeapon(X))),

    // And we must know that an enemy of America counts as “hostile”:
    // Enemy(x, America) ⇒ Hostile(x)
    ForAll(X, If(IsEnemyOf(X, America), IsHostile(X))),

    // "West, who is American..": American(West)
    IsAmerican(West),

    // "The country Nono, an enemy of America..": Enemy(Nono, America).
    IsEnemyOf(Nono, America),

}.AsReadOnly();

```

Using forward chaining:

```csharp
using SCFirstOrderLogic.Inference.Chaining;

var kb = new SimpleForwardChainingKnowledgeBase();
kb.Tell(axioms);
var result = kb.Ask(IsCriminal(West)); // will be true
```

Using backward chaining:

```csharp
using SCFirstOrderLogic.Inference.Chaining;

var kb = new SimpleBackwardChainingKnowledgeBase();
kb.Tell(axioms);
var result = kb.Ask(IsCriminal(West)); // will be true
```

Using resolution:

```csharp
using SCFirstOrderLogic.Inference.Chaining;

var kb = new SimpleBackwardChainingKnowledgeBase();
kb.Tell(axioms);
var result = kb.Ask(IsCriminal(West)); // will be true
```

Some things to note:
* The synchronous `Tell` and `Ask` methods used above are actually extension methods. The library has deep async support - because "real-world" KBs will tend to need to do IO. At the time of writing, the only implementation that currently supports this meaningfully is the resolution one, though.


## Examples

For initial usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.
