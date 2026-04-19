using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VizsgaRemekWpf.Models;

namespace VizsgaRemekWpf.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public string Title => "📦 Rendelések";

        private ObservableCollection<OrderModel> _orders = new();
        public ObservableCollection<OrderModel> Orders
        {
            get => _orders;
            set => Set(ref _orders, value);
        }

        private string _totalRevenue = "0 Ft";
        public string TotalRevenue
        {
            get => _totalRevenue;
            set => Set(ref _totalRevenue, value);
        }

        private string _successfulOrders = "0";
        public string SuccessfulOrders
        {
            get => _successfulOrders;
            set => Set(ref _successfulOrders, value);
        }

        private string _pendingOrders = "0";
        public string PendingOrders
        {
            get => _pendingOrders;
            set => Set(ref _pendingOrders, value);
        }

        private IEnumerable<ISeries> _statusSeries = new List<ISeries>();
        public IEnumerable<ISeries> StatusSeries
        {
            get => _statusSeries;
            set => Set(ref _statusSeries, value);
        }

        public void Load(List<OrderModel> orders, List<UserModel> users)
        {
            foreach (var order in orders)
            {
                var user = users.FirstOrDefault(u => u.Id == order.UserId);

                order.UserName = user?.Name;

                if (string.IsNullOrWhiteSpace(order.UserName))
                    order.UserName = user?.UserName ?? order.UserId;
            }

            Orders = new ObservableCollection<OrderModel>(orders);

            var successfulStatuses = new[] { "paid", "completed" };

            var revenue = orders
                .Where(o => successfulStatuses.Contains((o.Status ?? "").ToLower()))
                .Sum(o => o.TotalPrice);

            TotalRevenue = revenue.ToString("N0") + " Ft";

            SuccessfulOrders = orders.Count(o =>
                successfulStatuses.Contains((o.Status ?? "").ToLower())).ToString();

            PendingOrders = orders.Count(o =>
                (o.Status ?? "").ToLower() == "pending").ToString();

            var colors = new[]
            {
                SKColor.Parse("#FF6B35"),
                SKColor.Parse("#4ECDC4"),
                SKColor.Parse("#FFB627"),
                SKColor.Parse("#A8FF78"),
                SKColor.Parse("#C77DFF")
            };

            StatusSeries = orders
                .GroupBy(o => (o.Status ?? "").ToLower())
                .Select((g, i) => new PieSeries<double>
                {
                    Values = new[] { (double)g.Count() },
                    Name = g.Key,
                    Fill = new SolidColorPaint(colors[i % colors.Length]),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12
                })
                .Cast<ISeries>()
                .ToList();
        }
    }
}