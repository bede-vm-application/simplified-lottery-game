namespace Bede.Lottery.Services
{
    using Models;

    [TestClass]
    public sealed class LotteryDrawServiceBuilderTests
    {
        private readonly Mock<ILogger<LotteryDrawServiceBuilder>> loggerMock = new();
        private readonly Mock<IRandomNumberService> randomNumberServiceMock = new();

        [TestMethod]
        public void BuildTest()
        {
            var serviceBuilder = this.CreateLotteryDrawServiceBuilder();
            serviceBuilder.AddPlayer(5);
            serviceBuilder.AddCpuPlayers();

            var service = serviceBuilder.Build();

            service.Should().BeOfType<LotteryDrawService>();
        }

        [TestMethod]
        public void BuildWithInvalidCpuPlayersTest()
        {
            var serviceBuilder = this.CreateLotteryDrawServiceBuilder();
            serviceBuilder.AddPlayer(5);
            serviceBuilder.AddCpuPlayers();

            var act = serviceBuilder.AddCpuPlayers;

            act.Should().Throw<InvalidOperationException>();
        }

        [TestMethod]
        public void BuildWithExceededPlayersTest()
        {
            var serviceBuilder = this.CreateLotteryDrawServiceBuilder();
            serviceBuilder.AddPlayer(5);
            serviceBuilder.AddCpuPlayers();

            var act = () => serviceBuilder.AddPlayer(10);

            act.Should().Throw<InvalidOperationException>();
        }

        private LotteryDrawServiceBuilder CreateLotteryDrawServiceBuilder()
        {
            var model = new LotteryModel
            {
                Balance = 10M,
                TicketPrice = 1M,
                MinPlayerCount = 10,
                MaxPlayerCount = 15,
            };

            this.randomNumberServiceMock.Setup(mock => mock.Next(model.MinPlayerCount - 1, model.MaxPlayerCount - 1)).Returns(model.MaxPlayerCount - 1);

            var serviceBuilder = new LotteryDrawServiceBuilder(this.loggerMock.Object, this.randomNumberServiceMock.Object, model);
            return serviceBuilder;
        }
    }
}
