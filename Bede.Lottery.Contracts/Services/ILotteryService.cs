namespace Bede.Lottery.Services
{
    using Models;
    using ViewModels;

    public interface ILotteryService
    {
        WelcomeViewModel GetWelcomeViewModel();
        DrawViewModel GetDrawViewModel(DrawModel drawModel);
    }
}
