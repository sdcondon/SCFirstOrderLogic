// Copyright 2022-2023 Simon Condon
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
using System.Collections.Generic;

namespace SCFirstOrderLogic.InternalUtilities
{
    /// <summary>
    /// Useful internal extension methods for <see cref="HashSet{T}"/> instances.
    /// </summary>
    internal static class HashSetExtensions
    {
        /// <summary>
        /// <para>
        /// Determines whether this set and the specified set contain the same elements.
        /// </para>
        /// <para>
        /// NB: In general, <see cref="ISet{T}.SetEquals(IEnumerable{T})"/> implementations have to handle the possibility that duplicates
        /// occur in the argument (note that even if the argument is another set, there is still the possibility that the equality
        /// comparers in use by each set differ). This necessarily slows it down. By accepting an hash set in this method and
        /// checking its key comparer, we can avoid a bit of work if the key comparers are the same object (as they will tend to be for the 
        /// sets in this library). Performance testing has shown this has a small but noticeable impact.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the elements held in both sets.</typeparam>
        /// <param name="thisSet">The current set.</param>
        /// <param name="otherSet">The other set.</param>
        /// <returns>True if and only if the two sets contain the same elements.</returns>
        public static bool SetEquals<T>(this HashSet<T> thisSet, HashSet<T> otherSet)
        {
            if (!object.ReferenceEquals(thisSet.Comparer, otherSet.Comparer))
            {
                // Fall back on the normal set equals implementation if the comparers aren't the exact same object.
                return thisSet.SetEquals(otherSet);
            }

            if (object.ReferenceEquals(thisSet, otherSet))
            {
                return true;
            }

            if (thisSet.Count != otherSet.Count)
            {
                return false;
            }

            foreach (var element in otherSet)
            {
                if (!thisSet.Contains(element))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
