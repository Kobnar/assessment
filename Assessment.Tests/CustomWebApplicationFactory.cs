using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Assessment.Tests;


public class CustomWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddJsonFile("appsettings.Test.json"); // Specify the test config file
        });
    }
    
    public IConfiguration GetConfiguration()
    {
        using var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<IConfiguration>();
    }
    
    public T GetService<T>() where T : notnull
    {
        using var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<T>();
    }
}