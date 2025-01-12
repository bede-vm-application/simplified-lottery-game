namespace Bede.Lottery.ViewModels
{
    public record DrawViewModel
    {
        public bool BalanceExceeded { get; init; }
        public required ICollection<PlayerViewModel> Players { get; init; }
        public required RewardViewModel? GrandPrize { get; init; }
        public required RewardViewModel? SecondTier { get; init; }
        public required RewardViewModel? ThirdTier { get; init; }
        public decimal HouseRevenue { get; init; }

        public record PlayerViewModel
        {
            public required int PlayerNumber { get; init; }
            public int NumberOfTickets { get; init; }
        }

        public record RewardViewModel
        {
            public required ICollection<int> PlayerNumbers { get; init; }
            public decimal Amount { get; init; }
        }
    }
}
