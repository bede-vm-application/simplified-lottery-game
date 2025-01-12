namespace Bede.Lottery.Controllers
{
    using Models;
    using Services;
    using Views;

    internal sealed class LotteryController(IViewProvider viewProvider, ILotteryService lotteryService)
        : BaseController(viewProvider), ILotteryController
    {
        private readonly ILotteryService lotteryService = lotteryService;

        public IView Welcome()
        {
            var model = this.lotteryService.GetWelcomeViewModel();
            return this.View(model);
        }

        public IView Draw(DrawModel drawModel)
        {
            var model = this.lotteryService.GetDrawViewModel(drawModel);
            return this.View(model);
        }

        public IView InputError()
        {
            return this.View();
        }
    }
}
