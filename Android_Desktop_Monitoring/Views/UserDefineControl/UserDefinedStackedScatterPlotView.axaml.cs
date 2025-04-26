using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using System.Drawing;
using Avalonia.Styling;
using ScottPlot.Avalonia;
using ScottPlot;
using System.ComponentModel;
using Android_Desktop_Monitoring.ViewModels;
using System;
using System.Linq;
using HarfBuzzSharp;
using ScottPlot.Plottables;
using Color = Avalonia.Media.Color;
using Colors = Avalonia.Media.Colors;

namespace Android_Desktop_Monitoring;

public partial class UserDefinedStackedScatterPlotView : UserControl
{
    private UserDefinedStackedScatterPlotViewModel? _viewModel;

    private static int buffercount = 120;

    private readonly double[] _y1buffer = new double[buffercount]; // 고정된 100 포인트 버퍼
    private readonly double[] _xbuffer = Enumerable.Range(0, buffercount).Select(i => (double)i).ToArray();
    private readonly double[] _y2buffer = new double[buffercount];
    //private int _index = 0;
    //private SignalPlot? _signal;

    ScottPlot.Color FillColor = ScottPlot.Colors.Green.WithAlpha(.7);


    public UserDefinedStackedScatterPlotView()
    {
        InitializeComponent();

        PlotInitialze();

        this.DataContextChanged += OnDataContextChanged;
        //FillColor = PlotColor;

        //if(!(_viewModel is null))
        //    _viewModel.XValues.CollectionChanged += (_, __) => RefreshPlot();
        //OnDataContextChanged(this, EventArgs.Empty);
    }

    private void PlotInitialze()
    {
        ScottPlot.Plot? plt = UserDefindStackedScatterPlot.Plot;

        ScottPlot.PlotStyle? style = plt.GetStyle();

        style.FigureBackgroundColor = ScottPlot.Colors.Gray;

        style.DataBackgroundColor = ScottPlot.Colors.White;

        //style.AxisColor = 
        plt.SetStyle(style);

        //plt.Grid.IsVisible = false;
        
        //Plot Tick Meter 삭제.
        plt.Axes.Bottom.IsVisible = false;
        plt.Axes.Left.IsVisible = false;
        plt.Axes.Right.IsVisible = false;
        plt.Axes.Top.IsVisible = false;

        plt.Axes.SetLimitsX(0, _xbuffer.Length-1);
        plt.Axes.SetLimitsY(0,100);

        plt.PlotControl.UserInputProcessor.IsEnabled = false;

    }

    private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(_viewModel.Yvalues))
        {
            AddNewValue(_viewModel.Yvalues.Y1, _viewModel.Yvalues.Y2);
            RefreshPlot();
        }
        else if (e.PropertyName is nameof(_viewModel.FillColor))
        {
            FillColor = _viewModel.FillColor;
        }
    }

    private void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (_viewModel != null)
        {
            // 이전 ViewModel의 이벤트 제거 (중복 방지)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = DataContext as UserDefinedStackedScatterPlotViewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            RefreshPlot(); // 초기화 시점에 그리기
        }
    }

    private void RefreshPlot()
    {
        //if (_viewModel.XValues is null || _viewModel.YValues is null)
        //if (_viewModel.XValue is null)
        //    return;

        UserDefindStackedScatterPlot.Plot.Clear();
        
        
        //SignalPlot은 Fill Y 기능이 없음.--;;
        //ScatterPlot는 점이 생겨서 안이쁨...--;
        //FillY는 ZeroFill기준으로 FillY 그래프를 사용.
        //var plot = UserDefindPlot.Plot.Add.Signal(_buffer, 1, ScottPlot.Colors.Green);

        var plot1 = UserDefindStackedScatterPlot.Plot.Add.ScatterLine(_xbuffer, _y1buffer, ScottPlot.Colors.Black);
        var plot2 = UserDefindStackedScatterPlot.Plot.Add.ScatterLine(_xbuffer, _y2buffer, ScottPlot.Colors.Black);

        //Set Plot Style
        plot1.LineColor = ScottPlot.Colors.Transparent;
        plot2.LineColor = ScottPlot.Colors.Transparent;

        plot1.LineWidth = Single.MinValue;
        plot2.LineWidth = Single.MinValue;

        plot1.FillYColor = ScottPlot.Colors.Yellow.WithAlpha(.7);
        plot2.FillYColor = ScottPlot.Colors.Green.WithAlpha(.7);


        UserDefindStackedScatterPlot.Refresh();


    }

    private void AddNewValue(double valuey1, double valuey2)
    {
        // 왼쪽으로 밀기
        for (int i = 0; i < _y1buffer.Length - 1; i++)
            _y1buffer[i] = _y1buffer[i + 1];
        // 마지막에 새로운 값 추가
        _y1buffer[^1] = valuey1;

        for (int i = 0; i < _y2buffer.Length - 1; i++)
            _y2buffer[i] = _y2buffer[i + 1];
        _y2buffer[^1] = valuey2;
    }



}

