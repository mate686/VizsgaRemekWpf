using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using VizsgaRemekWpf.Services;

namespace VizsgaRemekWpf.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ApiService _api;

   
        public event Action<string, string>? LoginSuccess; 

        public LoginViewModel(ApiService api)
        {
            _api = api;
            LoginCommand = new RelayCommand(_ => ExecuteLogin());
        }


        private string _username = "";
        public string Username
        {
            get => _username;
            set => Set(ref _username, value);
        }


        public string Password { get; set; } = "";

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set => Set(ref _errorMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                Set(ref _isLoading, value);
                ((RelayCommand)LoginCommand).RaiseCanExecuteChanged();
            }
        }

      

        public ICommand LoginCommand { get; }

        private async void ExecuteLogin()
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "❌ Kérjük töltsd ki az összes mezőt.";
                return;
            }

            IsLoading = true;
            try
            {
                var result = await _api.LoginAsync(Username, Password);

                if (result == null)
                {
                    ErrorMessage = "❌ Hibás felhasználónév vagy jelszó.";
                    return;
                }

                if (!result.IsAdmin)
                {
                    ErrorMessage = "⛔ Hozzáférés megtagadva. Csak adminok léphetnek be.";
                    return;
                }

                _api.SetToken(result.Token);
                LoginSuccess?.Invoke(result.Token, result.Username);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"❌ Kapcsolódási hiba: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
