using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using VizsgaRemekWpf.Models;

namespace VizsgaRemekWpf.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://localhost:4000/api";

        private readonly HttpClient _http = new();
        private readonly JsonSerializerOptions _json = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public void SetToken(string token)
        {
            _http.DefaultRequestHeaders.Authorization =
                string.IsNullOrEmpty(token)
                    ? null
                    : new AuthenticationHeaderValue("Bearer", token);
        }

        public async Task<AdminStatsModel> GetAdminStatsAsync()
        {
            var r = await _http.GetAsync($"{BaseUrl}/admin/stats");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<AdminStatsModel>(_json) ?? new AdminStatsModel();
        }

        private async Task<T> GetAsync<T>(string url)
        {
            

            var response = await _http.GetAsync(url);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"{url} -> {(int)response.StatusCode} {response.ReasonPhrase}\n{body}");

            return JsonSerializer.Deserialize<T>(body, _json)
                   ?? throw new Exception("Üres válasz.");
        }

        

        public async Task<LoginResult?> LoginAsync(string username, string password)
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { username, password }),
                Encoding.UTF8, "application/json");

            var response = await _http.PostAsync($"{BaseUrl}/auth/login", content);
            if (!response.IsSuccessStatusCode) return null;

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var token = doc.RootElement.GetProperty("token").GetString() ?? "";

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var role = jwt.Claims
                .FirstOrDefault(c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" ||
                    c.Type == "role")?.Value ?? "";

            return new LoginResult
            {
                Token = token,
                Username = username,
                IsAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            };
        }

        public Task<List<RestaurantModel>> GetRestaurantsAsync()
        => GetAsync<List<RestaurantModel>>($"{BaseUrl}/restaurant/allRestaurant");


        
        
   

        public Task<List<OrderModel>> GetOrdersAsync()
         => GetAsync<List<OrderModel>>($"{BaseUrl}/orders/allAdmin");

        public Task<List<UserModel>> GetUsersAsync()
            => GetAsync<List<UserModel>>($"{BaseUrl}/admin/users");

        public Task<List<ReviewDisplayModel>> GetAdminReviewsAsync()
            => GetAsync<List<ReviewDisplayModel>>($"{BaseUrl}/admin/reviews");

        public Task<List<FoodModel>> GetFoodsAsync()
            => GetAsync<List<FoodModel>>($"{BaseUrl}/foods");

    }
}
