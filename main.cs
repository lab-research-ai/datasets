using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Storage;
using Azure.Storage.Blobs;
using System;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var credential = new DefaultAzureCredential();
        var armClient = new ArmClient(credential);

        
        await foreach (var sub in armClient.GetSubscriptions().GetAllAsync())
        {
            Console.WriteLine($"Subscription: {sub.Data.DisplayName}");

            // Get all dataset storages
            var storageAccounts = sub.GetStorageAccounts();
            await foreach (var storageAccount in storageAccounts.GetAllAsync())
            {
                Console.WriteLine($"\nStorage Account: {storageAccount.Data.Name}");

                // Get blob service
                var blobService = await storageAccount.GetBlobService().GetAsync();

                // List containers
                await foreach (var container in blobService.Value.GetBlobContainers().GetAllAsync())
                {
                    Console.WriteLine($"  Container: {container.Data.Name}");

                    
                    string blobEndpoint = storageAccount.Data.PrimaryEndpoints.BlobUri.ToString();
                    var containerClient = new BlobContainerClient(new Uri($"{blobEndpoint}{container.Data.Name}"), credential);

                    // List blobs
                    await foreach (var blobItem in containerClient.GetBlobsAsync())
                    {
                        Console.WriteLine($"    Blob: {blobItem.Name}");
                    }
                }
            }
        }
    }
}
