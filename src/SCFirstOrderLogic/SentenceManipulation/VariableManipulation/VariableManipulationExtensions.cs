// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// Useful extension methods for variable manipulation and inspection.
/// </summary>
public static class VariableManipulationExtensions
{
    /// <summary>
    /// <para>
    /// Gets a value indicating whether 'this' clause subsumes another.
    /// </para>
    /// <para>
    /// That is, whether the other clause is logically entailed by this one.
    /// </para>
    /// </summary>
    /// <param name="thisClause"></param>
    /// <param name="otherClause"></param>
    /// <returns>True if this clause subsumes the other; otherwise false.</returns>
    public static bool Subsumes(this CNFClause thisClause, CNFClause otherClause)
    {
        if (thisClause.IsEmpty)
        {
            return false;
        }

        VariableSubstitution substitution = new();

        foreach (var literal in thisClause.Literals)
        {
            if (!otherClause.Literals.Any(l => InstanceUnifier.TryUpdate(literal, l, ref substitution)))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// <para>
    /// Gets a value indicating whether another clause subsumes this one.
    /// </para>
    /// <para>
    /// That is, whether this clause is logically entailed by the other.
    /// </para>
    /// </summary>
    /// <param name="thisClause"></param>
    /// <param name="otherClause"></param>
    /// <returns>True if the other clause subsumes this one; otherwise false.</returns>
    public static bool IsSubsumedBy(this CNFClause thisClause, CNFClause otherClause)
    {
        return otherClause.Subsumes(thisClause);
    }

    /// <summary>
    /// <para>
    /// Checks whether "this" clause unifies with any of an enumeration of other definite clauses.
    /// </para>
    /// </summary>
    /// <param name="thisClause">"This" clause.</param>
    /// <param name="clauses">The clauses to check for unification with.</param>
    /// <returns>True if this clause unifies with any of the provided clauses; otherwise false.</returns>
    // TODO: probably remove (/replace with SubsumesAnyOf/IsSubsumedByAnyOf?) - created prior to subsumption methods,
    // and its usage looks plain wrong now that I've a bit more FoL experience under my belt..
    public static bool UnifiesWithAnyOf(this CNFClause thisClause, IEnumerable<CNFClause> clauses)
    {
        return clauses.Any(c => thisClause.TryUnifyWith(c));
    }

    /// <summary>
    /// <para>
    /// Ordinalises a sentence; replacing all variable identifiers with the (zero-based) ordinal of where they
    /// first appear in a depth-first traversal of the sentence.
    /// </para>
    /// <para>
    /// This is useful because it makes the original identifiers irrelevant but preserves distinctness, so that 
    /// e.g. P(X, X) is transformed to a sentence that is identical to the transformation of P(Y, Y), but different
    /// to the transformation of P(X, Y). Making the original identifier irrelevant is useful when e.g. indexing.
    /// </para>
    /// </summary>
    /// <param name="sentence">The sentence to ordinalise.</param>
    /// <returns>The ordinalised sentence.</returns>
    public static Sentence Ordinalise(this Sentence sentence)
    {
        return sentence.Accept(new VariableOrdinalisation());
    }

    /// <summary>
    /// <para>
    /// Ordinalises a literal; replacing all variable identifiers with the (zero-based) ordinal of where they
    /// first appear in a depth-first traversal of the literal.
    /// </para>
    /// <para>
    /// This is useful because it makes the original identifiers irrelevant but preserves distinctness, so that 
    /// e.g. P(X, X) is transformed to a literal that is identical to the transformation of P(Y, Y), but different
    /// to the transformation of P(X, Y). Making the original identifier irrelevant is useful when e.g. indexing.
    /// </para>
    /// </summary>
    /// <param name="literal">The literal to ordinalise.</param>
    /// <returns>The ordinalised literal.</returns>
    public static Literal Ordinalise(this Literal literal)
    {
        return new VariableOrdinalisation().ApplyTo(literal);
    }

    /// <summary>
    /// <para>
    /// Ordinalises a term; replacing all variable identifiers with the (zero-based) ordinal of where they
    /// first appear in a depth-first traversal of the term.
    /// </para>
    /// <para>
    /// This is useful because it makes the original identifiers irrelevant but preserves distinctness, so that 
    /// e.g. F(X, X) is transformed to a term that is identical to the transformation of F(Y, Y), but different
    /// to the transformation of F(X, Y). Making the original identifier irrelevant is useful when e.g. indexing.
    /// </para>
    /// </summary>
    /// <param name="term">The term to ordinalise.</param>
    /// <returns>The ordinalised term.</returns>
    public static Term Ordinalise(this Term term)
    {
        return term.Accept(new VariableOrdinalisation());
    }

    /// <summary>
    /// Checks whether this term is an instance of another.
    /// </summary>
    /// <param name="term">The potential instance.</param>
    /// <param name="generalisation">The generalisation.</param>
    /// <returns>A value indicating whether this term is an instance of the generalisation.</returns>
    public static bool IsInstanceOf(this Term term, Term generalisation)
    {
        return InstanceUnifier.TryCreate(generalisation, term, out _);
    }

    /// <summary>
    /// Checks whether this term is a generalisation of another.
    /// </summary>
    /// <param name="term">The potential generalisation.</param>
    /// <param name="instance">The instance.</param>
    /// <returns>A value indicating whether this term is an generalisation of the instance.</returns>
    public static bool IsGeneralisationOf(this Term term, Term instance)
    {
        return InstanceUnifier.TryCreate(term, instance, out _);
    }

    /// <summary>
    /// Tries to unify "this" clause with another.
    /// </summary>
    /// <param name="thisClause">"This" clause.</param>
    /// <param name="otherClause">The other clause.</param>
    /// <returns>True if the two clauses were successfully unified, otherwise false.</returns>
    private static bool TryUnifyWith(this CNFClause thisClause, CNFClause otherClause)
    {
        if (thisClause.Literals.Count != otherClause.Literals.Count)
        {
            return false;
        }

        return TryUnifyWith(thisClause.Literals, otherClause.Literals, new VariableSubstitution()).Any();
    }

    private static IEnumerable<VariableSubstitution> TryUnifyWith(IEnumerable<Literal> thisLiterals, IEnumerable<Literal> otherLiterals, VariableSubstitution unifier)
    {
        if (!thisLiterals.Any())
        {
            yield return unifier;
        }
        else
        {
            foreach (var otherLiteral in otherLiterals)
            {
                if (Unifier.TryUpdate(thisLiterals.First(), otherLiteral, unifier, out var firstLiteralUnifier))
                {
                    // TODO-PERFORMANCE: Ugh, skip is bad enough - Except is going to get slow, esp when nested. Important thing for now is that it works as a baseline..
                    foreach (var restOfLiteralsUnifier in TryUnifyWith(thisLiterals.Skip(1), otherLiterals.Except(new[] { otherLiteral }), firstLiteralUnifier))
                    {
                        yield return restOfLiteralsUnifier;
                    }
                }
            }
        }
    }

    /// <summary>
    /// <para>
    /// Sentence transformation that converts all variable identifiers to the integer value of their order in a depth-first traversal of the sentence.
    /// </para>
    /// <para>
    /// Useful in e.g. indexing, where we want to make the original identifier irrelevant when comparing sentences/clauses/terms.
    /// </para>
    /// </summary>
    private class VariableOrdinalisation : RecursiveSentenceTransformation
    {
        private readonly Dictionary<object, VariableDeclaration> variableIdMap = new();

        public Literal ApplyTo(Literal literal)
        {
            return new((Predicate)ApplyTo(literal.Predicate), literal.IsNegated);
        }

        /// <inheritdoc/>
        public override VariableDeclaration ApplyTo(VariableDeclaration variable)
        {
            if (!variableIdMap.TryGetValue(variable.Identifier, out var declaration))
            {
                declaration = variableIdMap[variable.Identifier] = new VariableDeclaration(variableIdMap.Count);
            }

            return declaration;
        }
    }
}
