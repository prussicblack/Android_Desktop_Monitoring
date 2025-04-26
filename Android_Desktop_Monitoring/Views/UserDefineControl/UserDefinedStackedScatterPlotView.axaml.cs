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
    private UserDefinedPlotViewModel? _viewModel;

    private static int buffercount = 120;

    private readonly double[] _y1buffer = new double[buffercount]; // ������ 100 ����Ʈ ����
    private readonly double[] _xbuffer = Enumerable.Range(0, buffercount).Select(i => (double)i).ToArray();
    private readonly double[] _y2buffer = Enumerable.Range(0, buffercount).Select(i => (double)0).ToArray();
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
        
        //Plot Tick Meter ����.
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
        if (e.PropertyName is nameof(_viewModel.XValue))
        {
            AddNewValue(_viewModel.XValue);
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
            // ���� ViewModel�� �̺�Ʈ ���� (�ߺ� ����)
            _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        _viewModel = DataContext as UserDefinedPlotViewModel;

        if (_viewModel != null)
        {
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            RefreshPlot(); // �ʱ�ȭ ������ �׸���
        }
    }

    private void RefreshPlot()
    {
        //if (_viewModel.XValues is null || _viewModel.YValues is null)
        //if (_viewModel.XValue is null)
        //    return;

        UserDefindPlot.Plot.Clear();
        
        
        //SignalPlot�� Fill Y ����� ����.--;;
        //ScatterPlot�� ���� ���ܼ� ���̻�...--;
        //FillY�� ZeroFill�������� FillY �׷����� ���.
        //var plot = UserDefindPlot.Plot.Add.Signal(_buffer, 1, ScottPlot.Colors.Green);

        var plot = UserDefindPlot.Plot.Add.FillY(_xbuffer, _y1buffer, _y2buffer);
        
        //plot.FillY = true;
        plot.LineColor = ScottPlot.Colors.Transparent;
        //plot.FillColor = ScottPlot.Colors.Green.WithAlpha(.7);
        plot.FillColor = FillColor;
        //plot.LineStyle.

        UserDefindPlot.Refresh();


    }

    private void AddNewValue(double value)
    {
        // �������� �б�
        for (int i = 0; i < _y1buffer.Length - 1; i++)
            _y1buffer[i] = _y1buffer[i + 1];

        // �������� ���ο� �� �߰�
        _y1buffer[^1] = value;

    }



}

