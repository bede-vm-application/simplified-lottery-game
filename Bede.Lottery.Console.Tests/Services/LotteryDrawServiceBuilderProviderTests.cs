namespace Bede.Lottery.Services
{
    [TestClass]
    public sealed class LotteryDrawServiceBuilderProviderTests
    {
        private readonly Mock<ILogger<LotteryDrawServiceBuilder>> loggerMock = new();
        private readonly Mock<IRandomNumberService> randomNumberServiceMock = new();
        private readonly LotteryDrawServiceBuilderProvider serviceProvider;

        public LotteryDrawServiceBuilderProviderTests()
        {
            this.serviceProvider = new LotteryDrawServiceBuilderProvider(this.loggerMock.Object, this.randomNumberServiceMock.Object);
        }

        [TestMethod]
        public void CreateLotteryDrawServiceBuilderTest()
        {
            var serviceBuilder = this.serviceProvider.CreateLotteryDrawServiceBuilder(new Models.LotteryModel());
            serviceBuilder.Should().BeOfType<LotteryDrawServiceBuilder>();
        }
    }
}
