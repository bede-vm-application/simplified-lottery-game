namespace Bede.Lottery.Services
{
    using Models;

    public interface ILotteryDrawServiceBuilder
    {
        ILotteryDrawServiceBuilder AddCpuPlayers();
        ILotteryDrawServiceBuilder AddPlayer(int numberOfTickets);
        ILotteryDrawService Build();
    }
}
