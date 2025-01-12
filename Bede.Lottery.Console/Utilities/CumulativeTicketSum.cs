namespace Bede.Lottery.Utilities
{
    internal sealed class CumulativeTicketSum(ICollection<int> allNumberOfTickets)
    {
        private readonly int[] cumulativeTickets = GetCumulativeTickets(allNumberOfTickets);

        public int RemainingTickets => this.cumulativeTickets.Length > 0 ? this.cumulativeTickets[^1] : 0;

        public int GetWinningPlayerIndex(int winningTicket)
        {
            int winningPlayerIndex = Array.BinarySearch(this.cumulativeTickets, winningTicket);
            if (winningPlayerIndex < 0)
            {
                // BinarySearch returns the bitwise complement of the index of the first element that is larger than the ticket value
                winningPlayerIndex = ~winningPlayerIndex;
            }
            else
            {
                // If exact match is found make sure to pick the first player index as all following might already be spent
                for (
                    int previousIndex = winningPlayerIndex - 1;
                    previousIndex >= 0 && this.cumulativeTickets[previousIndex] == this.cumulativeTickets[winningPlayerIndex];
                    previousIndex--)
                {
                    winningPlayerIndex = previousIndex;
                }
            }

            return winningPlayerIndex;
        }

        public void DecrementTickets(int startIndex)
        {
            for (int index = startIndex; index < this.cumulativeTickets.Length; index++)
            {
                this.cumulativeTickets[index]--;
            }
        }

        private static int[] GetCumulativeTickets(ICollection<int> allNumberOfTickets)
        {
            int sumOfTickets = 0;
            var cumulativeTickets = new int[allNumberOfTickets.Count];
            int index = 0;

            foreach (int numberOfTickets in allNumberOfTickets)
            {
                sumOfTickets += numberOfTickets;
                cumulativeTickets[index] = sumOfTickets;
                index++;
            }

            return cumulativeTickets;
        }
    }
}
