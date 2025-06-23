using Microsoft.Extensions.Configuration;

namespace SignalR_Event_Consumer_Service.Config
{
    public class ConfigurationHelper
    {
        private readonly IConfigurationRoot _configuration;

        public ConfigurationHelper()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            _configuration = builder.Build();
        }

        public string GetSignalRHubUrl()
        {
            return _configuration.GetConnectionString("SignalRUrl");
        }
    }
}