using Azure.Storage.Blobs;

namespace PetStore.Pages.Common
{
    public class AzureBlobService
    {
        public async Task<string> UploadImageAsync(Stream fileStream, string fileName)
        {
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            string containerName = "images";
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync();
            var blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(fileStream, overwrite: true);
            return blobClient.Uri.ToString(); // Trả về URL của ảnh
        }
    }
}
