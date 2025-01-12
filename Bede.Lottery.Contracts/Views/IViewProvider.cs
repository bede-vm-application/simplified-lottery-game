namespace Bede.Lottery.Views
{
    public interface IViewProvider
    {
        IView CreateView(object? model, string viewName);
    }
}
