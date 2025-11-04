using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CsArena.Tests.config;

public static class TestConfigHelper
{
    public static IConfigurationRoot GetConfigurationRoot(
        bool enableSecrets = false,
        bool enableEnvVar = false,
        bool enableCmdLine = false, string[]? args = null)
    {
        var bld = new ConfigurationBuilder();

        bld.AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("config/appsettings.local.json", optional: true, reloadOnChange: true);

        if (enableSecrets)
            bld.AddUserSecrets("acab85f1-6255-410f-a9ef-b94f35eff04b");
        if (enableEnvVar)
            bld.AddEnvironmentVariables();
        if (enableCmdLine)
            bld.AddCommandLine(args ?? Environment.GetCommandLineArgs());

        return bld.Build();
    }

    public static Options GetOptions()
    {
        // Options pattern
        return GetConfigurationRoot().Get<Options>() ?? new Options();
    }

    public static void PrintConfig(ITestOutputHelper output, IConfiguration? config = null)
    {
        config ??= GetConfigurationRoot(enableSecrets: true, enableEnvVar: true, enableCmdLine: true);

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
        foreach (var provider in GetConfigurationRoot(enableSecrets: true, enableEnvVar: true, enableCmdLine: true).Providers.ToList())
        {
            output.WriteLine($"{provider}");
        }
    }
}