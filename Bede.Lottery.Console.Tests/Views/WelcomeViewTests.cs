namespace Bede.Lottery.Views
{
    using ViewModels;

    [TestClass]
    public sealed class WelcomeViewTests : BaseViewTests
    {
        [TestMethod]
        public void RenderTest()
        {
            var model = new WelcomeViewModel
            {
                Balance = 10,
                TicketPrice = 1
            };

            var view = new WelcomeView(
                this.LocalizerMock.Object,
                this.Stream,
                model);

            view.Render();
            string renderedView = this.GetRenderedViewFromStream();

            using var _ = new AssertionScope();
            this.Stream.CanWrite.Should().BeTrue();
            renderedView.Should().Be($@"WelcomeText

* BalanceLabel: {model.Balance:C}
* TicketPriceLabel: {model.TicketPrice:C}

NumberOfTicketsLabel
");
        }
    }
}
