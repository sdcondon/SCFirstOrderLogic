namespace SCFirstOrderLogic.TestUtilities;

/// <summary>
/// A badly behaved class of which all instances share a hash code of zero.
/// Intended for tests that cover hash code collision (of an constant, function or predicate identifier) scenarios.
/// </summary>
public class HashCodeCollision
{
    private readonly string id;

    public HashCodeCollision(string id) => this.id = id;

    public override int GetHashCode() => 0;

    public override bool Equals(object? obj) => obj is HashCodeCollision mhcc && id.Equals(mhcc.id);
}
