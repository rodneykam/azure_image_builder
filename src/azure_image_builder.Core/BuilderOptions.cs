// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

namespace azure_image_builder.Core
{
    /// <summary>
    ///     Represents some sample options for the application
    /// </summary>
    public class BuilderOptions
    {
        /// <summary>
        ///     Gets or sets Value1
        /// </summary>
        public string Value1 { get; set; }

        /// <summary>
        ///     Gets or sets Value2
        /// </summary>
        public string Value2 { get; set; }

        /// <summary>
        ///     Gets or sets Value3
        /// </summary>
        public string Value3 { get; set; }

        /// <summary>
        ///     Validates whether the options
        /// </summary>
        public void Validate()
        {
        }
    }
}
