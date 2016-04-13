namespace RaffleWebRole
{
    using System.Web;
    using RaffleCommon;

    public class RaffleSessionManager
    {
        public Raffle CurrentSessionRaffle
        {
            get
            {
                var raffle = HttpContext.Current.Session[this.RaffleKey] as Raffle;

                if (raffle == null)
                {
                    raffle = new Raffle();
                    HttpContext.Current.Session[this.RaffleKey] = raffle;
                }

                return raffle;
            }
        }

        private string RaffleKey
        {
            get
            {
                var sessionId = HttpContext.Current.Session.SessionID;
                var raffleKey = string.Format("{0}-{1}", "Raffle", sessionId);

                return raffleKey;
            }
        }

        public void RemoveRaffleFromCurrentSession()
        {
            HttpContext.Current.Session[this.RaffleKey] = null;
        }
    }
}
