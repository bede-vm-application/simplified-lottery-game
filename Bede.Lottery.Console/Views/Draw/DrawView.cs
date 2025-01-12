namespace Bede.Lottery.Views
{
    using ViewModels;

    internal sealed class DrawView(IStringLocalizer localizer, Stream stream, DrawViewModel model)
        : LocalizedStreamView(localizer, stream), IView
    {
        private readonly DrawViewModel model = model;

        protected override void Render(StreamWriter writer)
        {
            if (this.model.BalanceExceeded)
            {
                writer.WriteLine();
                writer.WriteLine(this.localizer["BalanceExceededText"]);
            }

            writer.WriteLine();
            writer.WriteLine($"{this.localizer["PlayersTitle"]}:");
            this.WritePlayers(writer);
            writer.WriteLine();
            writer.WriteLine($"{this.localizer["ResultsTitle"]}:");
            this.WriteRewards(writer, "GrandPrizeLabel", this.model.GrandPrize);
            this.WriteRewards(writer, "SecondTierLabel", this.model.SecondTier);
            this.WriteRewards(writer, "ThirdTierLabel", this.model.ThirdTier);
            writer.WriteLine();
            writer.WriteLine(this.localizer["CongratulationsText"]);
            writer.WriteLine();
            writer.WriteLine($"{this.localizer["HouseRevenueLabel"]}: {this.model.HouseRevenue:C}");
        }

        private void WritePlayers(StreamWriter writer)
        {
            foreach (var player in this.model.Players)
            {
                string ticketsLabel = player.NumberOfTickets > 1 ? "TicketsText" : "TicketText";
                writer.Write($"{this.localizer["PlayerLabel"]} {player.PlayerNumber} ");
                writer.WriteLine($"{this.localizer["BoughtText"]} {player.NumberOfTickets} {this.localizer[ticketsLabel]}.");
            }
        }

        private void WriteRewards(
            StreamWriter writer,
            string rewardLabel,
            DrawViewModel.RewardViewModel? reward)
        {
            if (reward is null)
            {
                return;
            }

            bool multipleWinners = reward.PlayerNumbers.Count > 1;
            string playerLabel = multipleWinners ? "PlayersLabel" : "PlayerLabel";
            string winText = multipleWinners ? "WinText" : "WinsText";

            writer.Write($"* {this.localizer[rewardLabel]}:");
            writer.Write($" {this.localizer[playerLabel]} ");
            writer.Write(string.Join(", ", reward.PlayerNumbers));
            writer.Write($" {this.localizer[winText]}");
            writer.Write($" {reward.Amount:C}");
            writer.WriteLine("!");
        }
    }
}
