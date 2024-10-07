using AdoStateProcessor.Misc;
using AdoStateProcessor.Repos;
using AdoStateProcessor.Repos.Interfaces;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
        services.AddSingleton<IWorkItemRepo, WorkItemRepo>();
        services.AddSingleton<IRulesRepo, RulesRepo>();
        services.AddSingleton<IHelper, Helper>();

        services.AddHttpClient<IRulesRepo, RulesRepo>();
    })
    .Build();

host.Run();