using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SCFirstOrderLogic.SentenceManipulation.ConjunctiveNormalForm
{
    public class CNFLiteralUnifier
    {
        public bool TryUnify(CNFLiteral x, CNFLiteral y, [NotNullWhen(returnValue: true)] out IDictionary<VariableReference, Term>? unifier)
        {
            unifier = new Dictionary<VariableReference, Term>();

            if (!TryUnify(x, y, unifier))
            {
                unifier = null;
                return false;
            }

            return true;
        }

        private bool TryUnify(CNFLiteral x, CNFLiteral y, IDictionary<VariableReference, Term> unifier)
        {
            if (x.IsNegated != y.IsNegated || !x.Predicate.Symbol.Equals(y.Predicate.Symbol))
            {
                return false;
            }

            foreach (var args in x.Predicate.Arguments.Zip(y.Predicate.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private bool TryUnify(Term x, Term y, IDictionary<VariableReference, Term> unifier)
        {
            return (x, y) switch
            {
                (VariableReference variable, _) => TryUnify(variable, y, unifier),
                (_, VariableReference variable) => TryUnify(variable, x, unifier),
                (Function functionX, Function functionY) => TryUnify(functionX, functionY, unifier),
                _ => x.Equals(y), // only potential for equality is if they're both constants. Worth being explicit?
            };
        }

        private bool TryUnify(VariableReference variable, Term other, IDictionary<VariableReference, Term> unifier)
        {
            if (unifier.TryGetValue(variable, out var value))
            {
                return TryUnify(value, other, unifier);
            }
            else if (other is VariableReference otherVariable && unifier.TryGetValue(otherVariable, out value))
            {
                return TryUnify(variable, value, unifier);
            }
            else if (Occurs(variable, other))
            {
                return false;
            }
            else
            {
                unifier[variable] = other;
                return true;
            }
        }

        private bool TryUnify(Function x, Function y, IDictionary<VariableReference, Term> unifier)
        {
            if (!x.Symbol.Equals(y.Symbol))
            {
                return false;
            }

            foreach (var args in x.Arguments.Zip(y.Arguments, (x, y) => (x, y)))
            {
                if (!TryUnify(args.x, args.y, unifier))
                {
                    return false;
                }
            }

            return true;
        }

        private bool Occurs(VariableReference variable, Term term)
        {
            var finder = new VariableFinder(variable); // TODO-PERFORMANCE: GC impact when creating a bunch of these.. Mutability and pooling?
            finder.ApplyTo(term);
            return finder.IsFound;
        }

        private class VariableFinder : SentenceTransformation
        {
            private readonly VariableReference variableReference;

            public VariableFinder(VariableReference variableReference) => this.variableReference = variableReference;

            public bool IsFound { get; private set; } = false;

            // TODO-PERFORMANCE: For performance, should probably override everything and stop as soon as IsFound is true.
            // And/or establish visitor pattern to make this easier..

            protected override Term ApplyTo(VariableReference variable)
            {
                if (variable.Equals(variableReference))
                {
                    IsFound = true;
                }

                return variable;
            }
        }
    }
}
