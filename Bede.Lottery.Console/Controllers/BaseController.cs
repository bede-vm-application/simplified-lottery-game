namespace Bede.Lottery.Controllers
{
    using System.Runtime.CompilerServices;

    using Views;

    internal abstract class BaseController(IViewProvider viewProvider)
    {
        private readonly IViewProvider viewProvider = viewProvider;

        protected IView View([CallerMemberName] string viewName = "")
        {
            return this.viewProvider.CreateView(null, viewName);
        }

        protected IView View<TViewModel>(TViewModel model, [CallerMemberName] string viewName = "") where TViewModel : notnull
        {
            return this.viewProvider.CreateView(model, viewName);
        }
    }
}
