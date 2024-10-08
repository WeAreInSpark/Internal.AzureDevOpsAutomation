using AdoStateChangeHttpFunction;
using AdoStateProcessor.Misc;
using AdoStateProcessor.Repos;
using AdoStateProcessor.Repos.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
    {
        configurationBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: true);
        configurationBuilder.AddEnvironmentVariables();
    })
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddAdoClientFactory(hostBuilderContext.Configuration);
        services.AddSingleton<IWorkItemRepo, WorkItemRepo>();
        services.AddSingleton<IRulesRepo, RulesRepo>();
        services.AddSingleton<IHelper, Helper>();
    })
    .Build();

host.Run();
