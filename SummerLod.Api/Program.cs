using Serilog;
using SummerLod.Api.Services;
using System.Collections;
using System.Reflection;

namespace SummerLod.Api;

public static class Program
{
    private static void DumpEnvironmentVars()
    {
        Console.WriteLine("ENVIRONMENT VARIABLES:");
        IDictionary dct = Environment.GetEnvironmentVariables();
        List<string> keys = [];
        var enumerator = dct.GetEnumerator();
        while (enumerator.MoveNext())
        {
            keys.Add(((DictionaryEntry)enumerator.Current).Key.ToString()!);
        }

        foreach (string key in keys.OrderBy(s => s))
            Console.WriteLine($"{key} = {dct[key]}");
    }

    private static void ConfigureCorsServices(IServiceCollection services,
        IConfiguration config)
    {
        string[] origins = ["http://localhost:4200"];

        IConfigurationSection section = config.GetSection("AllowedOrigins");
        if (section.Exists())
        {
            origins = section.AsEnumerable()
                .Where(p => !string.IsNullOrEmpty(p.Value))
                .Select(p => p.Value).ToArray()!;
        }

        services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyMethod()
                .AllowAnyHeader()
                // https://github.com/aspnet/SignalR/issues/2110 for AllowCredentials
                .AllowCredentials()
                .WithOrigins(origins);
        }));
    }

    private static void ConfigureServices(IServiceCollection services,
        IConfiguration config)
    {
        // API controllers
        services.AddControllers();

        // CORS
        ConfigureCorsServices(services, config);

        // Serilog
        services.AddSingleton(sp =>
        {
            var factory = sp.GetRequiredService<ILoggerFactory>();
            return factory.CreateLogger("Logger");
        });

        // Swagger/OpenAPI (https://aka.ms/aspnetcore/swashbuckle)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // local
        services.AddSingleton<TeiEntityParserService>();
    }

    private static void ConfigureLogger(WebApplicationBuilder builder)
    {
        if (string.IsNullOrWhiteSpace(builder.Environment.WebRootPath))
        {
            string currentDirectory = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly().Location) ?? "";
            builder.Environment.WebRootPath = Path.Combine(currentDirectory,
                "wwwroot");
        }

        string logFilePath = Path.Combine(builder.Environment.WebRootPath,
            "log.txt");
        builder.Host.UseSerilog((hostingContext, services, loggerConfiguration)
            => loggerConfiguration
            // .ReadFrom.Configuration(hostingContext.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day),
                writeToProviders: true);
    }

    public static async Task Main(string[] args)
    {
        DumpEnvironmentVars();

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        IConfiguration config = new ConfigurationService(builder.Environment)
            .Configuration;

        ConfigureServices(builder.Services, config);
        ConfigureLogger(builder);

        WebApplication app = builder.Build();

        // CORS
        app.UseCors("CorsPolicy");

        // HTTP request pipeline
        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();
        app.Run();
    }
}
