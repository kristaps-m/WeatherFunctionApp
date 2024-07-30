using Azure;
using Azure.Data.Tables;
using System;

namespace WeatherFunctionApp.Models
{
    public class WeatherLogEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }

        public string Status { get; set; }
        public string Message { get; set; }
        //public string PartitionKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public string RowKey { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public DateTimeOffset? Timestamp { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        //public ETag ETag { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
