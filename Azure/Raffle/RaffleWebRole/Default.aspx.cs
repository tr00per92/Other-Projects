namespace RaffleWebRole
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.UI;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using RaffleCommon;

    public partial class Default : Page
    {
        private RaffleSessionManager raffleSessionManager;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.raffleSessionManager = new RaffleSessionManager();
            this.UpdatePageControls();
        }

        protected void btnStartStopRaffle_OnClick(object sender, EventArgs e)
        {
            switch (this.raffleSessionManager.CurrentSessionRaffle.Status)
            {
                case RaffleStatus.NotStarted:
                    this.raffleSessionManager.CurrentSessionRaffle.Start();
                    break;
                case RaffleStatus.Running:
                    this.raffleSessionManager.CurrentSessionRaffle.Draw();
                    break;
                case RaffleStatus.Completed:
                    this.raffleSessionManager.RemoveRaffleFromCurrentSession();
                    // Reload the current page
                    this.Response.Redirect(this.Request.RawUrl);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("RaffleStatus out of range.");
            }

            this.UpdatePageControls();
        }

        private void UpdatePageControls()
        {
            switch (this.raffleSessionManager.CurrentSessionRaffle.Status)
            {
                case RaffleStatus.NotStarted:
                    this.labelRaffleStatus.Text = "Not started";
                    this.btnStartStopRaffle.Text = "Start the raffle";
                    this.labelMessage.Text = string.Empty;
                    this.labelRaffleId.Text = this.raffleSessionManager.CurrentSessionRaffle.RaffleId.ToString();
                    this.divPlaceBet.Visible = false;
                    this.divResult.Visible = false;
                    break;
                case RaffleStatus.Running:
                    this.labelRaffleStatus.Text = "Running";
                    this.btnStartStopRaffle.Text = "Draw";
                    this.labelMessage.Text = "Accepting bets. Place bets from 1 to 6 inclusive.";
                    this.labelRaffleId.Text = this.raffleSessionManager.CurrentSessionRaffle.RaffleId.ToString();
                    this.divPlaceBet.Visible = true;
                    this.divResult.Visible = false;
                    break;
                case RaffleStatus.Completed:
                    this.labelRaffleStatus.Text = "Completed";
                    this.btnStartStopRaffle.Text = "Create new raffle";
                    this.labelMessage.Text = "Raffle bets";
                    this.labelRaffleId.Text = this.raffleSessionManager.CurrentSessionRaffle.RaffleId.ToString();
                    this.divPlaceBet.Visible = false;
                    this.divResult.Visible = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("RaffleStatus out of range.");
            }

            this.labelBetTicketNumber.Text = this.raffleSessionManager.CurrentSessionRaffle.NextTicketNumber.ToString();

            if (!this.Page.IsPostBack)
            {
                this.tbBetNumber.Text = string.Empty;
            }

            this.lbBets.DataSource = this.raffleSessionManager
                                         .CurrentSessionRaffle
                                         .Bets
                                         .Select(bet => string.Format("Ticket #: {0} Bet: {1}", bet.Key, bet.Value));
            this.lbBets.DataBind();
        }

        protected void btnPlaceBet_OnClick(object sender, EventArgs e)
        {
            var betNumber = int.Parse(this.tbBetNumber.Text);
            this.tbBetNumber.Text = string.Empty;

            this.raffleSessionManager.CurrentSessionRaffle.PlaceBet(betNumber);

            this.UpdatePageControls();
        }

        protected void btnGetResults_OnClick(object sender, EventArgs e)
        {
            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient();
            var table = tableClient.GetTableReference("RaffleResult");
            var retrieveOperation = TableOperation.Retrieve<RaffleResultEntity>(
                "RaffleResult",
                this.raffleSessionManager.CurrentSessionRaffle.RaffleId.ToString());

            var retrievedResult = table.Execute(retrieveOperation);

            if (retrievedResult.Result == null)
            {
                this.labelResult.Text = "Result not available yet.";
            }
            else
            {
                var raffleResult = retrievedResult.Result as RaffleResultEntity;
                this.labelResult.Text = string.Format(
                    "Winning number: {0}; Winning ticket numbers: {1}",
                    raffleResult.WinningNumber,
                    raffleResult.WinningTicketNumbers);
            }
        }
    }
}
