using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using VizsgaRemekWpf.Services;
using VizsgaRemekWpf.ViewModels;
using VizsgaRemekWpf.Views;

namespace VizsgaRemekWpf
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public App()
        {
            
            var services = new ServiceCollection();

            
            services.AddSingleton<ApiService>();
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddTransient<OverviewViewModel>();
            services.AddTransient<RestaurantsViewModel>();
            services.AddTransient<OrdersViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<ReviewsViewModel>();

 
            services.AddSingleton<MainView>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

     
            var mainView = _serviceProvider.GetRequiredService<MainView>();
            mainView.Show();
        }
    }

}
