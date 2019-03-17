using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;

namespace WebJob
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IotDatabaseContext Database;
        private const string HUMIDITY = "humidity";
        private const string TEMPERATURE = "temperature";
        public decimal LastSeenTemp;
        public decimal LastSeenHumidity;
        public EventProcessor(IotDatabaseContext databse)
        {
            Database = databse;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public async Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            foreach (var eventData in messages)
            {
                var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
                Console.Out.WriteLine($"Message received. Partition: '{context.PartitionId}', Data: '{data}'");

                var reading = JsonConvert.DeserializeObject<MessageData>(data);

                if (reading.humidity != null)
                {
                    if (reading.humidity != LastSeenHumidity)
                    {
                        LastSeenHumidity = reading.humidity.Value;

                        //Needs updating/inserting
                        var row = Database.SensorData.FirstOrDefault(x => x.Partition == context.PartitionId
                                                                        && x.Type == HUMIDITY);
                        if (row != null)
                        {
                            row.Modified = DateTime.UtcNow;
                            row.Value = reading.humidity.ToString();
                            Database.Update(row);
                        }
                        else
                        {
                            var timestamp = DateTime.UtcNow;
                            var newRow = new SensorData();
                            newRow.Partition = context.PartitionId;
                            newRow.Created = timestamp;
                            newRow.Modified = timestamp;
                            newRow.Type = HUMIDITY;
                            newRow.Value = reading.humidity.ToString();

                            Database.Add(newRow);
                        }

                        Database.SaveChanges();
                    }
                }

                if (reading.temperature != null)
                {
                    if (reading.temperature != LastSeenTemp)
                    {
                        LastSeenTemp = reading.temperature.Value;

                        //Needs updating/inserting
                        var row = Database.SensorData.FirstOrDefault(x => x.Partition == context.PartitionId
                                                                        && x.Type == TEMPERATURE);
                        if (row != null)
                        {
                            row.Modified = DateTime.UtcNow;
                            row.Value = reading.temperature.ToString();
                            Database.Update(row);
                        }
                        else
                        {
                            var timestamp = DateTime.UtcNow;
                            var newRow = new SensorData();
                            newRow.Created = timestamp;
                            newRow.Partition = context.PartitionId;
                            newRow.Modified = timestamp;
                            newRow.Type = TEMPERATURE;
                            newRow.Value = reading.temperature.ToString();

                            Database.Add(newRow);
                        }

                        Database.SaveChanges();
                    }
                }
            }

            await context.CheckpointAsync();
        }
    }
}