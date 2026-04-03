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
    public class ReviewsViewModel : BaseViewModel
    {
        public string Title => "⭐ Értékelések";

        private ObservableCollection<ReviewDisplayModel> _reviews = new();
        public ObservableCollection<ReviewDisplayModel> Reviews
        { get => _reviews; set => Set(ref _reviews, value); }

        private IEnumerable<ISeries> _ratingDistributionSeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> RatingDistributionSeries
        { get => _ratingDistributionSeries; set => Set(ref _ratingDistributionSeries, value); }

        public Axis[] RatingXAxes { get; } = new[]
        {
            new Axis
            {
                Labels = new[] { "⭐ 1", "⭐⭐ 2", "⭐⭐⭐ 3", "⭐⭐⭐⭐ 4", "⭐⭐⭐⭐⭐ 5" },
                LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80"))
            }
        };

        public Axis[] DefaultYAxes { get; } = new[]
        {
            new Axis { LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80")) }
        };

        public void Load(List<ReviewDisplayModel> reviews)
        {
            Reviews = new ObservableCollection<ReviewDisplayModel>(reviews);

            var dist = Enumerable.Range(1, 5)
                .Select(r => reviews.Count(rv => rv.Rating == r)).ToArray();

            RatingDistributionSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = dist,
                    Fill = new LinearGradientPaint(SKColor.Parse("#FF6B35"), SKColor.Parse("#FFB627")),
                    Name = "Értékelések"
                }
            };
        }
    }
}
