namespace RaffleCommon
{
    using System;
    using Microsoft.WindowsAzure.Storage.Table;

    public class BetEntity : TableEntity
    {
        private Guid raffleId;

        private int ticketNumber;

        public Guid RaffleId
        {
            get { return this.raffleId; }
            set
            {
                this.raffleId = value;
                this.PartitionKey = this.raffleId.ToString();
            }
        }

        public int TicketNumber
        {
            get { return this.ticketNumber; }
            set
            {
                this.ticketNumber = value;
                this.RowKey = this.ticketNumber.ToString();
            }
        }

        public int BetNumber { get; set; }
    }
}
