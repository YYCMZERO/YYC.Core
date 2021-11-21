using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace AntJoin.Core.Configuration
{
    public class LoadAppSettingConfiguration
    {
        public static IConfigurationRoot BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile("appsettings." + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") + ".json", true, true)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
