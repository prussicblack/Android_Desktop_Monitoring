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

    public string LbPlot1 { get; set; } = "Plot1";
    public string LbPlot2 { get; set; } = "Plot2";
    public string LbPlot3 { get; set; } = "Plot3";
    public string LbPlot4 { get; set; } = "Plot4";

    public TCPClient_Receiver client = null;

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



        LbPlot1 = "CPU";
        LbPlot2 = "RAM";
        LbPlot3 = "NetWork";
        LbPlot4 = "GPU";

        //Device 초기화 및 생성.
        client = new TCPClient_Receiver();
        client.InitConnect();
    }

    //나중에 알람 관련 내용 추가할것..
    //소켓 연결 끊김 등의 내용이 들어가야 함.


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
