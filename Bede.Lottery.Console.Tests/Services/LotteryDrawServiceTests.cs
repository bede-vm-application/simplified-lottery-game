namespace Bede.Lottery.Services
{
    using Bede.Lottery.ViewModels;
    using Bede.Lottery.Views;
    using Models;

    [TestClass]
    public sealed class LotteryDrawServiceTests
    {
        private readonly Mock<IRandomNumberService> randomNumberServiceMock = new();

        [TestMethod]
        public void GetPlayersTest()
        {
            var model = new LotteryModel();
            var players = new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 10,
            };
            var service = new LotteryDrawService(this.randomNumberServiceMock.Object, model, players);

            var playersModel = service.GetPlayersViewModel();

            playersModel.Should().BeEquivalentTo([
                new DrawViewModel.PlayerViewModel { PlayerNumber = 1, NumberOfTickets = 5 },
                new DrawViewModel.PlayerViewModel { PlayerNumber = 2, NumberOfTickets = 10 },
            ]);
        }

        [TestMethod]
        public void GetRewardViewModelForGrandPrizeTest()
        {
            var model = new LotteryModel
            {
                TicketPrice = 1,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    NumberOfWinningTickets = 1,
                    FractionOfRevenue = 0.5M
                }
            };
            var players = new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 10,
                [3] = 7,
                [4] = 10,
                [5] = 3,
                [6] = 7,
                [7] = 2,
                [8] = 9,
                [9] = 3,
                [10] = 2,
            };
            var service = new LotteryDrawService(this.randomNumberServiceMock.Object, model, players);

            var viewModel = service.GetRewardViewModel(model => model.GrandPrize);

            viewModel.Should().BeEquivalentTo(new DrawViewModel.RewardViewModel
            {
                PlayerNumbers = [1],
                Amount = 29
            });
        }

        [TestMethod]
        public void GetRewardViewModelWithInvalidNumberOfTicketsTest()
        {
            var model = new LotteryModel
            {
                TicketPrice = 1,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    FractionOfRevenue = 0.5M
                }
            };
            var players = new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 10,
                [3] = 7,
                [4] = 10,
                [5] = 3,
                [6] = 7,
                [7] = 2,
                [8] = 9,
                [9] = 3,
                [10] = 2,
            };
            var service = new LotteryDrawService(this.randomNumberServiceMock.Object, model, players);

            var act = () => service.GetRewardViewModel(model => model.GrandPrize);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot get number of winning tickets because neither configuration was set.");
        }

        [TestMethod]
        public void GetRewardViewModelWithOverdrawTest()
        {
            var model = new LotteryModel
            {
                TicketPrice = 1,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    NumberOfWinningTickets = 1,
                    FractionOfRevenue = 0.5M
                },
                SecondTier = new LotteryModel.WinningsModel
                {
                    FractionOfWinningTickets = 1,
                    FractionOfRevenue = 0.3M
                }
            };
            var players = new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 10,
                [3] = 7,
                [4] = 10,
                [5] = 3,
                [6] = 7,
                [7] = 2,
                [8] = 9,
                [9] = 3,
                [10] = 2,
            };
            var service = new LotteryDrawService(this.randomNumberServiceMock.Object, model, players);

            var act = () =>
            {
                service.GetRewardViewModel(model => model.GrandPrize);
                service.GetRewardViewModel(model => model.SecondTier);
            };

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot draw 58 because the lottery has 57 remaining.");
        }

        [TestMethod]
        public void GetRewardViewModelWithMissingTierTest()
        {
            var model = new LotteryModel
            {
                TicketPrice = 1,
                GrandPrize = new LotteryModel.WinningsModel
                {
                    NumberOfWinningTickets = 1,
                    FractionOfRevenue = 0.5M
                },
                SecondTier = new LotteryModel.WinningsModel
                {
                    FractionOfWinningTickets = 0.1,
                    FractionOfRevenue = 0.3M
                }
            };
            var players = new Dictionary<int, int>
            {
                [1] = 5,
                [2] = 10,
                [3] = 7,
                [4] = 10,
                [5] = 3,
                [6] = 7,
                [7] = 2,
                [8] = 9,
                [9] = 3,
                [10] = 2,
            };
            var service = new LotteryDrawService(this.randomNumberServiceMock.Object, model, players);

            var act = () =>
            {
                service.GetRewardViewModel(model => model.GrandPrize);
                service.GetRewardViewModel(model => model.SecondTier);
                service.GetRewardViewModel(model => model.ThirdTier);
            };

            act.Should().NotThrow();
        }

        [TestMethod]
        public void GetRewardViewModelWithNoPlayersTest()
        {
            var model = new LotteryModel();
            var players = new Dictionary<int, int>();
            var service = new LotteryDrawService(this.randomNumberServiceMock.Object, model, players);

            var act = () => service.GetRewardViewModel(model => model.GrandPrize);

            act.Should().Throw<InvalidOperationException>()
                .WithMessage("Cannot get draw rewards with no players.");
        }
    }
}
