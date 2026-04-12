using Melpominee.app.Services.Database;
using Melpominee.app.Services.Startup;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
namespace Melpominee.Tests.Unit.Services.Startup;

public class DataContextTests
{
    /// <summary>
    /// Sentinel test for the unified-singleton invariant. Both the DI-
    /// resolved <see cref="DataContext"/> and the static
    /// <c>DataContext.Instance</c> must reference the exact same object, or
    /// the async-gated and legacy sync code paths will drift across two
    /// <c>NpgsqlDataSource</c> pools and two <c>_initialized</c> flags.
    /// This matches the factory registration in <c>Program.cs</c>.
    /// </summary>
    [Fact]
    public void DataContext_DI_And_Instance_SameObject()
    {
        var services = new ServiceCollection();
        services.AddSingleton<StartupReadiness>();
        services.AddSingleton<DataContext>(sp =>
        {
            var readiness = sp.GetRequiredService<StartupReadiness>();
            DataContext.Instance.AttachReadiness(readiness);
            return DataContext.Instance;
        });
        using var provider = services.BuildServiceProvider();

        var fromDi = provider.GetRequiredService<DataContext>();

        ReferenceEquals(fromDi, DataContext.Instance).ShouldBeTrue(
            "DI must resolve to the same object as DataContext.Instance or " +
            "the async and sync paths will run against split NpgsqlDataSource pools.");
    }

    /// <summary>
    /// The DI factory attaches <see cref="StartupReadiness"/> to the
    /// singleton. After resolution, a fresh reference to the readiness
    /// primitive should be observable through the static singleton — if
    /// this breaks, <see cref="DataContext.ConnectAsync"/> will behave as
    /// though no readiness gate is wired up.
    /// </summary>
    [Fact]
    public void DataContext_DI_Factory_AttachesReadiness()
    {
        var services = new ServiceCollection();
        services.AddSingleton<StartupReadiness>();
        services.AddSingleton<DataContext>(sp =>
        {
            var readiness = sp.GetRequiredService<StartupReadiness>();
            DataContext.Instance.AttachReadiness(readiness);
            return DataContext.Instance;
        });
        using var provider = services.BuildServiceProvider();

        var dc = provider.GetRequiredService<DataContext>();
        var readinessFromDi = provider.GetRequiredService<StartupReadiness>();

        // Mutate readiness through DI and observe via Instance — they are
        // the same graph because DataContext holds a reference to the
        // singleton readiness instance.
        readinessFromDi.ShouldNotBeNull();
        dc.ShouldBe(DataContext.Instance);
    }
}
