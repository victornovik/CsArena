using Microsoft.Extensions.Configuration;

namespace CsArena.Tests.config;

public static class TestConfigHelper
{
    private static readonly IReadOnlyDictionary<string, string?> TestSecrets = new Dictionary<string, string?>
    {
        ["Credentials:Dev:UserName"] = "user_from_secrets",
        ["Credentials:Stg:UserName"] = "user_from_secrets",
        ["Credentials:Stg:Password"] = "password_from_secrets"
    };

    public static IConfigurationRoot GetConfigurationRoot(
        bool enableSecrets = false,
        bool enableEnvVar = false,
        bool enableCmdLine = false, string[]? args = null)
    {
        var bld = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory);

        bld.AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile("config/appsettings.local.json", optional: true, reloadOnChange: true);

        if (enableSecrets)
        {
            bld.AddUserSecrets("acab85f1-6255-410f-a9ef-b94f35eff04b");
            bld.AddInMemoryCollection(TestSecrets);
        }
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

    /// <summary>Writes every configuration key as "path - value" (parents before children) and returns the lines.</summary>
    public static IReadOnlyList<string> PrintConfig(ITestOutputHelper output, IConfiguration? config = null)
    {
        config ??= GetConfigurationRoot(enableSecrets: true, enableEnvVar: true, enableCmdLine: true);

        var lines = new List<string>();
        CollectLines(config, lines);
        foreach (var line in lines)
        {
            output.WriteLine(line);
        }
        return lines;
    }

    private static void CollectLines(IConfiguration config, List<string> lines)
    {
        foreach (var pair in config.GetChildren())
        {
            lines.Add($"{pair.Path} - {pair.Value}");
            CollectLines(pair, lines);
        }
    }

    /// <summary>Writes a header and one line per configuration provider, in registration order, and returns the lines.</summary>
    public static IReadOnlyList<string> PrintConfigProviders(ITestOutputHelper output)
    {
        List<string> lines = ["CONFIGURATION PROVIDERS:"];
        lines.AddRange(GetConfigurationRoot(enableSecrets: true, enableEnvVar: true, enableCmdLine: true)
            .Providers.Select(provider => $"{provider}"));

        foreach (var line in lines)
        {
            output.WriteLine(line);
        }
        return lines;
    }
}
