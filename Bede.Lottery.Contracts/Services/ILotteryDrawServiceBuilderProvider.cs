namespace Bede.Lottery.Services
{
    using Models;

    public interface ILotteryDrawServiceBuilderProvider
    {
        ILotteryDrawServiceBuilder CreateLotteryDrawServiceBuilder(LotteryModel model);
    }
}
