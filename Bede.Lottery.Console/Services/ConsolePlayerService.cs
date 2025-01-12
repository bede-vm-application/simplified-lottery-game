namespace Bede.Lottery.Services
{
    using Models;

    internal sealed class ConsolePlayerService(IConsoleInputService consoleInputService) : IPlayerService
    {
        public async Task<DrawModel> GetPlayerInputAsync()
        {
            var input = await Task.Run(consoleInputService.ReadLine).ConfigureAwait(true);
            int numberOfTickets = Convert.ToInt32(input, CultureInfo.InvariantCulture);
            if (numberOfTickets <= 0)
            {
                throw new FormatException("Value cannot be less than or equal to 0.");
            }

            var model = new DrawModel { NumberOfTickets = numberOfTickets };
            return model;
        }
    }
}
