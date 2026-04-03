using Microsoft.Extensions.DependencyInjection;


namespace VizsgaRemekWpf.Services
{
    public class ViewModelFactory : IViewModelFactory
    {
        private readonly IServiceProvider _provider;
        public ViewModelFactory(IServiceProvider provider) { _provider = provider; }
        public T Create<T>() where T : class => _provider.GetRequiredService<T>();
    }
}
