namespace Bede.Lottery.Views
{
    public abstract class BaseViewTests : IDisposable
    {
        protected Mock<IStringLocalizer> LocalizerMock { get; } = new();
        protected MemoryStream Stream { get; } = new();

        protected BaseViewTests()
        {
            this.LocalizerMock
                .Setup(mock => mock[It.IsAny<string>()])
                .Returns((string key) => new LocalizedString(key, key, resourceNotFound: true));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.Stream.Dispose();
            }
        }

        protected string GetRenderedViewFromStream()
        {
            return Encoding.Default.GetString(this.Stream.ToArray());
        }
    }
}
