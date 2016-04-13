namespace RaffleCommon
{
    using System;
    using System.Collections.Generic;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Queue;
    using Microsoft.WindowsAzure.Storage.Table;

    public class Raffle
    {
        public Raffle()
        {
            this.Status = RaffleStatus.NotStarted;
            this.RaffleId = Guid.NewGuid();
            this.Bets = new Dictionary<int, int>();
            this.NextTicketNumber = 1;
        }

        public Dictionary<int, int> Bets { get; }

        public Guid RaffleId { get; }

        public RaffleStatus Status { get; private set; }

        public int NextTicketNumber { get; private set; }

        public void Start()
        {
            if (this.Status == RaffleStatus.Running)
            {
                throw new InvalidOperationException("Raffle is already running.");
            }

            this.Status = RaffleStatus.Running;
        }

        public void Draw()
        {
            if (this.Status == RaffleStatus.Completed)
            {
                throw new InvalidOperationException("Raffle is already completed.");
            }

            if (this.Status == RaffleStatus.NotStarted)
            {
                throw new InvalidOperationException("Raffle is not started.");
            }

            this.InsertBets();
            this.PostDrawRaffleMessage();
            this.Status = RaffleStatus.Completed;
        }

        private void PostDrawRaffleMessage()
        {
            var queue = this.CreateQueueReference("drawraffle");

            queue.CreateIfNotExists();

            var message = new CloudQueueMessage(this.RaffleId.ToString());
            queue.AddMessage(message);
        }

        private void InsertBets()
        {
            if (this.Bets.Count == 0)
            {
                return;
            }

            var tableBets = this.CreateTableClient("Bet");

            tableBets.CreateIfNotExists();

            var batchOperation = new TableBatchOperation();

            foreach (var kvp in this.Bets)
            {
                var bet = new BetEntity();
                bet.TicketNumber = kvp.Key;
                bet.BetNumber = kvp.Value;
                bet.RaffleId = this.RaffleId;

                batchOperation.Insert(bet);
            }

            tableBets.ExecuteBatch(batchOperation);
        }

        public void PlaceBet(int betNumber)
        {
            if (this.Status == RaffleStatus.NotStarted)
            {
                throw new InvalidOperationException("Raffle is not yet running.");
            }

            if (this.Status == RaffleStatus.Completed)
            {
                throw new InvalidOperationException("Raffle is already completed.");
            }

            if (betNumber < 1 || betNumber > 6)
            {
                throw new ArgumentOutOfRangeException("Bets must be between 1 and 6 inclusive.");
            }

            this.Bets[this.NextTicketNumber++] = betNumber;
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
