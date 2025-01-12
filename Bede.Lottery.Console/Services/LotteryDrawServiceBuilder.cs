namespace Bede.Lottery.Services
{
    using System;
    using Models;

    internal sealed partial class LotteryDrawServiceBuilder(
        ILogger<LotteryDrawServiceBuilder> logger,
        IRandomNumberService randomNumberService,
        LotteryModel model)
        : ILotteryDrawServiceBuilder
    {
        private readonly Dictionary<int, int> players = new(model.MaxPlayerCount);
        private int numberOfCpuPlayers;

        public ILotteryDrawServiceBuilder AddCpuPlayers()
        {
            if (this.numberOfCpuPlayers > 0)
            {
                throw new InvalidOperationException($"CPU players cannot be added more than once.");
            }

            int numberOfCpuPlayers = this.numberOfCpuPlayers = this.GetRandomNumberOfCpuPlayers();
            LogAddCpuPlayers(logger, numberOfCpuPlayers);

            for (int index = 0; index < numberOfCpuPlayers; index++)
            {
                int numberOfTickets = this.GetRandomNumberOfTickets();
                this.AddPlayer(numberOfTickets);
            }

            return this;
        }

        public ILotteryDrawServiceBuilder AddPlayer(int numberOfTickets)
        {
            if (this.players.Count >= model.MaxPlayerCount)
            {
                throw new InvalidOperationException($"Maximum number of players ({model.MaxPlayerCount}) exceeded.");
            }

            int playerNumber = this.players.Count + 1;
            this.players.Add(playerNumber, numberOfTickets);

            return this;
        }

        public ILotteryDrawService Build()
        {
            var drawService = new LotteryDrawService(randomNumberService, model, this.players);
            return drawService;
        }

        private int GetRandomNumberOfTickets()
        {
            int randomNumberOfTickets = randomNumberService.Next(1, model.MaxNumberOfTickets);
            return randomNumberOfTickets;
        }

        private int GetRandomNumberOfCpuPlayers()
        {
            int minNumberOfCpuPlayers = model.MinPlayerCount - this.players.Count;
            int maxNumberOfCpuPlayers = model.MaxPlayerCount - this.players.Count;
            int randomNumberOfCpuPlayers = randomNumberService.Next(minNumberOfCpuPlayers, maxNumberOfCpuPlayers);
            return randomNumberOfCpuPlayers;
        }

        [LoggerMessage(Level = LogLevel.Debug, Message = "Adding {numberOfCpuPlayers} number of CPU players.")]
        static partial void LogAddCpuPlayers(ILogger<LotteryDrawServiceBuilder> logger, int numberOfCpuPlayers);
    }
}
