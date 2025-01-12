namespace Bede.Lottery.Services
{
    using Models;
    using ViewModels;

    internal sealed class ConfiguredLotteryService(
        IConfiguration configuration,
        ILotteryDrawServiceBuilderProvider drawServiceBuilderProvider)
        : ILotteryService
    {
        public const string LotterySectionKey = "Lottery";

        public WelcomeViewModel GetWelcomeViewModel()
        {
            var viewModel = this.GetConfiguredModel<WelcomeViewModel>();
            Validate(viewModel);
            return viewModel;
        }

        public DrawViewModel GetDrawViewModel(DrawModel drawModel)
        {
            var lotteryModel = this.GetConfiguredModel<LotteryModel>();
            Validate(lotteryModel);
            var viewModel = this.GetDrawViewModel(drawModel, lotteryModel);
            return viewModel;
        }

        private DrawViewModel GetDrawViewModel(DrawModel drawModel, LotteryModel lotteryModel)
        {
            var drawServiceBuilder = drawServiceBuilderProvider.CreateLotteryDrawServiceBuilder(lotteryModel);

            bool balanceExceeded = drawModel.NumberOfTickets > lotteryModel.MaxNumberOfTickets;
            int numberOfPlayerTickets = balanceExceeded ? lotteryModel.MaxNumberOfTickets : drawModel.NumberOfTickets;

            drawServiceBuilder.AddPlayer(numberOfPlayerTickets);
            drawServiceBuilder.AddCpuPlayers();
            var drawService = drawServiceBuilder.Build();

            var players = drawService.GetPlayersViewModel();
            var grandPrize = drawService.GetRewardViewModel(model => model.GrandPrize);
            var secondTier = drawService.GetRewardViewModel(model => model.SecondTier);
            var thirdTier = drawService.GetRewardViewModel(model => model.ThirdTier);

            var model = new DrawViewModel
            {
                BalanceExceeded = balanceExceeded,
                Players = drawService.GetPlayersViewModel(),
                GrandPrize = grandPrize,
                SecondTier = secondTier,
                ThirdTier = thirdTier,
                HouseRevenue = drawService.HouseRevenue
            };
            return model;
        }

        private TModel GetConfiguredModel<TModel>()
        {
            var section = configuration.GetSection(LotterySectionKey);
            if (section.Get<TModel>() is TModel model)
            {
                return model;
            }

            throw new KeyNotFoundException($"Missing configuration value at '{LotterySectionKey}' for type '{typeof(TModel)}.'");
        }

        private static void Validate(WelcomeViewModel model)
        {
            if (model.Balance <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{nameof(IBalanceModel.Balance)}' must be greater than 0.");
            }

            if (model.TicketPrice <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{nameof(ITicketPriceModel.TicketPrice)}' must be greater than 0.");
            }

            if (model.TicketPrice > model.Balance)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{nameof(ITicketPriceModel.TicketPrice)}' = {model.TicketPrice} " +
                    $"cannot be greater than the value at '{LotterySectionKey}:{nameof(IBalanceModel.Balance)}' = {model.Balance}");
            }
        }

        private static void Validate(LotteryModel model)
        {
            if (model.MinPlayerCount <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{nameof(LotteryModel.MinPlayerCount)}' must be greater than 0.");
            }

            if (model.MaxPlayerCount <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{nameof(LotteryModel.MaxPlayerCount)}' must be greater than 0.");
            }

            if (model.MinPlayerCount > model.MaxPlayerCount)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{nameof(LotteryModel.MinPlayerCount)}' = {model.MinPlayerCount} " +
                    $"cannot be greater than the value at '{LotterySectionKey}:{nameof(LotteryModel.MaxPlayerCount)}' = {model.MaxPlayerCount}");
            }

            Validate(model.GrandPrize, nameof(LotteryModel.GrandPrize));
            Validate(model.SecondTier, nameof(LotteryModel.SecondTier));
            Validate(model.ThirdTier, nameof(LotteryModel.ThirdTier));
        }

        private static void Validate(LotteryModel.WinningsModel? winningsModel, string subSectionName)
        {
            if (winningsModel is null)
            {
                return;
            }

            if (winningsModel.NumberOfWinningTickets is null && winningsModel.FractionOfWinningTickets is null)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{subSectionName}:{nameof(LotteryModel.WinningsModel.NumberOfWinningTickets)}' " +
                    $"or '{LotterySectionKey}:{subSectionName}:{nameof(LotteryModel.WinningsModel.FractionOfWinningTickets)}' must be set.");
            }

            if (winningsModel.NumberOfWinningTickets <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{subSectionName}:{nameof(LotteryModel.WinningsModel.NumberOfWinningTickets)}' must be greater than 0.");
            }

            if (winningsModel.FractionOfWinningTickets <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{subSectionName}:{nameof(LotteryModel.WinningsModel.FractionOfWinningTickets)}' must be greater than 0.");
            }

            if (winningsModel.FractionOfRevenue <= 0)
            {
                throw new InvalidOperationException(
                    $"Configuration value at '{LotterySectionKey}:{subSectionName}:{nameof(LotteryModel.WinningsModel.FractionOfRevenue)}' must be greater than 0.");
            }
        }
    }
}
