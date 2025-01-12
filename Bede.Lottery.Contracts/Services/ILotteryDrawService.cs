namespace Bede.Lottery.Services
{
    using Models;
    using ViewModels;

    public interface ILotteryDrawService
    {
        decimal HouseRevenue { get; }

        ICollection<DrawViewModel.PlayerViewModel> GetPlayersViewModel();
        DrawViewModel.RewardViewModel? GetRewardViewModel(Func<LotteryModel, LotteryModel.WinningsModel?> winningsModel);
    }
}
