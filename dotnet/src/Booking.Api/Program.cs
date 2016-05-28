using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Booking.Api
{
    public class Program
    {
        private const string HostingJsonFile = "hosting.json"; 
        
        public static void Main(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            
            var config = new ConfigurationBuilder()
                .SetBasePath(currentDir)
                .AddJsonFile(HostingJsonFile)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();
                
            var host = new WebHostBuilder()
                .UseConfiguration(config)
                .UseContentRoot(currentDir)
                .UseStartup<Startup>()
                .Build();
                
            host.Run();
        }
    }
}
