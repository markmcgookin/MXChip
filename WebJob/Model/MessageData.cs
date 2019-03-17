using System;

namespace WebJob
{
    public class MessageData
    {
        public long messageId { get; set; }
        public Nullable<decimal> temperature { get; set; }
        public Nullable<decimal> humidity { get; set; }
    }
}