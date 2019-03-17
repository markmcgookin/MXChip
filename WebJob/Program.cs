using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WebJob
{
    class Program
    {
        static void Main(string[] args)
        {
            // .NET Core sets the source directory as the working directory - so change that:
            Directory.SetCurrentDirectory(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

            var serviceCollection = new ServiceCollection();
            var config = GetWebJobConfiguration();

            var culture = CultureInfo.CreateSpecificCulture("en-GB");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            MainAsync(config).GetAwaiter().GetResult();
        }


        private static async Task MainAsync(IConfigurationRoot config)
        {
            Console.WriteLine("Registering EventProcessor...");
            var eventHub = config.GetValue<string>("EventHubName");
            var connectionString = config.GetValue<string>("EventHubConnectionString");
            var storage = config.GetConnectionString("AzureWebJobsStorage");
            var container = config.GetValue<string>("StorageContainerName");

            var eventProcessorHost = new EventProcessorHost(
                eventHub,
                PartitionReceiver.DefaultConsumerGroupName,
                connectionString,
                storage,
                container);

            // Registers the Event Processor Host and starts receiving messages
            await eventProcessorHost.RegisterEventProcessorFactoryAsync(new EventProcessorFactory(config));

            //not sure this will work on line... check
            Console.WriteLine("Receiving. Press ENTER to stop worker.");
            Console.ReadLine();

            // Disposes of the Event Processor Host
            await eventProcessorHost.UnregisterEventProcessorAsync();
        }

        private static IConfigurationRoot GetWebJobConfiguration()
        {
            var jsonFile = "appsettings.json";
            var path = Directory.GetCurrentDirectory() + $"/{jsonFile}";
            var reader = new JsonTextReader(new StreamReader(path));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(jsonFile, optional: false, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return configuration;
        }
    }
}
