namespace Bede.Lottery.Views
{
    internal sealed class InputErrorView(IStringLocalizer localizer, Stream stream)
        : LocalizedStreamView(localizer, stream), IView
    {
        protected override void Render(StreamWriter writer)
        {
            writer.WriteLine();
            writer.WriteLine(this.localizer["InputErrorText"]);
        }
    }
}
