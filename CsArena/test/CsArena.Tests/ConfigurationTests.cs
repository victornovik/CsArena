using CsArena.Tests.config;
using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CsArena.Tests;

public class ConfigurationTests(ITestOutputHelper output)
{
    [Fact]
    public void PrintConfigTest()
    {
        TestConfigHelper.PrintConfig(output);
    }

    [Fact]
    public void PrintConfigProvidersTest()
    {
        TestConfigHelper.PrintConfigProviders(output);
    }

    [Fact]
    public void GetValueByCompoundPathTest()
    {
        var cfg = TestConfigHelper.GetConfigurationRoot();

        Assert.Equal(15.4, cfg.GetValue<double>("Books:Price"));
        Assert.Equal("Jon Skeet", cfg.GetValue<string>("Books:Author"));
        Assert.Equal("Software", cfg.GetValue<string>("Books:Category"));

        Assert.False(cfg.GetSection("Books:Pages").Exists());
        Assert.Equal(10, cfg.GetValue("Books:Pages", 10));

        // Settings from `appsettings.local.json` override `appsettings.json` settings
        Assert.Equal("local_dev_name", cfg.GetValue<string>("Credentials:Dev:UserName"));
        Assert.Equal("local_stg_name", cfg.GetValue<string>("Credentials:Stg:UserName"));

        output.WriteLine(cfg.GetDebugView());
    }

    [Fact]
    public void GetSectionTest()
    {
        var cfg = TestConfigHelper.GetConfigurationRoot();
        
        var books = cfg.GetSection("Books");
        Assert.Equal(15.4, books.GetValue<double>("Price"));
        Assert.Equal("Jon Skeet", books.GetValue<string>("Author"));
        Assert.Equal("Software", books.GetValue<string>("Category"));

        var dev = cfg.GetSection("Credentials:Dev");
        Assert.Equal("local_dev_name", dev.GetValue<string>("UserName"));

        var stg = cfg.GetSection("Credentials:Stg");
        Assert.Equal("local_stg_name", stg.GetValue<string>("UserName"));

        output.WriteLine(cfg.GetDebugView());
    }

    [Fact]
    public void CaseInsensitiveKeysTest()
    {
        var cfg = TestConfigHelper.GetConfigurationRoot();

        Assert.Equal(15.4, cfg.GetValue<double>("booKs:price"));
        Assert.Equal("Jon Skeet", cfg.GetValue<string>("books:AUTHOR"));
        Assert.Equal("Software", cfg.GetValue<string>("books:category"));
        Assert.Equal("local_dev_name", cfg.GetValue<string>("Credentials:DEV:username"));
        Assert.Equal("local_stg_name", cfg.GetValue<string>("Credentials:STG:username"));

        output.WriteLine(cfg.GetDebugView());
    }

    [Fact]
    public void OptionsPatternTest()
    {
        var opt = TestConfigHelper.GetOptions();

        Assert.Equal(15.4, opt.Books!.Price);
        Assert.Equal("Jon Skeet", opt.Books.Author);
        Assert.Equal("Software", opt.Books.Category);

        Assert.Equal("local_dev_name", opt.Credentials?.Dev?.UserName);
        Assert.Equal("local_stg_name", opt.Credentials?.Stg?.UserName);
        Assert.Null(opt.Credentials?.QA?.UserName);
    }

    [Fact]
    public void UserSecretsTest()
    {
        var cfg = TestConfigHelper.GetConfigurationRoot(enableSecrets: true);

        Assert.Equal(15.4, cfg.GetValue<double>("Books:Price"));
        Assert.Equal("Jon Skeet", cfg.GetValue<string>("Books:Author"));
        Assert.Equal("Software", cfg.GetValue<string>("Books:Category"));

        Assert.Equal(10, cfg.GetValue("Books:Pages", 10));

        // Settings from UserSecrets override both `appsettings.local.json` and `appsettings.json`
        Assert.Equal("user_from_secrets", cfg.GetValue<string>("Credentials:Dev:UserName"));
        Assert.Equal("password_from_secrets", cfg.GetValue<string>("Credentials:Stg:Password"));

        output.WriteLine(cfg.GetDebugView());
    }

    [Fact]
    public void UserSecretsMappedToOptionsTest()
    {
        var cfg = TestConfigHelper.GetConfigurationRoot(enableSecrets: true);
        var opt = cfg.Get<Options>();

        Assert.Equal(15.4, opt!.Books!.Price);
        Assert.Equal("Jon Skeet", opt.Books.Author);
        Assert.Equal("Software", opt.Books.Category);

        Assert.Equal("user_from_secrets", opt.Credentials?.Dev?.UserName);
        Assert.Equal("user_from_secrets", opt.Credentials?.Stg?.UserName);
        Assert.Null(opt.Credentials?.QA?.UserName);

        output.WriteLine(cfg.GetDebugView());
    }

    [Fact]
    public void EnvVariablesTest()
    {
        // In environment variables, a colon separator may not work on all platforms.
        // A double underscore __ is supported by all platforms and is automatically converted into a colon :.
        Environment.SetEnvironmentVariable("Books__Price", "55.66");
        Environment.SetEnvironmentVariable("Books__Author", "Pikul");
        Environment.SetEnvironmentVariable("Books__Category", "History");
        Environment.SetEnvironmentVariable("Credentials__Dev__UserName", "RealUser");
        Environment.SetEnvironmentVariable("Credentials__Dev__Password", "123");

        var cfg = TestConfigHelper.GetConfigurationRoot(enableSecrets: true, enableEnvVar: true);

        Assert.Equal(55.66, cfg.GetValue<double>("Books:Price"));
        Assert.Equal("Pikul", cfg.GetValue<string>("Books:Author"));
        Assert.Equal("History", cfg.GetValue<string>("Books:Category"));

        // Settings from env variables override UserSecrets, `appsettings.local.json` and `appsettings.json`
        Assert.Equal("RealUser", cfg.GetValue<string>("Credentials:Dev:UserName"));
        Assert.Equal("123", cfg.GetValue<string>("Credentials:Dev:Password"));

        output.WriteLine(cfg.GetDebugView());
    }

    [Fact]
    public void CommandLineTest()
    {
        string[] cmdLineArgs = ["--Books:Price=3.4", "--Books:Author=Gogol", "--Books:Category=Fiction", "Credentials:Dev:UserName=user", "Credentials:Dev:Password=pwd"];
        var cfg = TestConfigHelper.GetConfigurationRoot(enableSecrets: true, enableEnvVar: true, enableCmdLine:true, args:cmdLineArgs);

        Assert.Equal(3.4, cfg.GetValue<double>("Books:Price"));
        Assert.Equal("Gogol", cfg.GetValue<string>("Books:Author"));
        Assert.Equal("Fiction", cfg.GetValue<string>("Books:Category"));

        Assert.Equal("user_from_secrets", cfg.GetValue<string>("Credentials:Stg:UserName"));
        Assert.Equal("password_from_secrets", cfg.GetValue<string>("Credentials:Stg:Password"));

        // Settings from command line override everything
        Assert.Equal("user", cfg.GetValue<string>("Credentials:Dev:UserName"));
        Assert.Equal("pwd", cfg.GetValue<string>("Credentials:Dev:Password"));

        output.WriteLine(cfg.GetDebugView());
    }
}