using System;

namespace SCFirstOrderLogic.Inference.Resolution
{
    /// <summary>
    /// Sub-type of <see cref="IClauseStore"/> for types intended for storing the intermediate clauses of a particular query.
    /// Notably, is <see cref="IDisposable"/> so that any required resources can be promptly tidied away once the query is complete.
    /// </summary>
    public interface IQueryClauseStore : IClauseStore, IDisposable
    {
    }
}
