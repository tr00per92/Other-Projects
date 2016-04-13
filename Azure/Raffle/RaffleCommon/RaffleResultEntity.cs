namespace RaffleCommon
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class RaffleResultEntity : TableEntity
    {
        private Guid raffleId;

        public Guid RaffleId
        {
            get { return this.raffleId; }
            set
            {
                this.raffleId = value;
                this.PartitionKey = "RaffleResult";
                this.RowKey = this.raffleId.ToString();
            }
        }

        public int WinningNumber { get; set; }

        public string WinningTicketNumbers { get; set; }
    }
}
