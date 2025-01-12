namespace Bede.Lottery.Services
{
    using Models;

    internal sealed class LotteryDrawServiceBuilderProvider(
        ILogger<LotteryDrawServiceBuilder> logger,
        IRandomNumberService randomNumberService)
        : ILotteryDrawServiceBuilderProvider
    {
        private readonly ILogger<LotteryDrawServiceBuilder> logger = logger;
        private readonly IRandomNumberService randomNumberService = randomNumberService;

        public ILotteryDrawServiceBuilder CreateLotteryDrawServiceBuilder(LotteryModel model)
        {
            return new LotteryDrawServiceBuilder(this.logger, this.randomNumberService, model);
        }
    }
}
