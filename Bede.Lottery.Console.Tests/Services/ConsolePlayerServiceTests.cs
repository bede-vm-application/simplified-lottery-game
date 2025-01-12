namespace Bede.Lottery.Services
{
    using Models;

    [TestClass]
    public sealed class ConsolePlayerServiceTests
    {
        private readonly Mock<IConsoleInputService> consoleInputServiceMock = new();
        private readonly ConsolePlayerService service;

        public ConsolePlayerServiceTests()
        {
            this.service = new ConsolePlayerService(this.consoleInputServiceMock.Object);
        }

        [TestMethod]
        public async Task GetPlayerInputAsyncTest()
        {
            const int numberOfTickets = 5;
            this.consoleInputServiceMock.Setup(mock => mock.ReadLine()).Returns($"{numberOfTickets}");

            var model = await this.service.GetPlayerInputAsync().ConfigureAwait(false);

            model.Should().BeEquivalentTo(new DrawModel { NumberOfTickets = numberOfTickets });
        }

        [TestMethod]
        public async Task GetPlayerInputAsyncWithNegativeValueTest()
        {
            const int negativeNumberOfTickets = -1;
            this.consoleInputServiceMock.Setup(mock => mock.ReadLine()).Returns($"{negativeNumberOfTickets}");

            var act = this.service.GetPlayerInputAsync;

            await act.Should().ThrowAsync<FormatException>().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPlayerInputAsyncWithZeroValueTest()
        {
            const int zeroNumberOfTickets = 0;
            this.consoleInputServiceMock.Setup(mock => mock.ReadLine()).Returns($"{zeroNumberOfTickets}");

            var act = this.service.GetPlayerInputAsync;

            await act.Should().ThrowAsync<FormatException>().ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetPlayerInputAsyncWithTextTest()
        {
            this.consoleInputServiceMock.Setup(mock => mock.ReadLine()).Returns("text");

            var act = this.service.GetPlayerInputAsync;

            await act.Should().ThrowAsync<FormatException>().ConfigureAwait(false);
        }
    }
}
