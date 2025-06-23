using Message_Processing_and_Anomaly_Detection.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Message_Processing_and_Anomaly_Detection.Interfaces
{
    public interface ISignalRService
    {
        Task SendAlertAsync(string message);


    }
}
