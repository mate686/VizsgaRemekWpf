using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VizsgaRemekWpf.Models;

namespace VizsgaRemekWpf.ViewModels
{
    public class OrdersViewModel : BaseViewModel
    {
        public string Title => "📦 Rendelések";

        private ObservableCollection<OrderModel> _orders = new();
        public ObservableCollection<OrderModel> Orders
        { get => _orders; set => Set(ref _orders, value); }

        private string _totalRevenue = "—";
        public string TotalRevenue { get => _totalRevenue; set => Set(ref _totalRevenue, value); }

        private string _paidOrders = "—";
        public string PaidOrders { get => _paidOrders; set => Set(ref _paidOrders, value); }

        private string _pendingOrders = "—";
        public string PendingOrders { get => _pendingOrders; set => Set(ref _pendingOrders, value); }

        private IEnumerable<ISeries> _statusSeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> StatusSeries
        { get => _statusSeries; set => Set(ref _statusSeries, value); }

        public Axis[] DefaultYAxes { get; } = new[]
        {
            new Axis { LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80")) }
        };

        public void Load(List<OrderModel> orders)
        {
            Orders = new ObservableCollection<OrderModel>(orders);

            var revenue = orders.Where(o => o.Status == "Paid").Sum(o => o.TotalPrice);
            TotalRevenue = revenue.ToString("N0") + " Ft";
            PaidOrders = orders.Count(o => o.Status == "Paid").ToString();
            PendingOrders = orders.Count(o => o.Status == "pending").ToString();

            var colors = new[]
            {
                SKColor.Parse("#FF6B35"), SKColor.Parse("#FFB627"),
                SKColor.Parse("#4ECDC4"), SKColor.Parse("#A8FF78")
            };

            StatusSeries = orders.GroupBy(o => o.Status)
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
    }
}
