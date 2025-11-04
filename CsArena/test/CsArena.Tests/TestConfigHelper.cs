using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CsArena.Tests;

public static class TestConfigHelper
{
    public static IConfigurationRoot GetIConfigurationRoot()
    {
        return new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true)
            .AddUserSecrets("e3dfcccf-0cb3-423a-b302-e3e92e95c128")
            .AddEnvironmentVariables()
            .Build();
    }

    //public static MyAppConfig GetMyAppConfiguration()
    //{
    //    var configuration = new MyAppConfig();
    //    GetIConfigurationRoot().Bind("MyApp", configuration);
    //    return configuration;
    //}

    public static void PrintConfig(ITestOutputHelper output, IConfiguration? config = null)
    {
        config ??= GetIConfigurationRoot();
        foreach (var pair in config.GetChildren())
        {
            var res = $"{pair.Path} - {pair.Value}";
            // Console.WriteLine(res);
            output.WriteLine(res);
            PrintConfig(output, pair);
        }
    }

    public static void PrintConfigProviders(ITestOutputHelper output)
    {
        output.WriteLine("CONFIGURATION PROVIDERS:");
        foreach (var provider in GetIConfigurationRoot().Providers.ToList())
        {
            output.WriteLine($"{provider}");
        }
    }
}