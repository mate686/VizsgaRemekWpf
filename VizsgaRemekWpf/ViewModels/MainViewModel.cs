using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using VizsgaRemekWpf.Services;

namespace VizsgaRemekWpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly ApiService _api;
        private readonly IViewModelFactory _factory;

        public LoginViewModel LoginVM { get; }

        private bool _isLoggedIn;
        public bool IsLoggedIn { get => _isLoggedIn; set => Set(ref _isLoggedIn, value); }

        private string _adminUsername = "";
        public string AdminUsername { get => _adminUsername; set => Set(ref _adminUsername, value); }

        private string _lastUpdated = "";
        public string LastUpdated { get => _lastUpdated; set => Set(ref _lastUpdated, value); }

        private bool _isRefreshing;
        public bool IsRefreshing { get => _isRefreshing; set => Set(ref _isRefreshing, value); }

    
        public ObservableCollection<object> Tabs { get; }

        private object? _selectedTab;
        public object? SelectedTab { get => _selectedTab; set => Set(ref _selectedTab, value); }

        private readonly OverviewViewModel _overviewVm;
        private readonly RestaurantsViewModel _restaurantsVm;
        private readonly OrdersViewModel _ordersVm;
        private readonly UsersViewModel _usersVm;
        private readonly ReviewsViewModel _reviewsVm;

        public ICommand LogoutCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand SelectTabCommand { get; }

        public MainViewModel(ApiService api, IViewModelFactory factory, LoginViewModel loginVm)
        {
            _api = api;
            _factory = factory;
            LoginVM = loginVm;
            LoginVM.LoginSuccess += OnLoginSuccess;


            _overviewVm = _factory.Create<OverviewViewModel>();
            _restaurantsVm = _factory.Create<RestaurantsViewModel>();
            _ordersVm = _factory.Create<OrdersViewModel>();
            _usersVm = _factory.Create<UsersViewModel>();
            _reviewsVm = _factory.Create<ReviewsViewModel>();

            Tabs = new ObservableCollection<object>
            {
                _overviewVm,
                _restaurantsVm,
                _ordersVm,
                _usersVm,
                _reviewsVm
            };

            SelectedTab = Tabs.First();

            LogoutCommand = new RelayCommand(_ => Logout());
            RefreshCommand = new RelayCommand(_ => LoadAllDataAsync());
            SelectTabCommand = new RelayCommand(tab => { if (tab != null) SelectedTab = tab; });
        }

        private async void OnLoginSuccess(string token, string username)
        {
            _api.SetToken(token);
            AdminUsername = username;

            var loaded = await LoadAllDataAsync();

            if (loaded)
            {
                IsLoggedIn = true;
                LoginVM.ErrorMessage = "";
            }
            else
            {
                _api.SetToken("");
                AdminUsername = "";
                IsLoggedIn = false;
                LoginVM.ErrorMessage = "❌ Sikeres login után a védett adatok betöltése 401 hibával leállt.";
            }
        }

        private void Logout()
        {
            _api.SetToken("");
            AdminUsername = "";
            IsLoggedIn = false;
            LoginVM.Password = "";
            LoginVM.ErrorMessage = "";
            SelectedTab = Tabs.First();
        }

        private async Task<bool> LoadAllDataAsync()
        {
            IsRefreshing = true;
            try
            {
                var restaurants = await _api.GetRestaurantsAsync();
                var orders = await _api.GetOrdersAsync();
                var users = await _api.GetUsersAsync();
                var reviews = await _api.GetAdminReviewsAsync();
                var foods = await _api.GetFoodsAsync();

                _overviewVm.Load(restaurants, orders, users, foods);
                _restaurantsVm.Load(restaurants);
                _ordersVm.Load(orders, users);
                _usersVm.Load(users);
                _reviewsVm.Load(reviews);

                LastUpdated = $"Frissítve: {DateTime.Now:HH:mm:ss}";
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Hiba az adatok betöltésekor:\n{ex.Message}",
                    "Hiba",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);

                return false;
            }
            finally
            {
                IsRefreshing = false;
            }
        }
    }
}
