using System;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Generic;
using System.Linq;
namespace Android_Desktop_Monitoring.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private IList<double>? xValues;

    [ObservableProperty]
    private IList<double>? yValues;


    public IRelayCommand<string?> CMD_TestBTN { get; }

    public UserDefinedPlotViewModel Plot1 { get; } = new UserDefinedPlotViewModel();
    public UserDefinedPlotViewModel Plot2 { get; } = new UserDefinedPlotViewModel();
    public UserDefinedStackedScatterPlotViewModel Plot3 { get; } = new UserDefinedStackedScatterPlotViewModel();
    public UserDefinedPlotViewModel Plot4 { get; } = new UserDefinedPlotViewModel();

    public MainViewModel()
    {
        // 마찬가지 Avalonia 방식으로 변경.
        //CMD_TestBTN = new DelegateCommand(HandleUnload);
        //CMD_TestBTN = new RelayCommand<string?>(ExecuteCMD_TestBTNt, CanExecuteExecuteCMD_TestBTNt);

        //Plot1 = new UserDefinedPlotViewModel();

        Plot1.ChangeColor(ScottPlot.Colors.Green.WithAlpha(.7));
        Plot2.ChangeColor(ScottPlot.Colors.Blue.WithAlpha(.7));
        Plot3.ChangeColor(ScottPlot.Colors.Purple.WithAlpha(.7), ScottPlot.Colors.Green.WithAlpha(.7));
        Plot4.ChangeColor(ScottPlot.Colors.Yellow.WithAlpha(.7));

    }




    private void ExecuteCMD_TestBTNt(string? name)
    {
        // 실제 동작 구현
        int count = 1000;
        XValues = Enumerable.Range(0, count).Select(i => (double)i).ToList();

    }

    private bool CanExecuteExecuteCMD_TestBTNt(string? name)
    {
        // 실행 조건 로직 (예: return !string.IsNullOrWhiteSpace(name); )
        return true;
    }
}
