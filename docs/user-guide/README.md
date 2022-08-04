# SCFirstOrderLogic User Guide

## Overview

First and foremost, here is a quick overview of the namespaces found within this library:
 
* **`SCFirstOrderLogic`:** the root namespace contains classes representing individual elements of first order logic sentences ([universal](../../src/SCFirstOrderLogic/UniversalQuantification.cs) and [existential](../../src/SCFirstOrderLogic/ExistentialQuantification.cs) quantifications, [conjunctions](../../src/SCFirstOrderLogic/Conjunction.cs), [disjunctions](../../src/SCFirstOrderLogic/Disjunction.cs), [predicates](../../src/SCFirstOrderLogic/Predicate.cs), [functions](../../src/SCFirstOrderLogic/Function.cs) and so on).
Instances of these classes are composed into tree structures that represent sentences of first order logic.
  * **`Inference`:** intended as the top-level namespace for actual inference algorithms. Directly contains an [interface for knowledge bases](../../src/SCFirstOrderLogic/Inference/IKnowledgeBase.cs) (stores of knowledge that can callers can `Tell` and `Ask` things).
    * **`Chaining`:** Contains implementations of the knowledge base interface that use very simple forms of forward and backward chaining - as well as some supporting types.
    * **`Resolution`:** Contains an implementation of the knowledge base interface that uses a very simple form of [resolution](https://en.wikipedia.org/wiki/Resolution_(logic)) - as well as some supporting types. These supporting types include an interface for clause stores, and a very simple implementation that just stores clauses in a list.
    * **`Unification`:** Utility logic for [unifying](https://en.wikipedia.org/wiki/Unification_(computer_science)) literals.
  * **`LanguageIntegration`:** contains classes to create FoL sentences from LINQ expressions (i.e. allowing sentences to be provided as lambda expressions). For details, see the [language integration](./language-integration.md) page.
  * **`SentenceCreation`:** Logic to ease the creation of sentences - these serve as an alternative to using language integration. Contains the [`SentenceFactory`](../../src/SCFirstOrderLogic/SentenceCreation/SentenceFactory.cs) and [`OperableSentenceFactory`](../../src/SCFirstOrderLogic/SentenceCreation/OperableSentenceFactory.cs) static classes, which both include a number of shorthand static helper methods for instantiating sentences.
  * **`SentenceFormatting`:** Logic for creating string representations of sentences. Includes logic for ensuring unique labelling of standardised variables and Skolem functions (which the symbol representations themselves don't concern themselves with).
  * **`SentenceManipulation`:** Assorted logic for the manipulation of sentences. Contains some interfaces and base classes for sentence visitors and transformations. Also contains classes for conversion to and representation of [conjunctive normal form](https://en.wikipedia.org/wiki/Conjunctive_normal_form).

## Getting Started

NB: This guide assumes some familiarity with first-order logic. It won't explain first-order logic concepts - it'll just explain how to use FoL via this library.

### Writing Sentences

Obviously the first challenge is to write sentences. This can be done in several ways. This section will use the following sentence as an example:

```
∀ g, c, [IsGrandparentOf(g, c) ⇔ ∃ p, IsParentOf(g, p) ∧ IsParentOf(p, c)]
```

..that is, the definition of a grandparent. We use this example because its very straightforward but includes a good cross-section of FoL elements.

The most basic way to express this is to manually compose a bunch of instances of the types found in the top-level `SCFirstOrderLogic` namespace, like this:

```
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
First, we have the `SentenceFactory` static class in the `SentenceCreation` namespace. Here's how the example looks with this one:

```
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
* The factory provides methods for conjunctions (`And`). Disjunctions and negations also. See the next two examples if you really want to use C# operators.
* The supporting methods here are the recommended approach for Predicates (as well as Functions and Constants).

Next, you'll also find `OperableSentenceFactory` in `SentenceCreation`. It works similarly to `SentenceFactory`, but lets you use operators:

```
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

Finally, there are the types to be found in the `LanguageIntegration` namespace. The `SentenceFactory` in this namespace is based on the idea of
modelling the domain as an IEnumerable<T>, then expressing our sentence as a LINQ expression. Like this:

```
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

### Storing Knowledge and Making Inferences

..

### Examples

For initial usage examples, see the [example domains](../../src/SCFirstOrderLogic.ExampleDomains) project (and, to a lesser extent, the [tests](../../src/SCFirstOrderLogic.Tests)).
Beyond that, see the XML documentation against the classes - which I hope is fairly decent.
