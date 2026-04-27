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

        private string _paidOrders = "0";
        public string PaidOrders
        {
            get => _paidOrders;
            set => Set(ref _paidOrders, value);
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
                {
                    order.UserName = user?.UserName ?? order.UserId;
                }
            }

            Orders = new ObservableCollection<OrderModel>(orders);

            string NormalizeStatus(string? status)
            {
                return (status ?? "").Trim().ToLower();
            }

            var paidCount = orders.Count(o =>
                NormalizeStatus(o.Status) == "paid");

            var completedCount = orders.Count(o =>
                NormalizeStatus(o.Status) == "completed");

            var pendingCount = orders.Count(o =>
                NormalizeStatus(o.Status) == "pending");

            var successfulCount = orders.Count(o =>
                NormalizeStatus(o.Status) == "paid" ||
                NormalizeStatus(o.Status) == "completed");

            var revenue = orders
                .Where(o =>
                    NormalizeStatus(o.Status) == "paid" ||
                    NormalizeStatus(o.Status) == "completed")
                .Sum(o => o.TotalPrice);

            TotalRevenue = revenue.ToString("N0") + " Ft";

            SuccessfulOrders = successfulCount.ToString();

            PaidOrders = paidCount.ToString();

            PendingOrders = pendingCount.ToString();

            StatusSeries = new List<ISeries>
            {
                new PieSeries<double>
                {
                    Values = new[] { (double)pendingCount },
                    Name = "Pending",
                    Fill = new SolidColorPaint(SKColor.Parse("#FFB627")),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12
                },

                new PieSeries<double>
                {
                    Values = new[] { (double)paidCount },
                    Name = "Paid",
                    Fill = new SolidColorPaint(SKColor.Parse("#FF6B35")),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12
                },

                new PieSeries<double>
                {
                    Values = new[] { (double)completedCount },
                    Name = "Completed",
                    Fill = new SolidColorPaint(SKColor.Parse("#4ECDC4")),
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 12
                }
            };
        }
    }
}