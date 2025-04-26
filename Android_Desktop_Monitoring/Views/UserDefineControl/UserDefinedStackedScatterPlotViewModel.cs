using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScottPlot.Palettes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android_Desktop_Monitoring.ViewModels;
using static Android_Desktop_Monitoring.ViewModels.UserDefinedStackedScatterPlotViewModel;
using ScottPlot.TickGenerators.TimeUnits;

namespace Android_Desktop_Monitoring.ViewModels
{
    public partial class UserDefinedStackedScatterPlotViewModel : ViewModelBase
    {
        private yvalues _Yvalues;

        public yvalues Yvalues
        {
            get => _Yvalues;
            set
            {
                if (_Yvalues.Y1 != value.Y1 || _Yvalues.Y2 != value.Y2)
                {
                    _Yvalues = value;
                    OnPropertyChanged(nameof(Yvalues));
                }
            }

        }



        private DispatcherTimer UpdateTimer;
        static readonly TimeSpan TIMESPAN_NORMAL = TimeSpan.FromSeconds(1);
        static readonly TimeSpan TIMESPAN_FREQUENT = TimeSpan.FromMilliseconds(500);

        
        private ScottPlot.Color _FillColor = ScottPlot.Colors.Green.WithAlpha(.7);

        public ScottPlot.Color FillColor
        {
            get => _FillColor;
            set
            {
                //if (_FillColor != value)
                //{
                    _FillColor = value;
                    OnPropertyChanged(nameof(FillColor));
                //}
            }
        }

        static DispatcherTimer CreateTimer(TimeSpan interval, Action onTime)
        {
            var timer = new DispatcherTimer();
            timer.Tick += (s, e) => onTime();
            timer.Interval = interval;
            return timer;
        }

        public UserDefinedStackedScatterPlotViewModel()
        {
            InitAlarmUpdateTimer();
            //FillColor = fillColor;
        }


        private void InitAlarmUpdateTimer()
        {

            UpdateTimer = CreateTimer(TIMESPAN_FREQUENT, OnUpdateDisplayTime);

            UpdateTimer.Start();
        }

        private void OnUpdateDisplayTime()
        {
            //UpdateTimer.Stop();
            Random rand = new Random();

            int r1 = rand.Next(0, 100);
            int r2 = rand.Next(0, 100);

            SetValues(r1, r2);

            FillColor = FillColor;

            //OnPropertyChanged(nameof(XValue));
            //UpdateTimer.Start();


        }

        public void SetValues(double y1, double y2)
        {
            Yvalues = new yvalues(y1, y2); // ← 이러면 둘 다 세팅되면서
        }



        public void ChangeColor(ScottPlot.Color PlotColor)
        {

            FillColor = PlotColor;
        }




        public struct yvalues
        {
            public double Y1 { get; set; }
            public double Y2 { get; set; }

            public yvalues(double y1, double y2)
            {
                Y1 = y1;
                Y2 = y2;
            }

        }

    }
}
