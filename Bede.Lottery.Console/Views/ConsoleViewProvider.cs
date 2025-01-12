namespace Bede.Lottery.Views
{
    using ViewModels;

    internal sealed class ConsoleViewProvider(IStringLocalizerFactory localizerFactory) : IViewProvider
    {
        private readonly Lazy<Stream> stream = new(Console.OpenStandardOutput);

        public IView CreateView(object? model, string viewName) => viewName switch
        {
            "Welcome" => this.CreateWelcomeView(model),
            "Draw" => this.CreateDrawView(model),
            "InputError" => this.CreateErrorView(),
            _ => throw new ArgumentException($"Invalid view for controller action {viewName}.", nameof(viewName))
        };

        private WelcomeView CreateWelcomeView(object? model)
        {
            if (model is WelcomeViewModel welcomeModel)
            {
                var localizer = localizerFactory.Create(typeof(WelcomeView));
                return new WelcomeView(localizer, this.stream.Value, welcomeModel);
            }
            else
            {
                throw new ArgumentNullException(nameof(model));
            }

            throw new ArgumentException(
                $"Invalid model for '{nameof(WelcomeView)}' of type '{model.GetType()}'.", nameof(model));
        }

        private DrawView CreateDrawView(object? model)
        {
            if (model is DrawViewModel drawModel)
            {
                var localizer = localizerFactory.Create(typeof(DrawView));
                return new DrawView(localizer, this.stream.Value, drawModel);
            }
            else
            {
                throw new ArgumentNullException(nameof(model));
            }

            throw new ArgumentException(
                $"Invalid model for '{nameof(DrawView)}' of type '{model.GetType()}'.", nameof(model));
        }

        private InputErrorView CreateErrorView()
        {
            var localizer = localizerFactory.Create(typeof(InputErrorView));
            return new InputErrorView(localizer, this.stream.Value);
        }
    }
}
