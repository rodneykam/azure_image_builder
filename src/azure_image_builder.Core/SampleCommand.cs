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
    public class SampleCommand
    {
        /// <summary>
        /// Creates an instance of <see cref="SampleCommand"/>
        /// </summary>
        /// <param name="loggerFactory">A factory for logging</param>
        /// <param name="options">The options to run the command</param>
        public SampleCommand(ILoggerFactory loggerFactory, IOptions<SampleOptions> options)
        {
            Logger = loggerFactory.CreateLogger("Sample");
            Options = options.Value;
        }

        private SampleOptions Options { get; }

        private ILogger Logger { get; }

        /// <summary>
        /// Executes the command
        /// </summary>
        /// <returns>0 if the command was succesfull, non-zero if not</returns>
        public Task<int> ExecuteAsync()
        {
            Options.Validate();

            Logger.LogInformation("Value1 {Value1}", Options.Value1);
            Logger.LogInformation("Value2 {Value2}", Options.Value2);
            Logger.LogInformation("Value3 {Value3}", Options.Value3);

            return Task.FromResult(0);
        }
    }
}
