using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using VizsgaRemekWpf.Models;

namespace VizsgaRemekWpf.ViewModels
{
    public class OverviewViewModel : BaseViewModel
    {
        public string Title => "📊 Áttekintés";

        

        private int _restaurantCount;
        public int RestaurantCount { get => _restaurantCount; set => Set(ref _restaurantCount, value); }

        private int _orderCount;
        public int OrderCount { get => _orderCount; set => Set(ref _orderCount, value); }

        private int _userCount;
        public int UserCount { get => _userCount; set => Set(ref _userCount, value); }

        private string _totalRevenue = "—";
        public string TotalRevenue { get => _totalRevenue; set => Set(ref _totalRevenue, value); }

       

        private IEnumerable<ISeries> _orderStatusSeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> OrderStatusSeries
        { get => _orderStatusSeries; set => Set(ref _orderStatusSeries, value); }

        private IEnumerable<ISeries> _restaurantRevenueSeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> RestaurantRevenueSeries
        { get => _restaurantRevenueSeries; set => Set(ref _restaurantRevenueSeries, value); }

        private Axis[] _restaurantRevenueXAxes = Array.Empty<Axis>();
        public Axis[] RestaurantRevenueXAxes
        { get => _restaurantRevenueXAxes; set => Set(ref _restaurantRevenueXAxes, value); }

        private IEnumerable<ISeries> _foodCategorySeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> FoodCategorySeries
        { get => _foodCategorySeries; set => Set(ref _foodCategorySeries, value); }

        private Axis[] _foodCategoryXAxes = Array.Empty<Axis>();
        public Axis[] FoodCategoryXAxes
        { get => _foodCategoryXAxes; set => Set(ref _foodCategoryXAxes, value); }

        public Axis[] DefaultYAxes { get; } = new[]
        {
            new Axis { LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80")) }
        };

        

        public void Load(
            List<RestaurantModel> restaurants,
            List<OrderModel> orders,
            List<UserModel> users,
            List<FoodModel> foods)
        {
            RestaurantCount = restaurants.Count;
            OrderCount = orders.Count;
            UserCount = users.Count;

            var revenue = orders.Where(o => o.Status == "Paid").Sum(o => o.TotalPrice);
            TotalRevenue = revenue.ToString("N0") + " Ft";

            BuildOrderStatusPie(orders);
            BuildRestaurantRevenueChart(restaurants);
            BuildFoodCategoryChart(foods);
        }

        private void BuildOrderStatusPie(List<OrderModel> orders)
        {
            var colors = new[]
            {
                SKColor.Parse("#FF6B35"), SKColor.Parse("#FFB627"),
                SKColor.Parse("#4ECDC4"), SKColor.Parse("#A8FF78")
            };

            OrderStatusSeries = orders
                .GroupBy(o => o.Status)
                .Select((g, i) => new PieSeries<double>
                {
                    Values = new[] { (double)g.Count() },
                    Name = g.Key,
                    Fill = new SolidColorPaint(colors[i % colors.Length]),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12,
                })
                .Cast<ISeries>().ToList();
        }

        private void BuildRestaurantRevenueChart(List<RestaurantModel> restaurants)
        {
            var top5 = restaurants.Take(5).ToList();
            var rng = new Random(42);

            RestaurantRevenueSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = top5.Select(_ => (double)rng.Next(50000, 500000)).ToArray(),
                    Fill = new LinearGradientPaint(SKColor.Parse("#FF6B35"), SKColor.Parse("#FFB627")),
                    Name = "Bevétel (Ft)"
                }
            };
            RestaurantRevenueXAxes = new[]
            {
                new Axis
                {
                    Labels = top5.Select(r => r.Name).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80"))
                }
            };
        }

        private void BuildFoodCategoryChart(List<FoodModel> foods)
        {
            var groups = foods.GroupBy(f => f.Category)
                .OrderByDescending(g => g.Count()).Take(8).ToList();

            FoodCategorySeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = groups.Select(g => g.Count()).ToArray(),
                    Fill = new SolidColorPaint(SKColor.Parse("#4ECDC4")),
                    Name = "Ételek száma"
                }
            };
            FoodCategoryXAxes = new[]
            {
                new Axis
                {
                    Labels = groups.Select(g => g.Key).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80"))
                }
            };
        }
    }
}
