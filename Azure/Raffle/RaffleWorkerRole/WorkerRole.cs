namespace RaffleWorkerRole
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Threading;
    using Microsoft.WindowsAzure.ServiceRuntime;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;
    using RaffleCommon;

    public class WorkerRole : RoleEntryPoint
    {
        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.WriteLine("RaffleWorkerRole entry point called", "Information");

            while (true)
            {
                Thread.Sleep(15000);

                var drawRaffleMessage = this.ReadDrawRaffleMessage();

                if (drawRaffleMessage != null)
                {
                    Guid raffleId;
                    RaffleResultEntity raffleResult;

                    try
                    {
                        raffleId = Guid.Parse(drawRaffleMessage.AsString);

                        Trace.WriteLine("Processing Raffle id: " + raffleId);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Cannot parse RaffleId from message." + ex.Message);
                        this.DeleteDrawRaffleMessage(drawRaffleMessage);

                        return;
                    }

                    try
                    {
                        raffleResult = this.CreateRaffleResultEntity(raffleId);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error creating raffle result." + ex.Message);

                        raffleResult = new RaffleResultEntity();
                        raffleResult.RaffleId = raffleId;
                        raffleResult.WinningNumber = -1;
                        raffleResult.WinningTicketNumbers = "Error in draw. " + ex.Message;
                    }

                    try
                    {
                        Trace.WriteLine("Raffle Result for id: " + raffleId);
                        Trace.WriteLine("Winning number: " + raffleResult.WinningNumber);
                        Trace.WriteLine("Winning tickets: " + raffleResult.WinningTicketNumbers);

                        this.SaveRaffleResult(raffleResult);
                        this.DeleteDrawRaffleMessage(drawRaffleMessage);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Error saving raffle result. " + ex.Message);
                    }
                }
            }
        }

        private RaffleResultEntity CreateRaffleResultEntity(Guid raffleId)
        {
            var raffleResult = new RaffleResultEntity();
            raffleResult.RaffleId = raffleId;

            var bets = this.ReadBetEntities(raffleId);

            var random = new Random();
            raffleResult.WinningNumber = random.Next(1, 6);

            var winningTicketNumbers = new List<int>();
            foreach (var bet in bets)
            {
                if (bet.BetNumber == raffleResult.WinningNumber)
                {
                    winningTicketNumbers.Add(bet.TicketNumber);
                }
            }

            if (winningTicketNumbers.Count > 0)
            {
                raffleResult.WinningTicketNumbers = string.Join(", ", winningTicketNumbers);
            }
            else
            {
                raffleResult.WinningTicketNumbers = "No winning tickets";
            }

            return raffleResult;
        }

        private void DeleteDrawRaffleMessage(CloudQueueMessage drawRaffleMessage)
        {
            var queue = this.CreateQueueReference("drawraffle");

            if (!queue.Exists())
            {
                return;
            }

            try
            {
                queue.DeleteMessage(drawRaffleMessage);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Cannot delete message. " + ex.Message);
            }
        }

        private void SaveRaffleResult(RaffleResultEntity raffleResult)
        {
            var tableRaffleResults = this.CreateTableClient("RaffleResult");

            tableRaffleResults.CreateIfNotExists();

            var insertOperation = TableOperation.Insert(raffleResult);
            tableRaffleResults.Execute(insertOperation);
        }

        private List<BetEntity> ReadBetEntities(Guid raffleId)
        {
            Trace.WriteLine("Bets:");

            var tableBets = this.CreateTableClient("Bet");

            if (!tableBets.Exists())
            {
                return new List<BetEntity>();
            }

            var query = new TableQuery<BetEntity>()
                .Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, raffleId.ToString()));

            var betsForRaffle = tableBets.ExecuteQuery(query).ToList();

            foreach (var bet in betsForRaffle)
            {
                Trace.WriteLine(string.Format("Ticket: {0}; Bet Number: {1}", bet.TicketNumber, bet.BetNumber));
            }

            return betsForRaffle;
        }

        private CloudQueueMessage ReadDrawRaffleMessage()
        {
            var queue = this.CreateQueueReference("drawraffle");

            if (!queue.Exists())
            {
                return null;
            }

            var retrievedMessage = queue.GetMessage();
            return retrievedMessage;
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            Trace.AutoFlush = true;

            this.InitializeStorage();

            return base.OnStart();
        }

        private void InitializeStorage()
        {
            File.Delete(@"C:\RaffleWorkerRole.log");

            var queue = this.CreateQueueReference("drawraffle");
            if (queue.Exists())
            {
                queue.Clear();
            }

            var tableBets = this.CreateTableClient("Bet");
            tableBets.DeleteIfExists();

            var tableRaffleResults = this.CreateTableClient("RaffleResult");
            tableRaffleResults.DeleteIfExists();
        }

        // Queue name must be lowercase - Bad Request error is returned if queue with uppercase letter in the name is created
        private CloudQueue CreateQueueReference(string queueName)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;

            var queueClient = storageAccount.CreateCloudQueueClient();
            var queue = queueClient.GetQueueReference(queueName);

            return queue;
        }

        private CloudTable CreateTableClient(string tableName)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference(tableName);

            return table;
        }
    }
}
