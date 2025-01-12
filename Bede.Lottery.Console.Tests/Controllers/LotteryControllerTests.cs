namespace Bede.Lottery.Controllers
{
    using Models;
    using Services;
    using ViewModels;
    using Views;

    [TestClass]
    public sealed class LotteryControllerTests
    {
        private readonly Mock<IViewProvider> viewProviderMock = new();
        private readonly Mock<ILotteryService> lotteryServiceMock = new();
        private readonly LotteryController controller;

        public LotteryControllerTests()
        {
            this.controller = new LotteryController(this.viewProviderMock.Object, this.lotteryServiceMock.Object);
        }

        [TestMethod]
        public void WelcomeTest()
        {
            var viewModel = new WelcomeViewModel();
            this.lotteryServiceMock.Setup(mock => mock.GetWelcomeViewModel()).Returns(viewModel);
            this.viewProviderMock.Setup(mock => mock.CreateView(viewModel, nameof(LotteryController.Welcome))).Returns(Mock.Of<IView>());

            var welcomeView = this.controller.Welcome();

            welcomeView.Should().BeAssignableTo<IView>();
        }

        [TestMethod]
        public void DrawTest()
        {
            var model = new DrawModel { NumberOfTickets = 5 };
            var viewModel = new DrawViewModel
            {
                Players = Array.Empty<DrawViewModel.PlayerViewModel>(),
                GrandPrize = null,
                SecondTier = null,
                ThirdTier = null
            };
            this.lotteryServiceMock.Setup(mock => mock.GetDrawViewModel(model)).Returns(viewModel);
            this.viewProviderMock.Setup(mock => mock.CreateView(viewModel, nameof(LotteryController.Draw))).Returns(Mock.Of<IView>());

            var welcomeView = this.controller.Draw(model);

            welcomeView.Should().BeAssignableTo<IView>();
        }

        [TestMethod]
        public void InputErrorTest()
        {
            this.viewProviderMock.Setup(mock => mock.CreateView(null, nameof(LotteryController.InputError))).Returns(Mock.Of<IView>());

            var welcomeView = this.controller.InputError();

            welcomeView.Should().BeAssignableTo<IView>();
        }
    }
}
