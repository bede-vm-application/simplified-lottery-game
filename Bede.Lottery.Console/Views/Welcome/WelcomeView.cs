namespace Bede.Lottery.Views
{
    using ViewModels;

    internal sealed class WelcomeView(IStringLocalizer localizer, Stream stream, WelcomeViewModel model)
        : LocalizedStreamView(localizer, stream), IView
    {
        private readonly WelcomeViewModel model = model;

        protected override void Render(StreamWriter writer)
        {
            writer.WriteLine(this.localizer["WelcomeText"]);
            writer.WriteLine();
            writer.WriteLine($"* {this.localizer["BalanceLabel"]}: {this.model.Balance:C}");
            writer.WriteLine($"* {this.localizer["TicketPriceLabel"]}: {this.model.TicketPrice:C}");
            writer.WriteLine();
            writer.WriteLine(this.localizer["NumberOfTicketsLabel"]);
        }
    }
}
