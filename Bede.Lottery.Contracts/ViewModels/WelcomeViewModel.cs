namespace Bede.Lottery.ViewModels
{
    using Models;

    public record WelcomeViewModel : IBalanceModel, ITicketPriceModel
    {
        public decimal Balance { get; init; }
        public decimal TicketPrice { get; init; }
    }
}
