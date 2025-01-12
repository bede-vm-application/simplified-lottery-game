namespace Bede.Lottery.Views
{
    [TestClass]
    public sealed class InputErrorViewTests : BaseViewTests
    {
        [TestMethod]
        public void RenderTest()
        {
            var view = new InputErrorView(this.LocalizerMock.Object, this.Stream);

            view.Render();
            string renderedView = this.GetRenderedViewFromStream();

            using var _ = new AssertionScope();
            this.Stream.CanWrite.Should().BeTrue();
            renderedView.Should().Be(@"
InputErrorText
");
        }
    }
}
