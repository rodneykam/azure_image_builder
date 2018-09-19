// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

using System;
using Microsoft.Extensions.CommandLineUtils;

namespace azure_image_builder.Core
{
    /// <summary>
    ///     Represents the root application
    /// </summary>
    public class Application : CommandLineApplication
    {
        /// <inheritdoc />
        public Application(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Description = "azure_image_builder deploys unicorns and tops it off with snowflakes.";
            Name = "azure_image_builder";
            FullName = "azure_image_builder";
            ExtendedHelpText = Environment.NewLine + $"Run '{Name} help [command]' for more information on a command." +
                               Environment.NewLine;

            // Add Commands
            Commands.Add(new HelpCommandLineApplication
            {
                Parent = this
            });
            Commands.Add(new SampleCommandLineApplication
            {
                Parent = this
            });

            CommandOption versionOption = VersionOption("--version", ShortFormVersionGetter);
            OnExecute(() =>
            {
                if (versionOption.HasValue())
                    ShowVersion();
                else
                    ShowHelp();
                return 1;
            });
        }

        private string ShortFormVersionGetter()
        {
            return "1.0.0.0";
        }
    }
}
