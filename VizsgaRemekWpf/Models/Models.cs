using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VizsgaRemekWpf.Models
{
    public class LoginResult
    {
        public string Token { get; set; } = "";
        public bool IsAdmin { get; set; }
        public string Username { get; set; } = "";
    }

    public class RestaurantModel
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string OpeningHours { get; set; } = "";
        public string Category { get; set; } = "";
        public string RestaurantImageUrl { get; set; } = "";
    }

    public class OrderModel
    {
        public Guid PublicId { get; set; }
        public string UserId { get; set; } = "";
        public decimal TotalPrice { get; set; }
        public string Status { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public List<OrderItemModel> OrderItems { get; set; } = new();
    }

    public class OrderItemModel
    {
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public int RestaurantId { get; set; }
    }

    public class FoodModel
    {
        public Guid PublicId { get; set; }
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public decimal Price { get; set; }
    }

    public class UserModel
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public string UserName { get; set; } = "";
        public string Email { get; set; } = "";
        public int Points { get; set; }
        public string Role { get; set; } = "User";
    }

    public class ReviewDisplayModel
    {
        public Guid PublicId { get; set; }
        public string RestaurantName { get; set; } = "";
        public string UserName { get; set; } = "";
        public int Rating { get; set; }
        public string Comment { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }
}
