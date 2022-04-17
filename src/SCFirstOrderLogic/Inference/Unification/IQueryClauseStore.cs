using System;

namespace SCFirstOrderLogic.Inference.Unification
{
    /// <summary>
    /// Sub-type of <see cref="IClauseStore"/> for types intended for storing the intermediate clauses of a particular query.
    /// </summary>
    public interface IQueryClauseStore : IClauseStore, IDisposable
    {
    }
}
