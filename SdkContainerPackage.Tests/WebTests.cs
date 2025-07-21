using Microsoft.Extensions.Logging;

namespace SdkContainerPackage.Tests;

public class PackageTests
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(30);

    [Test]
    [CancelAfter(30_000)]
    public async Task GoesHealthy(CancellationToken cancellationToken)
    {
        var appHost = DistributedApplicationTestingBuilder.Create()
            .WithTestProjectOverride();

        appHost.AddApiService("apiService");

        //var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.SdkContainerPackage_AppHost>();
        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Debug);
            // Override the logging filters from the app's configuration
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Debug);
            logging.AddFilter("Aspire.", LogLevel.Debug);
            logging.AddSimpleConsole(x => x.TimestampFormat = "HH:mm:ss.fff ");
        });

        using var app = await appHost.BuildAsync(cancellationToken).WaitAsync(cancellationToken);

        await app.StartAsync(cancellationToken).WaitAsync(cancellationToken);

        await app.ResourceNotifications.WaitForResourceHealthyAsync("apiService", cancellationToken);
    }
}
