using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_Processing_and_Anomaly_Detection.Models
{
    public class ServerStatistics
    {
        public String ServerIdentifier { get; set; }
        public double MemoryUsage { get; set; } // in MB
        public double AvailableMemory { get; set; } // in MB
        public double CpuUsage { get; set; }
        public DateTime Timestamp { get; set; }

    }
}
