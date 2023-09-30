using Shimi;

namespace Bryan.Proof.Auth.Api.Test.TestBase;

public class ClockSkew : IDisposable
{
    public DateTime Clock { get; private set; }
    public Shim<DateTime>? ClockShim { get; private set; } = null;

    public ClockSkew()
    {
    }

    public ClockSkew(DateTime clock) => SetHostCLock(clock);

    public void SetHostCLock(DateTime clock, TimeSpan timezone = default)
    {
        ClearAsOf();

        Clock = clock;

        if (timezone == default)
            timezone = TimeZoneInfo.Local.BaseUtcOffset;

        var mockedInitialUtcDatetime = new DateTimeOffset(Clock, timezone).UtcDateTime;

        try
        {
            Console.WriteLine("DateMock");
            Shim.ResultOf(() => DateTime.UtcNow).To(mockedInitialUtcDatetime, out var clockShim);
            Console.WriteLine("Done!!!");
            ClockShim = clockShim;
        }
        catch { }
    }

    public void ClearAsOf() => ClockShim?.Clear();

    public void Dispose() => ClearAsOf();

    ~ClockSkew() { ClearAsOf(); }
}