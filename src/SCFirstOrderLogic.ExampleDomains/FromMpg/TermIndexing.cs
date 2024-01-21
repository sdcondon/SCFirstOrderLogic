using static SCFirstOrderLogic.SentenceCreation.SentenceFactory;

namespace SCFirstOrderLogic.ExampleDomains.FromMpg;

/// <summary>
/// Essentially a handy example for demonstrating and testing term indexing.
/// Note the <see cref="Terms"/> property, which contains a good selection of terms to fill out an
/// index with some content that goes at least a little beyond complete triviality.
/// See https://rg1-teaching.mpi-inf.mpg.de/autrea-ws19/script-6.2-7.4.pdf for the actual source.
/// </summary>
public static class TermIndexingExample
{
    public static readonly Constant A = new(nameof(A));
    public static readonly Constant B = new(nameof(B));
    public static readonly Constant C = new(nameof(C));
    public static readonly Constant D = new(nameof(D));

    public static Function F(Term x, Term y) => new(nameof(F), x, y);
    public static Function G(Term x, Term y) => new(nameof(G), x, y);
    public static Function H(Term x) => new(nameof(H), x);

    
    public static readonly Term[] ExampleTerms =
    {
        F(G(D, X), C),
        F(X, C),
        F(X, G(C, B)),
        G(B, H(C)),
        F(G(X, C), C),
        F(B, G(C, B)),
        F(B, G(X, B)),
    };
}
