using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

namespace Store.ViewModels.Report;

public class AnalysePageViewModel : ViewModelBase
{
    public string AnalysePage { get; set; } = "Phân tích";
    
}