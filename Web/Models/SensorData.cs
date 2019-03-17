using System;

namespace Web
{
    public class SensorData
    {
        public long Id { get; set; }
        public string Partition { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }
}