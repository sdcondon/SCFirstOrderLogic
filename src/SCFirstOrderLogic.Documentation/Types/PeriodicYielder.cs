namespace SCFirstOrderLogic.Documentation.Types;

public static class PeriodicYielder
{
    private static readonly TimeSpan YieldPeriod = TimeSpan.FromMilliseconds(200);
    private static DateTimeOffset LastYield = DateTimeOffset.MinValue;

    public static async Task PerhapsYield(CancellationToken cancellationToken)
    {
        if (LastYield + YieldPeriod < DateTimeOffset.UtcNow)
        {
            LastYield = DateTimeOffset.UtcNow;
            await Task.Delay(1, cancellationToken);
        }
    }
}
