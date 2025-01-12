namespace Bede.Lottery.Controllers
{
    using Models;
    using Views;

    public interface ILotteryController
    {
        IView Welcome();
        IView Draw(DrawModel model);
        IView InputError();
    }
}
