﻿using SCFirstOrderLogic.SentenceManipulation;

namespace SCFirstOrderLogic
{
    /// <summary>
    /// Representation of a term within a sentence of first order logic.
    /// </summary>
    public abstract class Term
    {
        // TODO: visitor pattern, perhaps..
        ////public abstract T Accept<T>(ISentenceVisitor<T> visitor);
        
        /// <summary>
        /// Gets a value indicating whether the term is a ground term - that is, that it contains no variables.
        /// </summary>
        public abstract bool IsGroundTerm { get; }

        /// <inheritdoc />
        public override string ToString() => SentenceFormatter.Print(this); // For now at least
    }
}