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
using VizsgaRemekWpf.Models;

namespace VizsgaRemekWpf.Services
{
    public class ApiService
    {
        private const string BaseUrl = "http://localhost:5000/api";

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

        public async Task<LoginResult?> LoginAsync(string username, string password)
        {
            var payload = new { username, password };
            var content = new StringContent(
                JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

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
                    c.Type == "role")
                ?.Value ?? "";

            return new LoginResult
            {
                Token = token,
                Username = username,
                IsAdmin = role.Equals("Admin", StringComparison.OrdinalIgnoreCase)
            };
        }

        public async Task<List<RestaurantModel>> GetRestaurantsAsync()
        {
            var r = await _http.GetAsync($"{BaseUrl}/restaurant/allRestaurant");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<RestaurantModel>>(_json) ?? new();
        }

        public async Task<List<OrderModel>> GetOrdersAsync()
        {
            var r = await _http.GetAsync($"{BaseUrl}/orders");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<OrderModel>>(_json) ?? new();
        }

        public async Task<List<FoodModel>> GetFoodsAsync()
        {
            var r = await _http.GetAsync($"{BaseUrl}/foods");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<FoodModel>>(_json) ?? new();
        }

        public async Task<List<UserModel>> GetUsersAsync()
        {
            var r = await _http.GetAsync($"{BaseUrl}/admin/users");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<UserModel>>(_json) ?? new();
        }

        public async Task<List<ReviewDisplayModel>> GetAdminReviewsAsync()
        {
            var r = await _http.GetAsync($"{BaseUrl}/admin/reviews");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<ReviewDisplayModel>>(_json) ?? new();
        }
    }
}
