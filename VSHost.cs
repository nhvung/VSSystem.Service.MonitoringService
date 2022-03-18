using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using VSSystem.Hosting;
using VSSystem.Logger;
using VSSystem.Service.MonitoringService.Service;

namespace VSSystem.Service.MonitoringService
{
    public class VSHost : AHost
    {
        public const string PRIVATE_KEY = "304c3357-3376-7645-2164-336e63332139";
        public static string SERVICE_NAME = null;
        static Task Main(string[] args)
        {
            return new VSHost("MonitoringService", 3061, 3062, true, null, PRIVATE_KEY)
                .RunAsync(args);
        }
        public VSHost(string name, int httpPort, int httpsPort, bool useOnlineConfiguration, string rootName, string privateKey)
            : base(name, httpPort, httpsPort, useOnlineConfiguration, rootName, privateKey)
        {
            if (!string.IsNullOrWhiteSpace(rootName))
            {
                if (string.IsNullOrWhiteSpace(SERVICE_NAME))
                {
                    SERVICE_NAME = _name;
                }
            }
        }
        public static ALogger StaticLogger = null;
        protected override void _InitLogger()
        {
            base._InitLogger();
            StaticLogger = _logger;
        }
        protected override void _InitInjectionServices()
        {
            string rootName = _rootName;
            if (!string.IsNullOrWhiteSpace(rootName))
            {
                rootName = _name;
            }
            _AddInjectedServices(new VSService(_name, _server_ID, rootName, _privateKey, _logger));
        }

        protected override void _UseStartup(IWebHostBuilder webHostBuilder)
        {
            webHostBuilder.UseStartup<VSStartup>();
        }
    }
}