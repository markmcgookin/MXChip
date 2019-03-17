using System.Linq;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace WebJob
{
    internal class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly IConfiguration Config;
        public EventProcessorFactory(IConfiguration config)
        {
            Config = config;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            var connectionString = Config.GetConnectionString("SensorDatabase");
            var optionsBuilder = new DbContextOptionsBuilder<IotDatabaseContext>()
                                        .UseSqlServer(connectionString, providerOptions => providerOptions.CommandTimeout(60))
                                        .UseQueryTrackingBehavior(QueryTrackingBehavior.TrackAll);

            var database = new IotDatabaseContext(optionsBuilder.Options);

            return new EventProcessor(database);
        }
    }
}