using SCFirstOrderLogic.SentenceManipulation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of an predicate sentence of first order logic, In typical FOL syntax, this is written as:
    /// <code>{predicate symbol}({term}, ..)</code>
    /// </summary>
    public sealed class Predicate : Sentence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="symbol">
        /// An object representing the symbol of the predicate.
        /// <para/>
        /// Symbol equality should indicate that it is the same predicate in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// <para/>
        /// NB: Yes, we *could* declare an ISymbol interface that is IEquatable&lt;ISymbol&gt; and defines a
        /// string-returning 'Render' method. However, given that the only things we need of a symbol are
        /// equatability and the ability to convert them to a string, and both of these things are possible with the
        /// object base class, for now at least we err on the side of simplicity and say that symbols can be any object.
        /// </param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Predicate(object symbol, params Term[] arguments)
            : this(symbol, (IList<Term>)arguments)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Predicate"/> class.
        /// </summary>
        /// <param name="symbol">
        /// An object representing the symbol of the predicate.
        /// <para/>
        /// Symbol equality should indicate that it is the same predicate in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// <para/>
        /// NB: Yes, we *could* declare an ISymbol interface that is IEquatable&lt;ISymbol&gt; and defines a
        /// string-returning 'Render' method. However, given that the only things we need of a symbol are
        /// equatability and the ability to convert them to a string, and both of these things are possible with the
        /// object base class, for now at least we err on the side of simplicity and say that symbols can be any object.
        /// </param>
        /// <param name="arguments">The arguments of this predicate.</param>
        public Predicate(object symbol, IList<Term> arguments)
        {
            Symbol = symbol;
            Arguments = new ReadOnlyCollection<Term>(arguments);
        }

        /// <summary>
        /// Gets the arguments of this predicate.
        /// </summary>
        public ReadOnlyCollection<Term> Arguments { get; }

        /// <summary>
        /// Gets an object representing the symbol of the predicate.
        /// <para/>
        /// Symbol equality should indicate that it is the same predicate in the domain.
        /// ToString of the Symbol should be appropriate for rendering in FoL syntax.
        /// <para/>
        /// Symbol is not a string to avoid problems caused by clashing symbols. By allowing other types
        /// we allow for equality logic that includes a type check, and thus the complete preclusion of clashes.
        /// <para/>
        /// NB: Yes, we *could* declare an ISymbol interface that is IEquatable&lt;ISymbol&gt; and defines a
        /// string-returning 'Render' method. However, given that the only things we need of a symbol are
        /// equatability and the ability to convert them to a string, and both of these things are possible with the
        /// object base class, for now at least we err on the side of simplicity and say that symbols can be any object.
        /// <para/>
        /// Perhaps should be called 'Identifier'..
        /// </summary>
        public object Symbol { get; }

        /// <inheritdoc />
        public override void Accept(ISentenceVisitor visitor) => visitor.Visit(this);

        /// <inheritdoc />
        public override void Accept<T>(ISentenceVisitor<T> visitor, T state) => visitor.Visit(this, state);

        /// <inheritdoc />
        public override void Accept<T>(ISentenceVisitorR<T> visitor, ref T state) => visitor.Visit(this, ref state);

        /// <inheritdoc />
        public override TOut Accept<TOut>(ISentenceTransformation<TOut> transformation) => transformation.ApplyTo(this);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is not Predicate otherPredicate
                || !otherPredicate.Symbol.Equals(Symbol)
                || otherPredicate.Arguments.Count != Arguments.Count)
            {
                return false;
            }

            for (int i = 0; i < Arguments.Count; i++)
            {
                if (!Arguments[i].Equals(otherPredicate.Arguments[i]))
                {
                    return false;
                }
            }

            return true;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = new HashCode();

            hashCode.Add(Symbol);
            foreach (var argument in Arguments)
            {
                hashCode.Add(argument);
            }

            return hashCode.ToHashCode();
        }
    }
}
