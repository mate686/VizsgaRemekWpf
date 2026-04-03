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
    public class RestaurantsViewModel : BaseViewModel
    {
        public string Title => "🏪 Éttermek";

        private ObservableCollection<RestaurantModel> _restaurants = new();
        public ObservableCollection<RestaurantModel> Restaurants
        { get => _restaurants; set => Set(ref _restaurants, value); }

        private IEnumerable<ISeries> _orderCountSeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> OrderCountSeries
        { get => _orderCountSeries; set => Set(ref _orderCountSeries, value); }

        private Axis[] _restaurantXAxes = Array.Empty<Axis>();
        public Axis[] RestaurantXAxes
        { get => _restaurantXAxes; set => Set(ref _restaurantXAxes, value); }

        public Axis[] DefaultYAxes { get; } = new[]
        {
            new Axis { LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80")) }
        };

        public void Load(List<RestaurantModel> restaurants)
        {
            Restaurants = new ObservableCollection<RestaurantModel>(restaurants);

            var top5 = restaurants.Take(5).ToList();
            var rng = new Random(7);

            OrderCountSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = top5.Select(_ => (double)rng.Next(10, 200)).ToArray(),
                    Fill = new SolidColorPaint(SKColor.Parse("#A8FF78")),
                    Name = "Rendelések"
                }
            };
            RestaurantXAxes = new[]
            {
                new Axis
                {
                    Labels = top5.Select(r => r.Name).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80"))
                }
            };
        }
    }
}
