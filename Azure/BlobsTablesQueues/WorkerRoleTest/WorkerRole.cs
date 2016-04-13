namespace WorkerRoleTest
{
    using System.Diagnostics;
    using System.Net;
    using System.Threading;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;

    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("WorkerRole1 entry point called", "Information");

            while (true)
            {
                Thread.Sleep(5000);

                // Retrieve storage account from connection string
                //string localStorageSetting = CloudConfigurationManager.GetSetting("LocalStorage");
                // CloudStorageAccount storageAccount = CloudStorageAccount.Parse(localStorageSetting);

                var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                //Debug.WriteLine("PMP Local St!");

                // Create the queue client
                var queueClient = storageAccount.CreateCloudQueueClient();

                // Retrieve a reference to a queue
                var queue = queueClient.GetQueueReference("myqueue");
                queue.CreateIfNotExists();

                // Get the next message
                var retrievedMessage = queue.GetMessage();

                if (retrievedMessage != null)
                {
                    Debug.WriteLine(retrievedMessage.AsString);
                    queue.DeleteMessage(retrievedMessage);
                }
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            return base.OnStart();
        }
    }
}
