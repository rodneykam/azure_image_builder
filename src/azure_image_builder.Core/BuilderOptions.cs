// // Copyright 2018 Change Healthcare. All rights reserved.
// // See LICENSE in the project root for license information.

namespace azure_image_builder.Core
{
    /// <summary>
    ///     Represents some sample options for the application
    /// </summary>
    public class BuilderOptions
    {
        public string subscriptionid { get; set; }
        public string clientid { get; set; }
        public string clientkey { get; set; }
        public string tenantid { get; set; }
        public string datapath { get; set; }
        
        /// <summary>
        ///     Validates whether the options
        /// </summary>
        public void Validate()
        {
        }
    }
}
