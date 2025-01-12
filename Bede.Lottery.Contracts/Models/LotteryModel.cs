namespace Bede.Lottery.Models
{
    public record LotteryModel : IBalanceModel, ITicketPriceModel
    {
        public decimal Balance { get; init; }
        public decimal TicketPrice { get; init; }
        public int MinPlayerCount { get; init; }
        public int MaxPlayerCount { get; init; }
        public WinningsModel? GrandPrize { get; init; }
        public WinningsModel? SecondTier { get; init; }
        public WinningsModel? ThirdTier { get; init; }

        public record WinningsModel
        {
            public int? NumberOfWinningTickets { get; init; }
            public double? FractionOfWinningTickets { get; init; }
            public decimal FractionOfRevenue { get; init; }
        }

        public int MaxNumberOfTickets
        {
            get => this.maxNumberOfTickets ??= (int)Math.Floor(this.Balance / this.TicketPrice);
        }

        private int? maxNumberOfTickets;
    }
}
