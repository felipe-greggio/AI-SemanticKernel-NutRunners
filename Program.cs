
using Apertadeiras_POC.Plugins;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents;

namespace Apertadeiras_POC;

public class Program
{
    public static void Main(string[] args)
    {

        var modelId = "gpt-4o-mini";
        var endpoint = "-";
        var apiKey = "-";

        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddTransient((serviceProvider) => {
            KernelPluginCollection pluginCollection = [];
            pluginCollection.AddFromType<LogsPlugin>("Logs");
            pluginCollection.AddFromType<TestPlugin>("Test");

            return pluginCollection;
        });

        builder.Services.AddTransient<Kernel>((serviceProvider) => {
            KernelPluginCollection pluginCollection = serviceProvider.GetRequiredService<KernelPluginCollection>();
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();

            kernelBuilder.AddAzureOpenAIChatCompletion(
                deploymentName: modelId,
                endpoint: endpoint,
                apiKey: apiKey
            );

            foreach (var plugin in pluginCollection)
            {
                kernelBuilder.Plugins.Add(plugin);
            }

            var kernel = kernelBuilder.Build();
            return kernel;
        });

        builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.S
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
