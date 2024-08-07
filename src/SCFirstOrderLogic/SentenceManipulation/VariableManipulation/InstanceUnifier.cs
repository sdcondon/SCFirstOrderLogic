﻿using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.VariableManipulation;

/// <summary>
/// Unification logic that is "one-way" - only allows variables present in one of the arguments (the "generalisation") to be substituted.
/// </summary>
internal static class InstanceUnifier
{
    /// <summary>
    /// Attempts to update a unifier so that it (also) unifies two given literals.
    /// </summary>
    /// <param name="generalisation">One of the two literals to attempt to unify.</param>
    /// <param name="instance">One of the two literals to attempt to unify.</param>
    /// <param name="unifier">The unifier to update. Will be updated to refer to a new unifier on success, or be unchanged on failure.</param>
    /// <returns>True if the two literals can be unified, otherwise false.</returns>
    public static bool TryUpdate(Literal generalisation, Literal instance, ref VariableSubstitution unifier)
    {
        var updatedUnifier = unifier.CopyAsMutable();

        if (TryUpdateInPlace(generalisation, instance, updatedUnifier))
        {
            unifier = updatedUnifier.CopyAsReadOnly();
            return true;
        }

        return false;
    }

    public static bool TryCreate(Term generalisation, Term instance, [MaybeNullWhen(false)] out VariableSubstitution unifier)
    {
        var unifierAttempt = new MutableVariableSubstitution();

        if (TryUpdateInPlace(generalisation, instance, unifierAttempt))
        {
            unifier = unifierAttempt.CopyAsReadOnly();
            return true;
        }

        unifier = null;
        return false;
    }

    private static bool TryUpdateInPlace(Literal generalisation, Literal instance, MutableVariableSubstitution unifier)
    {
        if (generalisation.IsNegated != instance.IsNegated
            || !generalisation.Predicate.Identifier.Equals(instance.Predicate.Identifier)
            || generalisation.Predicate.Arguments.Count != instance.Predicate.Arguments.Count)
        {
            return false;
        }

        foreach (var args in generalisation.Predicate.Arguments.Zip(instance.Predicate.Arguments, (x, y) => (x, y)))
        {
            if (!TryUpdateInPlace(args.x, args.y, unifier))
            {
                return false;
            }
        }

        return true;
    }

    private static bool TryUpdateInPlace(Term generalisation, Term instance, MutableVariableSubstitution unifier)
    {
        return (generalisation, instance) switch
        {
            (VariableReference variable, _) => TryUpdateInPlace(variable, instance, unifier),
            (Function functionX, Function functionY) => TryUpdateInPlace(functionX, functionY, unifier),
            // Otherwise we must have (Function, Variable) which can't be unified in this case
            _ => false,
        };
    }

    private static bool TryUpdateInPlace(VariableReference variable, Term instanceTerm, MutableVariableSubstitution unifier)
    {
        if (variable.Equals(instanceTerm))
        {
            return true;
        }
        else if (unifier.Bindings.TryGetValue(variable, out var variableValue))
        {
            // The variable is already mapped to something - we need to make sure that the
            // mapping is consistent with the "other" value.
            return TryUpdateInPlace(variableValue, instanceTerm, unifier);
        }
        else
        {
            unifier.AddBinding(variable, instanceTerm);
            return true;
        }
    }

    private static bool TryUpdateInPlace(Function x, Function y, MutableVariableSubstitution unifier)
    {
        if (!x.Identifier.Equals(y.Identifier) || x.Arguments.Count != y.Arguments.Count)
        {
            return false;
        }

        for (int i = 0; i < x.Arguments.Count; i++)
        {
            if (!TryUpdateInPlace(x.Arguments[i], y.Arguments[i], unifier))
            {
                return false;
            }
        }

        return true;
    }
}
