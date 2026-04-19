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



        public void Load(AdminStatsModel stats, List<FoodModel> foods)
        {
            RestaurantCount = stats.RestaurantCount;
            OrderCount = stats.OrderCount;
            UserCount = stats.UserCount;
            TotalRevenue = stats.TotalRevenue.ToString("N0") + " Ft";

            BuildOrderStatusPie(stats);
            BuildRestaurantRevenueChart(stats);
            BuildFoodCategoryChart(foods);
        }

        private void BuildOrderStatusPie(AdminStatsModel stats)
        {
            var colors = new[]
            {
        SKColor.Parse("#FF6B35"),
        SKColor.Parse("#FFB627"),
        SKColor.Parse("#4ECDC4")
    };

            OrderStatusSeries = new ISeries[]
            {
        new PieSeries<double>
        {
            Values = new[] { (double)stats.PaidOrders },
            Name = "Paid",
            Fill = new SolidColorPaint(colors[0]),
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 12
        },
        new PieSeries<double>
        {
            Values = new[] { (double)stats.PendingOrders },
            Name = "Pending",
            Fill = new SolidColorPaint(colors[1]),
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 12
        },
        new PieSeries<double>
        {
            Values = new[] { (double)stats.CompletedOrders },
            Name = "Completed",
            Fill = new SolidColorPaint(colors[2]),
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 12
        }
            };
        }

        private void BuildRestaurantRevenueChart(AdminStatsModel stats)
        {
            var top5 = stats.RestaurantRevenue.Take(5).ToList();

            RestaurantRevenueSeries = new ISeries[]
            {
        new ColumnSeries<double>
        {
            Values = top5.Select(x => (double)x.Revenue).ToArray(),
            Fill = new LinearGradientPaint(SKColor.Parse("#FF6B35"), SKColor.Parse("#FFB627")),
            Name = "Bevétel (Ft)"
        }
            };

            RestaurantRevenueXAxes = new[]
            {
        new Axis
        {
            Labels = top5.Select(x => x.RestaurantName).ToArray(),
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
