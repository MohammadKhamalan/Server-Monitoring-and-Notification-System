using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_Statistics_Collection_Service.Models
{
    public class ServerStatistics
    {
        public double MemoryUsage { get; set; } // in MB
        public double AvailableMemory { get; set; } // in MB
        public double CpuUsage { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
