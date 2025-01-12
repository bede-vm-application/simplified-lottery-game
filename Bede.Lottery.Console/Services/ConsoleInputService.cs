namespace Bede.Lottery.Services
{
    internal sealed class ConsoleInputService : IConsoleInputService
    {
        [ExcludeFromCodeCoverage(Justification = "Console.ReadLine cannot be unit tested")]
        public string? ReadLine() => Console.ReadLine();
    }
}
