# Getting Started

NB: This guide assumes some familiarity with first-order logic. It won't explain first-order logic concepts - it'll just explain how to use FoL via this library.

## Writing Sentences

Obviously the first challenge is to write sentences. This can be done in several ways. This section will use the following sentence as an example:

```
∀ g, c, [IsGrandparentOf(g, c) ⇔ ∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]
```

..that is, the definition of a grandparent. We use this example because its very straightforward but includes a good cross-section of FoL elements.

**Manual:** The most basic way to express this is to manually compose a bunch of instances of the types found in the top-level `SCFirstOrderLogic` namespace, like this:

```csharp
var g = new VariableDefinition("g");
var c = new VariableDefinition("c");
var p = new VariableDefinition("p");
Predicate IsGrandparent(Term grandparent, Term grandchild) => new Predicate(nameof(IsGrandparent), grandparent, grandchild);
Predicate IsParent(Term parent, Term child) => new Predicate(nameof(IsParent), parent, child);

var equivalenceLHS = IsGrandparent(g, c);
var equivalenceRHS = new ExistentialQuantification(p, new Conjunction(IsParent(g, p), IsParent(p, c)));
Sentence grandparentDefn = new UniversalQuantification(g, new UniversalQuantification(c, new Equivalence(equivalenceLHS, equivalenceRHS));
```

This is very direct, but obviously far too verbose to be workable. So there are a few alternatives.

**SentenceFactory:** First, we have the `SentenceFactory` static class in the `SentenceCreation` namespace. Here's how the example looks with this one:

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
* The factory provides methods for conjunctions (`And`). Disjunctions ('Or') and negations ('Not') also. See the next two examples if you really want to use C# operators for these.
* The supporting methods here are the recommended approach for Predicates (similarly for Functions and Constants).

**OperableSentenceFactory:** Next, you'll also find `OperableSentenceFactory` in `SentenceCreation`. It works similarly to `SentenceFactory`, but lets you use operators:

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

**LanguageIntegration:** Finally, there are the types to be found in the `LanguageIntegration` namespace. The `SentenceFactory` in this namespace is based on the idea of
modelling the domain as an IEnumerable<T>, then expressing our sentence as a LINQ expression. Like this:

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

..

## Examples

For initial usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.
