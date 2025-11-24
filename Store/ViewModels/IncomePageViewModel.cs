using System;
using System.Collections.ObjectModel;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;

namespace Store.ViewModels;

public class IncomePageViewModel : ViewModelBase
{
    public string Income { get; set; } = "Thu nhập";
    
    private readonly ObservableCollection<int> _values;
    public ISeries[] _series { get; set; }
    public Axis[] xAxis { get; set; }
    public Axis[] yAxis { get; set; }
    public IncomePageViewModel()
    {
        int currentYear = DateTime.Now.Year;
        _values = new ObservableCollection<int> {10, 20, 15, 30, 25, 40, 35};
        
        _series = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = _values,
                GeometrySize = 5,
                LineSmoothness = 0
            }
        };

        xAxis =
        [
            new Axis
            {
                Name = $"Năm {currentYear}",
                NameTextSize = 15,
                Labels = ["Tháng 1" ,"Tháng 2" ,"Tháng 3" ,"Tháng 4" ,"Tháng 5" ,"Tháng 6" ,"Tháng 7" ,"Tháng 8" ,"Tháng 9" ,"Tháng 10" ,"Tháng 11" ,"Tháng 12"],
            }
        ];

        yAxis =
        [
            new Axis
            {
                Name = "Doanh thu (triệu VNĐ)",
                NameTextSize = 15,
                Labeler = value => $"{value} M"
            }
        ];
    }
}