using System;
using System.IO;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace azure_image_builder.Core
{
    public class ImageInfo
    {
        public string Name { get; set; }
        public string OSType { get; set; }
        public string Location { get; set; }
        public string AdminUser { get; set; }
        public string AdminPW { get; set; }
        public string ImageName { get; set; }
        public string ImageGroup { get; set; }
        public string ComputerName { get; set; }
        public string VMName { get; set; }
        public string VMPublisher { get; set; }
        public string VMOffer { get; set; }
        public string VMSKU { get; set; }
        public string VMSizeType { get; set; }
    }
    
    class ImageConfiguration
    {
        
        public ImageInfo GetImageInfo()
        {
            var yamlFile = Path.Combine(Environment.CurrentDirectory, @"App_Data\Image.yaml");
            TextReader input = new StreamReader(yamlFile);

            var deserializer = new DeserializerBuilder()
                .Build();

           var imageInfo = deserializer.Deserialize<ImageInfo>(input);

            return imageInfo;
        }
    }
}
