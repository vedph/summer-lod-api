namespace SummerLod.Api.Services;

internal sealed class ConfigurationService
{
    private readonly IWebHostEnvironment _env;
    private IConfiguration? _configuration;

    public ConfigurationService(IWebHostEnvironment env)
    {
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    public IConfiguration Configuration
    {
        get
        {
            _configuration ??= new ConfigurationBuilder()
                .AddJsonFile("appsettings.json",
                    optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{_env.EnvironmentName}.json",
                    optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            return _configuration;
        }
    }
}
