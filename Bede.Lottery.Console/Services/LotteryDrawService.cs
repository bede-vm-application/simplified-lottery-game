namespace Bede.Lottery.Services
{
    using ViewModels;
    using Models;
    using Utilities;

    internal sealed partial class LotteryDrawService : ILotteryDrawService
    {
        private readonly IRandomNumberService randomNumberService;
        private readonly LotteryModel model;
        private readonly ReadOnlyDictionary<int, int> players;
        private readonly CumulativeTicketSum cumulativeTickets;
        private readonly int totalTickets;

        public LotteryDrawService(
            IRandomNumberService randomNumberService,
            LotteryModel model,
            Dictionary<int, int> players)
        {
            this.randomNumberService = randomNumberService;
            this.model = model;
            this.players = new ReadOnlyDictionary<int, int>(players);

            var cumulativeTicketSum = new CumulativeTicketSum(players.Values);
            int totalTickets = cumulativeTicketSum.RemainingTickets;
            decimal totalRevenue = totalTickets * model.TicketPrice;

            this.cumulativeTickets = cumulativeTicketSum;
            this.totalTickets = totalTickets;
            this.TotalRevenue = totalRevenue;
            this.HouseRevenue = totalRevenue;
        }

        public decimal TotalRevenue { get; }

        public decimal HouseRevenue { get; private set; }

        public ICollection<DrawViewModel.PlayerViewModel> GetPlayersViewModel()
        {
            var players = this.players.Select(pair => new DrawViewModel.PlayerViewModel
            {
                PlayerNumber = pair.Key,
                NumberOfTickets = pair.Value
            }).ToList();
            return players;
        }

        public DrawViewModel.RewardViewModel? GetRewardViewModel(Func<LotteryModel, LotteryModel.WinningsModel?> selector)
        {
            if (this.players.Count == 0)
            {
                throw new InvalidOperationException("Cannot get draw rewards with no players.");
            }

            var winningsModel = selector(this.model);
            if (winningsModel is null)
            {
                return null;
            }

            return this.GetRewardViewModel(winningsModel);
        }

        private DrawViewModel.RewardViewModel GetRewardViewModel(LotteryModel.WinningsModel winningsModel)
        {
            int numberOfWinningTickets = this.GetNumberOfWinningTickets(winningsModel);
            if (numberOfWinningTickets > this.cumulativeTickets.RemainingTickets)
            {
                throw new InvalidOperationException(
                    $"Cannot draw {numberOfWinningTickets} because the lottery has {this.cumulativeTickets.RemainingTickets} remaining.");
            }

            var winningPlayers = new SortedSet<int>();
            for (int index = 0; index < numberOfWinningTickets; index++)
            {
                int winningTicket = this.randomNumberService.Next(1, this.cumulativeTickets.RemainingTickets);
                var winningPlayerIndex = this.cumulativeTickets.GetWinningPlayerIndex(winningTicket);
                winningPlayers.Add(this.players.Keys.ElementAt(winningPlayerIndex));
                this.cumulativeTickets.DecrementTickets(winningPlayerIndex);
            }

            var individualTicketAmount = Math.Round(this.TotalRevenue * winningsModel.FractionOfRevenue / numberOfWinningTickets, 2);
            var individualAmount = Math.Round(individualTicketAmount * numberOfWinningTickets / winningPlayers.Count, 2);
            this.HouseRevenue -= individualAmount * winningPlayers.Count;

            var model = new DrawViewModel.RewardViewModel
            {
                PlayerNumbers = winningPlayers,
                Amount = individualAmount
            };
            return model;
        }

        private int GetNumberOfWinningTickets(LotteryModel.WinningsModel winningsModel)
        {
            if (winningsModel.NumberOfWinningTickets is int numberOfWinningTickets)
            {
                return numberOfWinningTickets;
            }

            if (winningsModel.FractionOfWinningTickets is double fractionOfWinningTickets)
            {
                return (int)Math.Round(fractionOfWinningTickets * this.totalTickets);
            }

            throw new InvalidOperationException("Cannot get number of winning tickets because neither configuration was set.");
        }
    }
}
