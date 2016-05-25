using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Booking
{
    public class Startup
    {
        private IConfigurationRoot Configuration { get; set; } = null;
        
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings-{env.EnvironmentName}.json", optional: true);
            
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            
        }
        
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async (context) => 
            {
                try
                {
                    var defaultConn = Configuration["ConnectionStrings:Default"];
                    await context.Response.WriteAsync(defaultConn + '\n');
                    
                    var conn = new NpgsqlConnection();
                    conn.ConnectionString = defaultConn;
                    
                    conn.Open();
                    
                    var commText = Configuration["Data:Command"];
                    await context.Response.WriteAsync(commText + '\n');
                    
                    var comm = conn.CreateCommand();
                    comm.CommandText = commText;
                    
                    var result = comm.ExecuteScalar();
                    await context.Response.WriteAsync(result.ToString());
                }
                catch (System.Exception ex)
                {
                    await context.Response.WriteAsync(ex.ToString());
                }
            });
        }
    }
}