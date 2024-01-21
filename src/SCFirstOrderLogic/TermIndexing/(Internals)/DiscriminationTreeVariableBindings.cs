// Copyright (c) 2021-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System.Collections.Generic;
using System.Linq;

namespace SCFirstOrderLogic.TermIndexing;

internal class DiscriminationTreeVariableBindings
{
    // TODO-ZZ: does this need to be a dictionary - we should always encounter variables in ordinal order, i think?
    private readonly Dictionary<int, IDiscriminationTreeElementInfo[]> map;

    public DiscriminationTreeVariableBindings() => map = new();

    private DiscriminationTreeVariableBindings(IEnumerable<KeyValuePair<int, IDiscriminationTreeElementInfo[]>> content) => map = new(content);

    // Overall performance would perhaps be better with a tree instead of copying a dictionary
    // Worth a test at some point perhaps (maybe after looking at substitution trees in general).
    // Might not be worth the complexity though.
    public static bool TryAddOrMatchBinding(int ordinal, IDiscriminationTreeElementInfo[] value, ref DiscriminationTreeVariableBindings bindings)
    {
        // NB the "one-way" nature of this binding means this logic can be
        // simpler than that of unification (see LiteralUnifier) - here, we just need to check for equality.
        // This behaviour will need to CHANGE if we add unifier discovery logic to the tree.
        if (!bindings.map.TryGetValue(ordinal, out var existingBinding))
        {
            bindings = new DiscriminationTreeVariableBindings(bindings.map.Append(KeyValuePair.Create(ordinal, value)));
            return true;
        }

        if (existingBinding.Length != value.Length)
        {
            return false;
        }

        for (int i = 0; i < existingBinding.Length; i++)
        {
            if (!existingBinding[i].Equals(value[i]))
            {
                return false;
            }
        }

        return true;
    }
}
