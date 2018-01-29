using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Trackable.EntityFramework
{
    public class LogEntryEntity : TableEntity
    {
        public LogEntryEntity(string PartitionKey, string RowKey)
        {
            this.PartitionKey = PartitionKey;
            this.RowKey = RowKey;
            value = "0";
        }

        public LogEntryEntity() { }

        public string value { get; set;}
    }
}
