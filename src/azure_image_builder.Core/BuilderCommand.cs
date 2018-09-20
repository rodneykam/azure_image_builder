// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace azure_image_builder.Core
{
    /// <summary>
    ///     Represents a sample command
    /// </summary>
    public class BuilderCommand
    {
        /// <summary>
        /// Creates an instance of <see cref="BuilderCommand"/>
        /// </summary>
        /// <param name="loggerFactory">A factory for logging</param>
        /// <param name="options">The options to run the command</param>
        public BuilderCommand(ILoggerFactory loggerFactory, IOptions<BuilderOptions> options)
        {
            Logger = loggerFactory.CreateLogger("AzImgBldr");
            Options = options.Value;
        }

        private BuilderOptions Options { get; }

        private ILogger Logger { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns>0 if the command was succesfull, non-zero if not</returns>
        public Task<int> ExecuteAsync()
        {
            Options.Validate();

            Logger.LogInformation("Value1 {0}", Options.Value1);
            Logger.LogInformation("Value2 {0}", Options.Value2);
            Logger.LogInformation("Value3 {0}", Options.Value3);

            return Task.FromResult(0);
        }
    }
}
