// Copyright (c) 2021-2025 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Diagnostics.CodeAnalysis;
using System.Linq;

// TODO-BREAKING: I'm beginning to think that Substitution would be a better namespace name than VariableManipulation
namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// <para>
/// Unification logic that is "one-way" - only allows variables present in one of the arguments (the "generalisation") to be substituted.
/// </para>
/// <para>
/// NB: Lack of occurs check means this can give substitutions that would infinitely loop if applied (see unit tests for an example).
/// As it stands, no substitutions created by this class are actually ever applied, so this isnt a problem - but this is why the type is internal.
/// </para>
/// </summary>
internal static class InstanceUnifier
{
    /// <summary>
    /// Attempts to create a unifier for a literal and an instance of that literal.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to create a unifier for.</param>
    /// <param name="instance">The instance to attempt to create a unifier for.</param>
    /// <param name="unifier">If the literals can be unified, this out parameter will be the unifier.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryCreate(Literal generalisation, Literal instance, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateUnsafe(generalisation, instance, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a unifier for a literal and an instance of that literal.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to create a unifier for.</param>
    /// <param name="instance">The instance to attempt to create a unifier for.</param>
    /// <returns>The unifier if the literals can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryCreate(Literal generalisation, Literal instance)
    {
        var unifierAttempt = new MutableVariableSubstitution();
        return TryUpdateUnsafe(generalisation, instance, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to create a unifier for a predicate and an instance of that predicate.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to create a unifier for.</param>
    /// <param name="instance">The instance to attempt to create a unifier for.</param>
    /// <param name="unifier">If the predicates can be unified, this out parameter will be the unifier.</param>
    /// <returns>True if the two predicates can be unified, otherwise false.</returns>
    public static bool TryCreate(Predicate generalisation, Predicate instance, [MaybeNullWhen(returnValue: false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateUnsafe(generalisation, instance, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a unifier for a predicate and an instance of that predicate.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to create a unifier for.</param>
    /// <param name="instance">The instance to attempt to create a unifier for.</param>
    /// <returns>The unifier if the predicates can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryCreate(Predicate generalisation, Predicate instance)
    {
        var unifierAttempt = new MutableVariableSubstitution();
        return TryUpdateUnsafe(generalisation, instance, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to create a unifier for a term and an instance of that term.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to create a unifier for.</param>
    /// <param name="instance">The instance to attempt to create a unifier for.</param>
    /// <param name="unifier">If the terms can be unified, this out parameter will be the unifier.</param>
    /// <returns>True if the two terms can be unified, otherwise false.</returns>
    public static bool TryCreate(Term generalisation, Term instance, [MaybeNullWhen(false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateUnsafe(generalisation, instance, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to create a unifier for a term and an instance of that term.
    /// </summary>
    /// <param name="generalisation">One of the two terms to attempt to create a unifier for.</param>
    /// <param name="instance">One of the two terms to attempt to create a unifier for.</param>
    /// <returns>The unifier if the terms can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryCreate(Term generalisation, Term instance)
    {
        var unifierAttempt = new MutableVariableSubstitution();
        return TryUpdateUnsafe(generalisation, instance, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given literal and instance of that literal.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <param name="updatedUnifier">Will be populated with the updated unifier on success, or be null on failure.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryUpdate(Literal generalisation, Literal instance, VariableSubstitution unifier, [MaybeNullWhen(false)] out VariableSubstitution updatedUnifier)
    {
        var potentialUpdatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(generalisation, instance, potentialUpdatedUnifier))
        {
            updatedUnifier = potentialUpdatedUnifier.CopyAsReadOnly();
            return true;
        }

        updatedUnifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given literal and instance of that literal.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryUpdate(Literal generalisation, Literal instance, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(generalisation, instance, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given literal and instance of that literal.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <returns>The unifier if the literals can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryUpdate(Literal generalisation, Literal instance, VariableSubstitution unifier)
    {
        var unifierAttempt = unifier.CopyAsMutable();
        return TryUpdateUnsafe(generalisation, instance, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given predicate and instance of that predicate.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <param name="updatedUnifier">Will be populated with the updated unifier on success, or be null on failure.</param>
    /// <returns>True if the two predicates can be unified, otherwise false.</returns>
    public static bool TryUpdate(Predicate generalisation, Predicate instance, VariableSubstitution unifier, [MaybeNullWhen(false)] out VariableSubstitution updatedUnifier)
    {
        var potentialUpdatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(generalisation, instance, potentialUpdatedUnifier))
        {
            updatedUnifier = potentialUpdatedUnifier.CopyAsReadOnly();
            return true;
        }

        updatedUnifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given predicate and instance of that predicate.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two predicates can be unified, otherwise false.</returns>
    public static bool TryUpdate(Predicate generalisation, Predicate instance, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(generalisation, instance, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given predicate and instance of that predicate.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <returns>The unifier if the predicates can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryUpdate(Predicate generalisation, Predicate instance, VariableSubstitution unifier)
    {
        var unifierAttempt = unifier.CopyAsMutable();
        return TryUpdateUnsafe(generalisation, instance, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given term and instance of that term.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <param name="updatedUnifier">Will be populated with the updated unifier on success, or be null on failure.</param>
    /// <returns>True if the two terms can be unified, otherwise false.</returns>
    public static bool TryUpdate(Term generalisation, Term instance, VariableSubstitution unifier, [MaybeNullWhen(false)] out VariableSubstitution updatedUnifier)
    {
        var potentialUpdatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(generalisation, instance, potentialUpdatedUnifier))
        {
            updatedUnifier = potentialUpdatedUnifier.CopyAsReadOnly();
            return true;
        }

        updatedUnifier = null;
        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given term and instance of that term.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two terms can be unified, otherwise false.</returns>
    public static bool TryUpdate(Term generalisation, Term instance, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateUnsafe(generalisation, instance, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies a given term and instance of that term.
    /// </summary>
    /// <param name="generalisation">The generalisation to attempt to unify.</param>
    /// <param name="instance">The instance to attempt to unify.</param>
    /// <param name="unifier">The unifier to update.</param>
    /// <returns>The unifier if the literals can be unified, otherwise null.</returns>
    public static VariableSubstitution? TryUpdate(Term generalisation, Term instance, VariableSubstitution unifier)
    {
        var unifierAttempt = unifier.CopyAsMutable();
        return TryUpdateUnsafe(generalisation, instance, unifierAttempt) ? unifierAttempt.CopyAsReadOnly() : null;
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Literal generalisation, Literal instance, MutableVariableSubstitution unifier)
    {
        if (generalisation.IsNegated != instance.IsNegated)
        {
            return false;
        }

        return TryUpdateUnsafe(generalisation.Predicate, instance.Predicate, unifier);
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Predicate generalisation, Predicate instance, MutableVariableSubstitution unifier)
    {
        if (!generalisation.Identifier.Equals(instance.Identifier)
            || generalisation.Arguments.Count != instance.Arguments.Count)
        {
            return false;
        }

        foreach (var args in generalisation.Arguments.Zip(instance.Arguments, (x, y) => (x, y)))
        {
            if (!TryUpdateUnsafe(args.x, args.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Term generalisation, Term instance, MutableVariableSubstitution unifier)
    {
        return (generalisation, instance) switch
        {
            (VariableReference variable, _) => TryUpdateUnsafe(variable, instance, unifier),
            (Function functionX, Function functionY) => TryUpdateUnsafe(functionX, functionY, unifier),
            // Otherwise we must have (Function, Variable) which can't be unified in this case
            _ => false,
        };
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(VariableReference generalisationVariable, Term instanceTerm, MutableVariableSubstitution unifier)
    {
        if (unifier.Bindings.TryGetValue(generalisationVariable, out var variableValue))
        {
            return variableValue.Equals(instanceTerm);
        }
        else
        {
            unifier.AddBinding(generalisationVariable, instanceTerm);
            return true;
        }
    }

    // NB: 'unsafe' in that it can partially update the unifier on failure
    private static bool TryUpdateUnsafe(Function generalisation, Function instance, MutableVariableSubstitution unifier)
    {
        if (!generalisation.Identifier.Equals(instance.Identifier) || generalisation.Arguments.Count != instance.Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < generalisation.Arguments.Count; i++)
        {
            if (!TryUpdateUnsafe(generalisation.Arguments[i], instance.Arguments[i], unifier))
            {
                return false;
            }
        }

        return true;
    }
}
