// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace azure_image_builder.Core
{
    /// <summary>
    ///     Represents the a sample command named execute
    /// </summary>
    public class BuilderCommandLineApplication : CommandLineApplication
    {
        /// <inheritdoc />
        public BuilderCommandLineApplication(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "execute";
            Description = "Creates Azure VM Images";
            string logLevels = Enum.GetNames(typeof(LogLevel)).Humanize("or");
            CommandOption verbosity = Option("-l|--verbosity", $"Specifies the verbosity, for example {logLevels}",
                CommandOptionType.SingleValue);
            CommandOption vault = Option("-v|--vault", "Specifies the vault URI where secrets are stored",
                CommandOptionType.SingleValue);
            CommandOption clientId = Option("-c|--client-id", "Specifies the client id to access the vault",
                CommandOptionType.SingleValue);
            CommandOption clientSecret = Option("-p|--client-secret", "Specifies the client secret to access the vault",
                CommandOptionType.SingleValue);
            OnExecute(() => Run(verbosity.Value(), vault.Value(), clientId.Value(), clientSecret.Value()));
        }

        private Task<int> Run(string logLevelOption, string vault, string clientId, string clientSecret)
        {
            IServiceProvider serviceProvider =
                CreateServiceProvider(vault, clientId, clientSecret, ParseLogLevel(logLevelOption));

            Parent.ShowRootCommandFullNameAndVersion();

            BuilderCommand command = ActivatorUtilities.CreateInstance<BuilderCommand>(serviceProvider);

            return command.ExecuteAsync();
        }

        private IServiceProvider CreateServiceProvider(string vault, string clientId, string clientSecret,
            LogLevel logLevel)
        {
            IConfigurationRoot configuration = CreateConfiguration(vault, clientId, clientSecret);

            ServiceCollection collection = new ServiceCollection();
            collection.AddLogging(builder =>
                {
                    builder
                        .SetMinimumLevel(logLevel)
                        .AddConsole()
                        .AddDebug();
                })
                .AddSingleton(configuration)
                .AddMemoryCache()
                .AddOptions()
                .Configure<BuilderOptions>(configuration);

            return collection.BuildServiceProvider(true);
        }

        private IConfigurationRoot CreateConfiguration(string vault, string clientId, string clientSecret)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
                .AddEnvironmentVariables("azure_image_builder_")
                .AddUserSecrets(typeof(BuilderCommandLineApplication).Assembly, true)
                .AddJsonFile(ApplicationProfilePath(), true);

            if (!string.IsNullOrWhiteSpace(vault) || !string.IsNullOrWhiteSpace(clientId) ||
                !string.IsNullOrWhiteSpace(clientSecret))
                configurationBuilder = configurationBuilder.AddAzureKeyVault(vault, clientId, clientSecret);

            IConfigurationRoot configuration = configurationBuilder.Build();
            return configuration;
        }

        private string ApplicationProfilePath()
        {
            string profilePath;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                profilePath = Path.Combine("~", ".changehealthcare", Name, "appsettings.json");
            }
            else
            {
                string applicationDataPath = Environment.GetEnvironmentVariable("APPDATA");
                profilePath = Path.Combine(applicationDataPath, "Change Healthcare", Name, "appsettings.json");
            }

            return profilePath;
        }

        private static LogLevel ParseLogLevel(string logLevelOption)
        {
            if (!Enum.TryParse(logLevelOption, true, out LogLevel logLevel)) logLevel = LogLevel.Information;

            return logLevel;
        }
    }
}
