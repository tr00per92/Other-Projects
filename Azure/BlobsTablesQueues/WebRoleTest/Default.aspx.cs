namespace WebRoleTest
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    using System.Web.UI;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;

    public partial class Default : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var process = Process.GetCurrentProcess();
            Debug.WriteLine("{0}: {1}: {2}", process.ProcessName, process.Id, Thread.CurrentThread.ManagedThreadId);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            var storageAccount = this.GetCloudStorageAccount();

            // Create the queue client
            var queueClient = storageAccount.CreateCloudQueueClient();

            var queue = queueClient.GetQueueReference("myqueue");

            // Create the queue if it doesn't already exist
            queue.CreateIfNotExists();

            var message = new CloudQueueMessage("Hello, World");
            queue.AddMessage(message);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            this.UploadBlob();
        }

        private void UploadBlob()
        {
            var storageAccount = this.GetCloudStorageAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            var container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference("myblob");

            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = File.OpenRead(@"C:\Users\Tr00peR\Desktop\BlobsTablesQueues\1.jpg"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            //blockBlob = container.GetBlockBlobReference("myblob1");
            //using (var fileStream = File.OpenRead(@"C:\NBU\P2014\WorkPMP\DSC_3938.JPG"))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //}

            //blockBlob = container.GetBlockBlobReference("myblob2");
            //using (var fileStream = File.OpenRead(@"C:\NBU\P2014\WorkPMP\Google.App.Engine.PDF"))
            //{
            //    blockBlob.UploadFromStream(fileStream);
            //}
        }

        protected void Button3_Click(object sender, EventArgs e)
        {
            this.DownloadBlob();
        }

        private void DownloadBlob()
        {
            var storageAccount = this.GetCloudStorageAccount();

            var blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container. 
            var container = blobClient.GetContainerReference("mycontainer");

            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Retrieve reference to a blob named "myblob".
            var blockBlob = container.GetBlockBlobReference("myblob");

            //================== PMP ========================
            // Loop over items within the container and output the length and URI.
            //foreach (IListBlobItem item in container.ListBlobs())
            //{
            //    if (item is CloudBlockBlob)
            //    {
            //        CloudBlockBlob blob = (CloudBlockBlob)item;

            //        Debug.WriteLine("Block blob of length {0}: {1}", blob.Properties.Length, blob.Uri);

            //    }
            //    else if (item is CloudPageBlob)
            //    {
            //        CloudPageBlob pageBlob = (CloudPageBlob)item;

            //        Debug.WriteLine("Page blob of length {0}: {1}", pageBlob.Properties.Length, pageBlob.Uri);

            //    }
            //    else if (item is CloudBlobDirectory)
            //    {
            //        var directory = (CloudBlobDirectory)item;

            //        Debug.WriteLine("Directory: {0}", directory.Uri);
            //    }
            //}
            //================== PMP END ====================

            var path = Path.Combine(@"C:\", @"Users\Tr00peR\Desktop\testblobpicture1.jpg");
            using (var fileStream = File.OpenWrite(path))
            {
                blockBlob.DownloadToStream(fileStream);
            }

            //blockBlob = container.GetBlockBlobReference("myblob1");
            //using (var fileStream = File.OpenWrite(@"C:\NBU\P2014\WorkPMP\testblobpicture2.jpg"))
            //{
            //    blockBlob.DownloadToStream(fileStream);
            //}

            //blockBlob = container.GetBlockBlobReference("myblob2");
            //using (var fileStream = File.OpenWrite(@"C:\NBU\P2014\WorkPMP\testblobbook.pdf"))
            //{
            //    blockBlob.DownloadToStream(fileStream);
            //}

            //// Delete the blob.
            //blockBlob.Delete();
        }

        protected void Button4_Click(object sender, EventArgs e)
        {
            var storageAccount = this.GetCloudStorageAccount();

            var tableClient = storageAccount.CreateCloudTableClient();

            var table = tableClient.GetTableReference("people");

            //table.DeleteIfExists();
            table.CreateIfNotExists();

            var customer1 = this.CreateSampleCustomer();

            // Create the TableOperation that inserts the customer entity.
            var insertOperation = TableOperation.InsertOrReplace(customer1);

            // Execute the insert operation.
            table.Execute(insertOperation);
        }

        protected void Button5_Click(object sender, EventArgs e)
        {
            var storageAccount = this.GetCloudStorageAccount();
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("people");
            var retrieveOperation = TableOperation.Retrieve<CustomerEntity>("Ivan", "Georgiev");
            var retrievedResult = table.Execute(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                var customer = retrievedResult.Result as CustomerEntity;

                Debug.WriteLine(
                    "{0} {1} {2} {3}",
                    customer.FirstName,
                    customer.LastName,
                    customer.Email,
                    customer.PhoneNumber);
            }
            else
            {
                Debug.WriteLine("The customer 'Ivan Georgiev' could not be found.");
            }
        }

        private CloudStorageAccount GetCloudStorageAccount()
        {
            return CloudStorageAccount.DevelopmentStorageAccount;

            // Old way to create development CloudStorageAccount
            // Doesn't work on VS 2010 with Microsoft.WindowsAzure.Storage 2.0.0.0

            //string localStorageSetting = CloudConfigurationManager.GetSetting("LocalStorage");
            //CloudStorageAccount storageAccount = CloudStorageAccount.Parse(localStorageSetting);
            //return storageAccount;
        }

        private CustomerEntity CreateSampleCustomer()
        {
            return new CustomerEntity
            {
                FirstName = "Ivan",
                LastName = "Georgiev",
                Email = "Ivan.Georgiev@AzureCloudSample.com",
                PhoneNumber = "555-555-5555"
            };
        }
    }
}
