namespace Bede.Lottery.Services
{
    using Controllers;
    using Models;
    using Views;

    [TestClass]
    public sealed class LotteryApplicationServiceTests
    {
        private readonly Mock<IHostApplicationLifetime> hostApplicationLifetimeMock = new();
        private readonly Mock<ILogger<LotteryApplicationService>> loggerMock = new();
        private readonly Mock<ILotteryController> controllerMock = new();
        private readonly Mock<IPlayerService> playerServiceMock = new();
        private readonly LotteryApplicationService service;

        public LotteryApplicationServiceTests()
        {
            this.service = new LotteryApplicationService(
                this.hostApplicationLifetimeMock.Object,
                this.loggerMock.Object,
                this.controllerMock.Object,
                this.playerServiceMock.Object
            );
        }

        [TestMethod]
        public async Task StartAsyncTest()
        {
            this.controllerMock.Setup(mock => mock.Welcome()).Returns(Mock.Of<IView>());
            this.controllerMock.Setup(mock => mock.Draw(It.IsAny<DrawModel>())).Returns(Mock.Of<IView>());

            await this.service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            var timeout = TimeSpan.FromSeconds(10);
            var stopApplicationTask = () => this.hostApplicationLifetimeMock
                .VerifyAsync(mock => mock.StopApplication(), Times.Once(), timeout);

            await stopApplicationTask.Should().NotThrowAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task StartAsyncWithInputErrorTest()
        {
            this.controllerMock.Setup(mock => mock.Welcome()).Returns(Mock.Of<IView>());
            this.playerServiceMock.SetupSequence(mock => mock.GetPlayerInputAsync())
                .ThrowsAsync(new FormatException())
                .Returns(Task.FromResult(Mock.Of<DrawModel>()));
            this.controllerMock.Setup(mock => mock.InputError()).Returns(Mock.Of<IView>());
            this.controllerMock.Setup(mock => mock.Draw(It.IsAny<DrawModel>())).Returns(Mock.Of<IView>());

            await this.service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            var timeout = TimeSpan.FromSeconds(10);
            var stopApplicationTask = () => this.hostApplicationLifetimeMock
                .VerifyAsync(mock => mock.StopApplication(), Times.Once(), timeout);

            await stopApplicationTask.Should().NotThrowAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task StartAsyncWithApplicationErrorTest()
        {
            this.controllerMock.Setup(mock => mock.Welcome()).Returns(Mock.Of<IView>());
            this.controllerMock.Setup(mock => mock.Draw(It.IsAny<DrawModel>())).Throws(new InvalidOperationException());

            await this.service.StartAsync(CancellationToken.None).ConfigureAwait(false);

            var timeout = TimeSpan.FromSeconds(10);
            var stopApplicationTask = () => this.hostApplicationLifetimeMock
                .VerifyAsync(mock => mock.StopApplication(), Times.Once(), timeout);

            await stopApplicationTask.Should().NotThrowAsync().ConfigureAwait(false);
        }

        [TestMethod]
        public void StopAsync()
        {
            var stopTask = this.service.StopAsync(CancellationToken.None);
            stopTask.Should().Be(Task.CompletedTask);
        }
    }
}
