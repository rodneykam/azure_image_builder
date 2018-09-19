// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

using System;
using azure_image_builder.Core;

namespace azure_image_builder
{
    /// <summary>
    ///     Represents the entry point of the application
    /// </summary>
    internal static class Program
    {
        /// <summary>
        ///     Entry Point
        /// </summary>
        private static int Main(string[] args)
        {
            int result;
            Application app = new Application();
            try
            {
                result = app.Execute(args);
            }
            catch (ArgumentException e)
            {
                Console.Error.WriteLine(e);
                result = -1;
            }

            Console.WriteLine();
            return result;
        }
    }
}
