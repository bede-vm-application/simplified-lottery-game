namespace Bede.Lottery.Utilities
{
    [TestClass]
    public sealed class CumulativeTicketSumTests
    {
        [TestMethod]
        public void GetWinningPlayerIndexExactTest()
        {
            var cumulativeTicketSum = new CumulativeTicketSum([1, 4, 0, 0, 5, 5, 5, 10, 15, 20]);

            int winningPlayerIndex = cumulativeTicketSum.GetWinningPlayerIndex(10);

            winningPlayerIndex.Should().Be(4);
        }

        [TestMethod]
        public void GetWinningPlayerIndexComplementTest()
        {
            var cumulativeTicketSum = new CumulativeTicketSum([1, 4, 0, 0, 5, 5, 5, 10, 15, 20]);

            int winningPlayerIndex = cumulativeTicketSum.GetWinningPlayerIndex(18);

            winningPlayerIndex.Should().Be(6);
        }

        [TestMethod]
        public void GetWinningPlayerIndexBackShiftTest()
        {
            var cumulativeTicketSum = new CumulativeTicketSum([1, 0, 0, 4, 0, 0, 5, 5, 5, 10, 15, 20]);

            int winningPlayerIndex = cumulativeTicketSum.GetWinningPlayerIndex(1);

            winningPlayerIndex.Should().Be(0);
        }

        [TestMethod]
        public void DecrementTicketsTest()
        {
            var cumulativeTicketSum = new CumulativeTicketSum([1, 0, 0, 4, 0, 0, 5, 5, 5, 10, 15, 20]);

            cumulativeTicketSum.DecrementTickets(6);

            using var _ = new AssertionScope();
            cumulativeTicketSum.RemainingTickets.Should().Be(64);
            cumulativeTicketSum.GetWinningPlayerIndex(9).Should().Be(6);
        }
    }
}
