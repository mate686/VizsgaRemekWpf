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
            // DI konténer összerakása — ugyanaz a minta mint a feltöltött ViewModelFactory-ban
            var services = new ServiceCollection();

            // Services
            services.AddSingleton<ApiService>();
            services.AddSingleton<IViewModelFactory, ViewModelFactory>();

            // ViewModels — ugyanúgy regisztrálva mint a feltöltött kódban
            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<MainViewModel>();
            services.AddTransient<OverviewViewModel>();
            services.AddTransient<RestaurantsViewModel>();
            services.AddTransient<OrdersViewModel>();
            services.AddTransient<UsersViewModel>();
            services.AddTransient<ReviewsViewModel>();

            // View — konstruktor injektálással kapja a MainViewModel-t
            services.AddSingleton<MainView>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // MainView megnyitása — DataContext a konstruktorban van beállítva
            var mainView = _serviceProvider.GetRequiredService<MainView>();
            mainView.Show();
        }
    }

}
