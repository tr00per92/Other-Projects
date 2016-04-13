namespace WebRoleTest
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class CustomerEntity : TableEntity
    {
        private string firstName;

        private string lastName;

        public string FirstName
        {
            get { return this.firstName; }
            set
            {
                this.firstName = value;
                this.PartitionKey = this.firstName;
            }
        }

        public string LastName
        {
            get { return this.lastName; }
            set
            {
                this.lastName = value;
                this.RowKey = this.lastName;
            }
        }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }
    }
}