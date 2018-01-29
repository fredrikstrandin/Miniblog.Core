using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vivus.Model.Setting;
using Vivus.Model.User;

namespace Vivius.Repository.Qeue
{
    public class AzureQeueMessageRepository : IQeueMessageRepository
    {
        private readonly CloudStorageAccountSetting _cloudStorageAccountSetting;

        public AzureQeueMessageRepository(IOptions<CloudStorageAccountSetting> cloudStorageAccountSetting)
        {
            _cloudStorageAccountSetting = cloudStorageAccountSetting.Value;
        }

        public async Task SendCreateUser(RegisterUserModel messageObject)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(_cloudStorageAccountSetting.ConnectionString);

            // Create the queue client.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

            // Retrieve a reference to a queue.
            CloudQueue queue = queueClient.GetQueueReference(_cloudStorageAccountSetting.CreateUserQeue);

            // Create the queue if it doesn't already exist.
            await queue.CreateIfNotExistsAsync();

            // Create a message and add it to the queue.
            string serializedMessage = JsonConvert.SerializeObject(messageObject);
            CloudQueueMessage cloudQueueMessage = new CloudQueueMessage(serializedMessage);

            await queue.AddMessageAsync(cloudQueueMessage);
        }
    }
}
