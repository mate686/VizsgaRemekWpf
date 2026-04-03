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
    public class UsersViewModel : BaseViewModel
    {
        public string Title => "👥 Felhasználók";

        private ObservableCollection<UserModel> _users = new();
        public ObservableCollection<UserModel> Users
        { get => _users; set => Set(ref _users, value); }

        private IEnumerable<ISeries> _pointsSeries = Enumerable.Empty<ISeries>();
        public IEnumerable<ISeries> PointsSeries
        { get => _pointsSeries; set => Set(ref _pointsSeries, value); }

        private Axis[] _usersXAxes = System.Array.Empty<Axis>();
        public Axis[] UsersXAxes
        { get => _usersXAxes; set => Set(ref _usersXAxes, value); }

        public Axis[] DefaultYAxes { get; } = new[]
        {
            new Axis { LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80")) }
        };

        public void Load(List<UserModel> users)
        {
            Users = new ObservableCollection<UserModel>(users);

            var top10 = users.OrderByDescending(u => u.Points).Take(10).ToList();

            PointsSeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = top10.Select(u => u.Points).ToArray(),
                    Fill = new LinearGradientPaint(SKColor.Parse("#FFB627"), SKColor.Parse("#FF6B35")),
                    Name = "Pontok"
                }
            };
            UsersXAxes = new[]
            {
                new Axis
                {
                    Labels = top10.Select(u => u.UserName ?? u.Name).ToArray(),
                    LabelsPaint = new SolidColorPaint(SKColor.Parse("#6B6B80"))
                }
            };
        }
    }
}
