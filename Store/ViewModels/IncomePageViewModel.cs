using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace Store.ViewModels;

public partial class IncomePageViewModel : ViewModelBase
{
    public string Income { get; set; } = "Thu nhập";
    
    private readonly ObservableCollection<int> _values;
    public LabelVisual Title { get; set; }
    public ISeries[] _series { get; set; }
    public Axis[] xAxis { get; set; }
    public Axis[] yAxis { get; set; }
    public IncomePageViewModel()
    {
        _values = new ObservableCollection<int> {0,10, 20, 30, 40, 50, 60, 70, 80, 90, 100};

        Title = new LabelVisual()
        {
            Text = "Thống kê doanh thu",
            TextSize = 18,
            Padding = new LiveChartsCore.Drawing.Padding(10),
            Paint = new SolidColorPaint(SKColors.Red)
        };
        
        _series = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = _values,
                GeometrySize = 5,
                LineSmoothness = 0
            }
        };

        xAxis = new Axis[]
        {
            new Axis()
            {
                Name = "Tháng",
                NameTextSize = 18,
                Labeler = _value => $"{_value}m",
                LabelsRotation = 0
            }
        };

        yAxis = new Axis[]
        {
            new Axis()
            {
                Name = "Doanh thu",
                NameTextSize = 18,
            }
        };
    }
}