﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_Processing_and_Anomaly_Detection.Models
{
    public class AnomalyDetectionConfig
    {
        public double MemoryUsageAnomalyThresholdPercentage { get; set; }
        public double CpuUsageAnomalyThresholdPercentage { get; set; }
        public double MemoryUsageThresholdPercentage { get; set; }
        public double CpuUsageThresholdPercentage { get; set; }
    }
}
