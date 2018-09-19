// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

using Microsoft.Extensions.CommandLineUtils;

namespace azure_image_builder.Core
{
    /// <summary>
    ///     Represents the help command
    /// </summary>
    public class HelpCommandLineApplication : CommandLineApplication
    {
        /// <inheritdoc />
        public HelpCommandLineApplication(bool throwOnUnexpectedArg = true) : base(throwOnUnexpectedArg)
        {
            Name = "help";
            Description = "Shows help for a command";
            CommandArgument command = Argument("command", "The command to show help for");
            OnExecute(() => InternalExecute(command.Value));
        }

        private int InternalExecute(string command)
        {
            if (Parent != null)
                Parent.ShowHelp(command);
            else
                ShowHelp(command);
            return 0;
        }
    }
}
