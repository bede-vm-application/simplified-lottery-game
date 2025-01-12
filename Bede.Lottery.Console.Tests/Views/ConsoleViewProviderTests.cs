namespace Bede.Lottery.Views
{
    using ViewModels;

    [TestClass]
    public sealed class ConsoleViewProviderTests
    {
        private readonly Mock<IStringLocalizerFactory> localizerFactoryMock = new();
        private readonly ConsoleViewProvider provider;

        public ConsoleViewProviderTests()
        {
            // this.localizerFactoryMock.Setup(factory => factory.Create(It.IsAny<Type>()));
            this.provider = new ConsoleViewProvider(this.localizerFactoryMock.Object);
        }

        [TestMethod]
        public void CreateWelcomeViewTest()
        {
            var model = new WelcomeViewModel();

            var view = this.provider.CreateView(model, "Welcome");

            using var _ = new AssertionScope();
            view.Should().BeOfType<WelcomeView>();
            this.CreateLocalizerCreateVerifyAction<WelcomeView>(Times.Once).Should().NotThrow();
        }

        [TestMethod]
        public void CreateWelcomeViewWithNullModelTest()
        {
            var act = () => this.provider.CreateView(null, "Welcome");

            using var _ = new AssertionScope();
            act.Should().Throw<ArgumentException>();
            this.CreateLocalizerCreateVerifyAction<WelcomeView>(Times.Never).Should().NotThrow();
        }

        [TestMethod]
        public void CreateWelcomeViewWithInvalidModelTest()
        {
            var act = () => this.provider.CreateView(new object(), "Welcome");

            using var _ = new AssertionScope();
            act.Should().Throw<ArgumentException>();
            this.CreateLocalizerCreateVerifyAction<WelcomeView>(Times.Never).Should().NotThrow();
        }

        [TestMethod]
        public void CreateDrawViewTest()
        {
            var model = new DrawViewModel()
            {
                Players = Array.Empty<DrawViewModel.PlayerViewModel>(),
                GrandPrize = null,
                SecondTier = null,
                ThirdTier = null
            };

            var view = this.provider.CreateView(model, "Draw");

            using var _ = new AssertionScope();
            view.Should().BeOfType<DrawView>();
            this.CreateLocalizerCreateVerifyAction<DrawView>(Times.Once).Should().NotThrow();
        }

        [TestMethod]
        public void CreateDrawViewWithNullModelTest()
        {
            var act = () => this.provider.CreateView(null, "Draw");

            using var _ = new AssertionScope();
            act.Should().Throw<ArgumentException>();
            this.CreateLocalizerCreateVerifyAction<DrawView>(Times.Never).Should().NotThrow();
        }

        [TestMethod]
        public void CreateDrawViewWithInvalidModelTest()
        {
            var act = () => this.provider.CreateView(new object(), "Draw");

            using var _ = new AssertionScope();
            act.Should().Throw<ArgumentException>();
            this.CreateLocalizerCreateVerifyAction<DrawView>(Times.Never).Should().NotThrow();
        }

        [TestMethod]
        public void CreateInputErrorViewTest()
        {
            var view = this.provider.CreateView(null, "InputError");

            using var _ = new AssertionScope();
            view.Should().BeOfType<InputErrorView>();
            this.CreateLocalizerCreateVerifyAction<InputErrorView>(Times.Once).Should().NotThrow();
        }

        [TestMethod]
        public void CreateUnsupportedViewTest()
        {
            var act = () => this.provider.CreateView(null, "UnsupportedViewName");

            act.Should().Throw<ArgumentException>();
        }

        private Action CreateLocalizerCreateVerifyAction<T>(Func<Times> times) =>
            () => this.localizerFactoryMock.Verify(mock => mock.Create(typeof(T)), times);
    }
}
