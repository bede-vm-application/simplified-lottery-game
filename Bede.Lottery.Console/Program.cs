namespace Bede.Lottery
{
    using Controllers;
    using Services;
    using Views;

    [ExcludeFromCodeCoverage(
        Justification = "Top-level has concrete framework dependencies that are difficult to work around.")]
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Default;

            var applicationBuilder = Host.CreateApplicationBuilder(args);
            applicationBuilder.Services.AddLocalization()
                .AddHostedService<LotteryApplicationService>()
                .AddSingleton<IConsoleInputService, ConsoleInputService>()
                .AddSingleton<IViewProvider, ConsoleViewProvider>()
                .AddSingleton<ILotteryDrawServiceBuilderProvider, LotteryDrawServiceBuilderProvider>()
                .AddSingleton<IPlayerService, ConsolePlayerService>()
                .AddTransient<IRandomNumberService, RandomNumberService>()
                .AddTransient<ILotteryService, ConfiguredLotteryService>()
                .AddTransient<ILotteryController, LotteryController>();

            using var consoleApp = applicationBuilder.Build();
            consoleApp.Run();
        }
    }
}
