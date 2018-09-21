using System;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.Compute.Fluent.Models;
using Microsoft.Azure.Management.Fluent;
using Microsoft.Extensions.Logging;

namespace azure_image_builder.Core
{

    class AzureCloudManager
    {
        private string SubscriptionId { get; }
        private string ClientId { get; }
        private string ClientKey { get; }
        private string TenantId { get; }
        private readonly Region _location = Region.USWest;

        private ImageInfo ImageInfo { get; set; }
        private ILogger Logger { get; }
        
        private string GroupName { get; set; }
        private IAzure Azure { get; set; }
            
        public AzureCloudManager(ILogger logger, string subscriptionId, string clientId, string clientKey, string tenantId)
        {
            Logger = logger;
            SubscriptionId = subscriptionId;
            ClientId = clientId;
            ClientKey = clientKey;
            TenantId = tenantId;
        }
        
        public void CreateVmImage(ImageInfo imageInfo)
        {
            ImageInfo = imageInfo;
            Authenticate();    
            CreateVm();
            CreateImage();
        }

        private void Authenticate()
        {
            try
            {
                var credentials = SdkContext.AzureCredentialsFactory
                    .FromServicePrincipal(ClientId,
                            ClientKey,
                            TenantId,
                            AzureEnvironment.AzureGlobalCloud);

                Azure = Microsoft.Azure.Management.Fluent.Azure
                    .Configure()
                    .WithLogLevel(HttpLoggingDelegatingHandler.Level.Basic)
                    .Authenticate(credentials)
                    .WithSubscription(SubscriptionId);
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw;
            }
        }


        private void CreateVm()
        {
            try 
            {
                var guidVm = Guid.NewGuid().ToString();
                GroupName = guidVm + "-rg";
                Logger.LogInformation("Creating Resource Group {0}", GroupName);
                var resourceGroup = Azure.ResourceGroups.Define(GroupName)
                    .WithRegion(_location)
                    .Create();

               Logger.LogInformation("Creating public IP address...");
                var publicIpAddress = Azure.PublicIPAddresses.Define(guidVm + "-pubip")
                    .WithRegion(_location)
                    .WithExistingResourceGroup(GroupName)
                    .WithStaticIP()
                    .Create();

               Logger.LogInformation("Creating virtual network...");
                var subnetName = guidVm + "-subnet"; 
                var network = Azure.Networks.Define(guidVm + "-vnet")
                    .WithRegion(_location)
                    .WithExistingResourceGroup(GroupName)
                    .WithAddressSpace("10.0.0.0/16")
                    .WithSubnet(subnetName, "10.0.0.0/24")
                    .Create();

               Logger.LogInformation("Creating network interface...");
                var networkInterface = Azure.NetworkInterfaces.Define(guidVm + "-NIC")
                    .WithRegion(_location)
                    .WithExistingResourceGroup(GroupName)
                    .WithExistingPrimaryNetwork(network)
                    .WithSubnet(subnetName)
                    .WithPrimaryPrivateIPAddressDynamic()
                    .Create();

                ImageInfo.VMName = guidVm.Substring(0,12) + "-vm";
                Logger.LogInformation("Creating virtual machine...{0}", ImageInfo.VMName);                
                 
                 Azure.VirtualMachines.Define(ImageInfo.VMName)
                    .WithRegion(_location)
                    .WithExistingResourceGroup(GroupName)
                    .WithExistingPrimaryNetworkInterface(networkInterface)
                    .WithLatestWindowsImage(ImageInfo.VMPublisher, ImageInfo.VMOffer, ImageInfo.VMSKU)
                    .WithAdminUsername(ImageInfo.AdminUser)
                    .WithAdminPassword(ImageInfo.AdminPW)
                    .WithComputerName(ImageInfo.ComputerName)
                    .WithSize(ImageInfo.VMSizeType)
                    .Create();
                
               Logger.LogInformation("Creating virtual machine...{0} Complete!", ImageInfo.VMName);

               Logger.LogInformation("Deallocating and Generalize virtual machine...{0}", ImageInfo.VMName);
                var vm = Azure.VirtualMachines.GetByResourceGroup(GroupName, ImageInfo.VMName);
                vm.Deallocate();
                vm.Generalize();

               Logger.LogInformation("Getting information about the virtual machine...");
               Logger.LogInformation("hardwareProfile");
               Logger.LogInformation("   vmSize: " + vm.Size);
               Logger.LogInformation("storageProfile");
               Logger.LogInformation("  imageReference");
               Logger.LogInformation("    publisher: " + vm.StorageProfile.ImageReference.Publisher);
               Logger.LogInformation("    offer: " + vm.StorageProfile.ImageReference.Offer);
               Logger.LogInformation("    sku: " + vm.StorageProfile.ImageReference.Sku);
               Logger.LogInformation("    version: " + vm.StorageProfile.ImageReference.Version);
               Logger.LogInformation("  osDisk");
               Logger.LogInformation("    osType: " + vm.StorageProfile.OsDisk.OsType);
               Logger.LogInformation("    name: " + vm.StorageProfile.OsDisk.Name);
               Logger.LogInformation("    createOption: " + vm.StorageProfile.OsDisk.CreateOption);
               Logger.LogInformation("    caching: " + vm.StorageProfile.OsDisk.Caching);
               Logger.LogInformation("osProfile");
               Logger.LogInformation("  computerName: " + vm.OSProfile.ComputerName);
               Logger.LogInformation("  adminUsername: " + vm.OSProfile.AdminUsername);
               Logger.LogInformation("  provisionVMAgent: " + vm.OSProfile.WindowsConfiguration.ProvisionVMAgent.Value);
               Logger.LogInformation("  enableAutomaticUpdates: " + vm.OSProfile.WindowsConfiguration.EnableAutomaticUpdates.Value);
               Logger.LogInformation("networkProfile");
                foreach (string nicId in vm.NetworkInterfaceIds)
                {
                   Logger.LogInformation("  networkInterface id: " + nicId);
                }
               Logger.LogInformation("disks");
                foreach (DiskInstanceView disk in vm.InstanceView.Disks)
                {
                   Logger.LogInformation("  name: " + disk.Name);
                   Logger.LogInformation("  statuses");
                    foreach (InstanceViewStatus stat in disk.Statuses)
                    {
                       Logger.LogInformation("    code: " + stat.Code);
                       Logger.LogInformation("    level: " + stat.Level);
                       Logger.LogInformation("    displayStatus: " + stat.DisplayStatus);
                       Logger.LogInformation("    time: " + stat.Time);
                    }
                }
               Logger.LogInformation("VM general status");
               Logger.LogInformation("  provisioningStatus: " + vm.ProvisioningState);
               Logger.LogInformation("  id: " + vm.Id);
               Logger.LogInformation("  name: " + vm.Name);
               Logger.LogInformation("  type: " + vm.Type);
               Logger.LogInformation("  location: " + vm.Region);
               Logger.LogInformation("VM instance status");
                foreach (InstanceViewStatus stat in vm.InstanceView.Statuses)
                {
                   Logger.LogInformation("  code: " + stat.Code);
                   Logger.LogInformation("  level: " + stat.Level);
                   Logger.LogInformation("  displayStatus: " + stat.DisplayStatus);
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                throw;
            }

        }

        public void CreateImage()
        {
           Logger.LogInformation("Creating Image...{0}", ImageInfo.VMName);

           var imageName = ImageInfo.ImageName + DateTime.Now.ToString("yyyy-mm-dd.HHmmss");
           var vm = Azure.VirtualMachines.GetByResourceGroup(GroupName, ImageInfo.VMName);
           var image = Azure.VirtualMachineCustomImages.Define(imageName)
                    .WithRegion(_location)
                    .WithExistingResourceGroup(ImageInfo.ImageGroup)
                    .FromVirtualMachine(vm)
                    .Create();
            Logger.LogInformation("Created Image...{0}", ImageInfo.VMName);

            // Delete Resource Group after Image is created
            Logger.LogInformation("Deleting Resource Group...{0}", GroupName);
            Azure.ResourceGroups.DeleteByName(GroupName);
        }

    }

}
