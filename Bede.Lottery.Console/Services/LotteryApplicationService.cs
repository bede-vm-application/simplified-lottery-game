namespace Bede.Lottery.Services
{
    using Controllers;
    using Models;

    internal sealed partial class LotteryApplicationService(
        IHostApplicationLifetime hostApplicationLifetime,
        ILogger<LotteryApplicationService> logger,
        ILotteryController lotteryController,
        IPlayerService playerService)
        : IHostedService
    {
        private readonly IHostApplicationLifetime hostApplicationLifetime = hostApplicationLifetime;
        private readonly ILogger<LotteryApplicationService> logger = logger;
        private readonly ILotteryController lotteryController = lotteryController;
        private readonly IPlayerService playerService = playerService;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(this.TryPlayLotteryAsync, cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "Top level application")]
        private async Task TryPlayLotteryAsync()
        {
            try
            {
                await this.PlayLotteryAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                ApplicationError(this.logger, exception);
            }
            finally
            {
                this.hostApplicationLifetime.StopApplication();
            }
        }

        private async Task PlayLotteryAsync()
        {
            var welcomeView = this.lotteryController.Welcome();
            welcomeView.Render();

            var drawModel = await this.GetPlayerInputAsync().ConfigureAwait(false);
            var drawView = this.lotteryController.Draw(drawModel);
            drawView.Render();
        }

        [SuppressMessage(
            "Design",
            "CA1031:Do not catch general exception types",
            Justification = "The exception type may vary depending on the injected implementation")]
        private async Task<DrawModel> GetPlayerInputAsync()
        {
            try
            {
                var drawModel = await this.playerService.GetPlayerInputAsync().ConfigureAwait(false);
                return drawModel;
            }
            catch (Exception exception)
            {
                LogInputError(this.logger, exception);

                var inputErrorView = this.lotteryController.InputError();
                inputErrorView.Render();

                return await this.GetPlayerInputAsync().ConfigureAwait(false);
            }
        }

        [LoggerMessage(Level = LogLevel.Critical, Message = "An unhandled error occurred.")]
        static partial void ApplicationError(ILogger<LotteryApplicationService> logger, Exception exception);

        [LoggerMessage(Level = LogLevel.Error, Message = "Could not get user input.")]
        static partial void LogInputError(ILogger<LotteryApplicationService> logger, Exception exception);
    }
}
