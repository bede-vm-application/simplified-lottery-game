namespace Bede.Lottery.Views
{
    using ViewModels;

    [TestClass]
    public sealed class DrawViewTests : BaseViewTests
    {
        [TestMethod]
        public void RenderTest()
        {
            var model = new DrawViewModel
            {
                BalanceExceeded = false,
                Players = [
                    new DrawViewModel.PlayerViewModel {PlayerNumber = 1, NumberOfTickets = 10},
                    new DrawViewModel.PlayerViewModel {PlayerNumber = 2, NumberOfTickets = 5},
                    new DrawViewModel.PlayerViewModel {PlayerNumber = 3, NumberOfTickets = 7},
                    new DrawViewModel.PlayerViewModel {PlayerNumber = 4, NumberOfTickets = 3},
                    new DrawViewModel.PlayerViewModel {PlayerNumber = 5, NumberOfTickets = 2},
                ],
                GrandPrize = new DrawViewModel.RewardViewModel
                {
                    PlayerNumbers = [1],
                    Amount = 35M
                },
                SecondTier = new DrawViewModel.RewardViewModel
                {
                    PlayerNumbers = [2, 3, 4],
                    Amount = 3M
                },
                ThirdTier = new DrawViewModel.RewardViewModel
                {
                    PlayerNumbers = [1, 3, 4, 5],
                    Amount = 1M
                },
                HouseRevenue = 15M
            };

            var view = new DrawView(this.LocalizerMock.Object, this.Stream, model);

            view.Render();
            string renderedView = this.GetRenderedViewFromStream();

            using var _ = new AssertionScope();
            this.Stream.CanWrite.Should().BeTrue();
            renderedView.Should().Be($@"
PlayersTitle:
PlayerLabel 1 BoughtText 10 TicketsText.
PlayerLabel 2 BoughtText 5 TicketsText.
PlayerLabel 3 BoughtText 7 TicketsText.
PlayerLabel 4 BoughtText 3 TicketsText.
PlayerLabel 5 BoughtText 2 TicketsText.

ResultsTitle:
* GrandPrizeLabel: PlayerLabel 1 WinsText {35:C}!
* SecondTierLabel: PlayersLabel 2, 3, 4 WinText {3:C}!
* ThirdTierLabel: PlayersLabel 1, 3, 4, 5 WinText {1:C}!

CongratulationsText

HouseRevenueLabel: {15:C}
");
        }

        [TestMethod]
        public void RenderWithBalanceExceededTest()
        {
            var model = new DrawViewModel
            {
                BalanceExceeded = true,
                Players = [
                    new DrawViewModel.PlayerViewModel {PlayerNumber = 1, NumberOfTickets = 10},
                ],
                GrandPrize = new DrawViewModel.RewardViewModel
                {
                    PlayerNumbers = [1],
                    Amount = 35M
                },
                SecondTier = null,
                ThirdTier = null,
                HouseRevenue = 15M
            };

            var view = new DrawView(this.LocalizerMock.Object, this.Stream, model);

            view.Render();
            string renderedView = this.GetRenderedViewFromStream();

            using var _ = new AssertionScope();
            this.Stream.CanWrite.Should().BeTrue();
            renderedView.Should().Be($@"
BalanceExceededText

PlayersTitle:
PlayerLabel 1 BoughtText 10 TicketsText.

ResultsTitle:
* GrandPrizeLabel: PlayerLabel 1 WinsText {35:C}!

CongratulationsText

HouseRevenueLabel: {15:C}
");
        }
    }
}
