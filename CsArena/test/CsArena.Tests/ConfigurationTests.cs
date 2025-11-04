using Microsoft.Extensions.Configuration;
using Xunit.Abstractions;

namespace CsArena.Tests;

public class ConfigurationTests(ITestOutputHelper output)
{
    [Fact]
    public void GetValueByCompoundPathTest()
    {
        var cfg = TestConfigHelper.GetIConfigurationRoot();

        Assert.Equal(15.4, cfg.GetValue<double>("Books:Price"));
        Assert.Equal("Jon Skeet", cfg.GetValue<string>("Books:Author"));
        Assert.Equal("Software", cfg.GetValue<string>("Books:Category"));

        Assert.Equal(10, cfg.GetValue<int>("Books:Pages", 10));

        // Settings from `appsettings.local.json` override `appsettings.json` settings
        Assert.Equal("local_dev_name", cfg.GetValue<string>("Credentials:Dev:UserName"));
        Assert.Equal("local_stg_name", cfg.GetValue<string>("Credentials:Staging:UserName"));
    }

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
}