using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Store.ViewModels;

public partial class IncomePageViewModel : ViewModelBase
{
    public string Income { get; set; } = "Thu nhập";
    
    private readonly ObservableCollection<int> _values;
    public ISeries[] _series { get; set; }
    public Axis[] xAxis { get; set; }
    public Axis[] yAxis { get; set; }
    public IncomePageViewModel()
    {
        _values = new ObservableCollection<int> {0,10, 20, 30, 40, 50, 60, 70, 80, 90, 100};
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
                Labeler = _value => $"{_value}m",
                LabelsRotation = 0
            }
        };

    }
}