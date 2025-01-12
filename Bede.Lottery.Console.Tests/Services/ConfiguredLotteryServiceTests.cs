namespace Bede.Lottery.Services
{
    using Models;
    using ViewModels;

    [TestClass]
    public sealed class ConfiguredLotteryServiceTests
    {
        private readonly Mock<IConfiguration> configurationMock = new();
        private readonly Mock<ILotteryDrawServiceBuilderProvider> drawServiceBuilderProviderMock = new();
        private readonly ConfiguredLotteryService service;

        public ConfiguredLotteryServiceTests()
        {
            this.service = new ConfiguredLotteryService(
                this.configurationMock.Object,
                this.drawServiceBuilderProviderMock.Object
            );
        }

        [TestMethod]
        public void GetWelcomeViewModelTest()
        {
            var expectedViewModel = new WelcomeViewModel { Balance = 10, TicketPrice = 1 };
            this.MockWelcomeViewModelConfiguration(expectedViewModel);

            var viewModel = this.service.GetWelcomeViewModel();

            viewModel.Should().BeEquivalentTo(expectedViewModel);
        }

        [TestMethod]
        public void GetWelcomeViewModelWithMissingConfigurationTest()
        {
            this.MockGetLotteryConfigurationSection();

            var act = this.service.GetWelcomeViewModel;

            act.Should().Throw<KeyNotFoundException>();
        }

        [TestMethod]
        public void GetWelcomeViewModelWithInvalidBalanceConfigurationTest()
        {
            this.MockWelcomeViewModelConfiguration(new WelcomeViewModel { Balance = 0, TicketPrice = 1 });

            var act = this.service.GetWelcomeViewModel;

            act.Should().Throw<InvalidOperationException>().WithMessage(
                "Configuration value at 'Lottery:Balance' must be greater than 0.");
        }

        [TestMethod]
        public void GetWelcomeViewModelWithInvalidTicketPriceConfigurationTest()
        {
            this.MockWelcomeViewModelConfiguration(new WelcomeViewModel { Balance = 10, TicketPrice = 0 });

            var act = this.service.GetWelcomeViewModel;

            act.Should().Throw<InvalidOperationException>().WithMessage(
                "Configuration value at 'Lottery:TicketPrice' must be greater than 0.");
        }

        [TestMethod]
        public void GetWelcomeViewModelWithInvalidBalanceToTicketPriceConfigurationTest()
        {
            this.MockWelcomeViewModelConfiguration(new WelcomeViewModel { Balance = 1, TicketPrice = 10 });

            var act = this.service.GetWelcomeViewModel;

            act.Should().Throw<InvalidOperationException>().WithMessage(
                "Configuration value at 'Lottery:TicketPrice' = 10 cannot be greater than the value at 'Lottery:Balance' = 1");
        }

        [TestMethod]
        public void GetDrawViewModelTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    NumberOfWinningTickets = 1,
                    FractionOfRevenue = 0.5M
                },
                SecondTier = new LotteryModel.WinningsModel
                {
                    FractionOfWinningTickets = 0.1,
                    FractionOfRevenue = 0.3M
                },
                ThirdTier = new LotteryModel.WinningsModel
                {
                    FractionOfWinningTickets = 0.2,
                    FractionOfRevenue = 0.1M
                },
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };
            var lotteryDrawService = this.GetLotteryDrawServiceMock(drawModel.NumberOfTickets);
            lotteryDrawService.Setup(mock => mock.GetPlayersViewModel()).Returns(Array.Empty<DrawViewModel.PlayerViewModel>());
            lotteryDrawService.Setup(mock => mock.GetRewardViewModel(It.IsAny<Func<LotteryModel, LotteryModel.WinningsModel?>>()))
                .Returns(Mock.Of<DrawViewModel.RewardViewModel>());

            var viewModel = this.service.GetDrawViewModel(drawModel);

            viewModel.Should().BeEquivalentTo(new DrawViewModel
            {
                BalanceExceeded = false,
                Players = Array.Empty<DrawViewModel.PlayerViewModel>(),
                GrandPrize = Mock.Of<DrawViewModel.RewardViewModel>(),
                SecondTier = Mock.Of<DrawViewModel.RewardViewModel>(),
                ThirdTier = Mock.Of<DrawViewModel.RewardViewModel>()
            });
        }

        [TestMethod]
        public void GetDrawViewModelWithBalanceExceededTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    NumberOfWinningTickets = 1,
                    FractionOfRevenue = 0.5M
                },
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 15 };
            var lotteryDrawService = this.GetLotteryDrawServiceMock(lotteryModel.MaxNumberOfTickets);
            lotteryDrawService.Setup(mock => mock.GetPlayersViewModel()).Returns(Array.Empty<DrawViewModel.PlayerViewModel>());
            lotteryDrawService.Setup(mock => mock.GetRewardViewModel(It.IsAny<Func<LotteryModel, LotteryModel.WinningsModel?>>()))
                .Returns(Mock.Of<DrawViewModel.RewardViewModel>());

            var viewModel = this.service.GetDrawViewModel(drawModel);

            viewModel.Should().BeEquivalentTo(new DrawViewModel
            {
                BalanceExceeded = true,
                Players = Array.Empty<DrawViewModel.PlayerViewModel>(),
                GrandPrize = Mock.Of<DrawViewModel.RewardViewModel>(),
                SecondTier = Mock.Of<DrawViewModel.RewardViewModel>(),
                ThirdTier = Mock.Of<DrawViewModel.RewardViewModel>()
            });
        }

        [TestMethod]
        public void GetDrawViewModelWithInvalidWinningTicketsTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    FractionOfRevenue = 0.5M
                },
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:GrandPrize:NumberOfWinningTickets' or " +
                    "'Lottery:GrandPrize:FractionOfWinningTickets' must be set.");
        }

        [TestMethod]
        public void GetDrawViewModelWithInvalidMinPlayerCountTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = -10,
                MaxPlayerCount = 15,
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:MinPlayerCount' must be greater than 0.");
        }

        [TestMethod]
        public void GetDrawViewModelWithInvalidMaxPlayerCountTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = -15,
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:MaxPlayerCount' must be greater than 0.");
        }

        [TestMethod]
        public void GetDrawViewModelWithMoreMinThanMaxPlayerCountTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 15,
                MaxPlayerCount = 10,
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:MinPlayerCount' = 15 cannot be greater than the value at 'Lottery:MaxPlayerCount' = 10");
        }

        [TestMethod]
        public void GetDrawViewModelWithNegativeNumberOfWinningTicketsTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    NumberOfWinningTickets = -1,
                    FractionOfRevenue = 0.5M
                },
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:GrandPrize:NumberOfWinningTickets' must be greater than 0.");
        }

        [TestMethod]
        public void GetDrawViewModelWithNegativeFractionOfWinningTicketsTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    FractionOfWinningTickets = -1,
                    FractionOfRevenue = 0.5M
                },
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:GrandPrize:FractionOfWinningTickets' must be greater than 0.");
        }

        [TestMethod]
        public void GetDrawViewModelWithNegativeFractionOfRevenueTest()
        {
            var lotteryModel = new LotteryModel
            {
                Balance = 10,
                TicketPrice = 1,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    FractionOfWinningTickets = 1,
                    FractionOfRevenue = -0.5M
                },
            };
            this.MockLotteryModelConfiguration(lotteryModel);
            var drawModel = new DrawModel { NumberOfTickets = 5 };

            var act = () => this.service.GetDrawViewModel(drawModel);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage(
                    "Configuration value at 'Lottery:GrandPrize:FractionOfRevenue' must be greater than 0.");
        }

        private void MockWelcomeViewModelConfiguration(WelcomeViewModel viewModel)
        {
            this.MockGetLotteryConfigurationSection(
                (nameof(viewModel.Balance), $"{viewModel.Balance}"),
                (nameof(viewModel.TicketPrice), $"{viewModel.TicketPrice}")
            );
        }

        private void MockLotteryModelConfiguration(LotteryModel model)
        {
            this.MockGetLotteryConfigurationSection(
                (nameof(model.Balance), $"{model.Balance}"),
                (nameof(model.TicketPrice), $"{model.TicketPrice}"),
                (nameof(model.MinPlayerCount), $"{model.MinPlayerCount}"),
                (nameof(model.MaxPlayerCount), $"{model.MaxPlayerCount}"),
                (
                    $"{nameof(model.GrandPrize)}:{nameof(model.GrandPrize.NumberOfWinningTickets)}",
                    model.GrandPrize?.NumberOfWinningTickets?.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.GrandPrize)}:{nameof(model.GrandPrize.FractionOfWinningTickets)}",
                    model.GrandPrize?.FractionOfWinningTickets?.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.GrandPrize)}:{nameof(model.GrandPrize.FractionOfRevenue)}",
                    model.GrandPrize?.FractionOfRevenue.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.SecondTier)}:{nameof(model.SecondTier.NumberOfWinningTickets)}",
                    model.SecondTier?.NumberOfWinningTickets?.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.SecondTier)}:{nameof(model.SecondTier.FractionOfWinningTickets)}",
                    model.SecondTier?.FractionOfWinningTickets?.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.SecondTier)}:{nameof(model.SecondTier.FractionOfRevenue)}",
                    model.SecondTier?.FractionOfRevenue.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.ThirdTier)}:{nameof(model.ThirdTier.NumberOfWinningTickets)}",
                    model.ThirdTier?.NumberOfWinningTickets?.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.ThirdTier)}:{nameof(model.ThirdTier.FractionOfWinningTickets)}",
                    model.ThirdTier?.FractionOfWinningTickets?.ToString(CultureInfo.InvariantCulture)
                ),
                (
                    $"{nameof(model.ThirdTier)}:{nameof(model.ThirdTier.FractionOfRevenue)}",
                    model.ThirdTier?.FractionOfRevenue.ToString(CultureInfo.InvariantCulture)
                )
            );
        }

        private void MockGetLotteryConfigurationSection(params IEnumerable<(string Key, string? Value)> initialData)
        {
            const string lotterySectionKey = ConfiguredLotteryService.LotterySectionKey;
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddInMemoryCollection(
                initialData.Where(pair => pair.Value is not null).ToDictionary(
                    pair => $"{lotterySectionKey}:{pair.Key}",
                    pair => pair.Value));

            var configuration = configurationBuilder.Build();
            var section = new ConfigurationSection(configuration, lotterySectionKey);

            this.configurationMock.Setup(mock => mock.GetSection(lotterySectionKey)).Returns(section);
        }

        private Mock<ILotteryDrawService> GetLotteryDrawServiceMock(int numberOfTickets)
        {
            var lotteryDrawServiceBuilderMock = new Mock<ILotteryDrawServiceBuilder>();
            this.drawServiceBuilderProviderMock.Setup(mock => mock.CreateLotteryDrawServiceBuilder(It.IsAny<LotteryModel>()))
                .Returns(lotteryDrawServiceBuilderMock.Object);
            lotteryDrawServiceBuilderMock.Setup(mock => mock.AddPlayer(numberOfTickets))
                .Returns(lotteryDrawServiceBuilderMock.Object);
            lotteryDrawServiceBuilderMock.Setup(mock => mock.AddCpuPlayers())
                .Returns(lotteryDrawServiceBuilderMock.Object);
            var lotteryDrawService = new Mock<ILotteryDrawService>();
            lotteryDrawServiceBuilderMock.Setup(mock => mock.Build())
                .Returns(lotteryDrawService.Object);

            return lotteryDrawService;
        }
    }
}
