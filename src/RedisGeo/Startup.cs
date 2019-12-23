using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Configuration;

namespace RedisGeo
{
  public class Startup
  {
    IConfiguration Configuration { get; set; }
    public Startup(IConfiguration configuration) => Configuration = configuration;

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
    {
      //loggerFactory.AddConsole();
      loggerFactory.AddProvider(new CustomLoggerProvider());

      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseServiceStack(new AppHost
      {
        AppSettings = new NetCoreAppSettings(Configuration)
      });
    }
  }

  public class CustomLoggerProvider : ILoggerProvider
  {
    public void Dispose() { }

    public ILogger CreateLogger(string categoryName)
    {
      return new CustomConsoleLogger(categoryName);
    }

    public class CustomConsoleLogger : ILogger
    {
      private readonly string _categoryName;

      public CustomConsoleLogger(string categoryName)
      {
        _categoryName = categoryName;
      }

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
      {
        if (!IsEnabled(logLevel))
        {
          return;
        }

        Console.WriteLine($"{logLevel}: {_categoryName}[{eventId.Id}]: {formatter(state, exception)}");
      }

      public bool IsEnabled(LogLevel logLevel)
      {
        return true;
      }

      public IDisposable BeginScope<TState>(TState state)
      {
        return null;
      }
    }
  }
}
