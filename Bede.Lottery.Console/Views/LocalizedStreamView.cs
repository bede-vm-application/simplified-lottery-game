namespace Bede.Lottery.Views
{
    internal abstract class LocalizedStreamView(IStringLocalizer localizer, Stream stream) : IView
    {
        protected readonly IStringLocalizer localizer = localizer;

        public void Render()
        {
            using var writer = new StreamWriter(stream, leaveOpen: true);
            this.Render(writer);
        }

        protected abstract void Render(StreamWriter writer);
    }
}
