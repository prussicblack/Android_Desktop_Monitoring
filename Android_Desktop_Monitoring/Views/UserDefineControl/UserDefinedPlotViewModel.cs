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

namespace Android_Desktop_Monitoring.ViewModels
{
    public partial class UserDefinedPlotViewModel : ViewModelBase
    {
        //[ObservableProperty]
        //private IList<double>? xValues = new List<double>(); //사용은 XValues가 됨.
        //private ObservableCollection<double> xValues = new ObservableCollection<double>();
        //public ObservableCollection<double>? XValues = new ObservableCollection<double>();

        private double _XValue = 0;

        public double XValue
        {
            get => _XValue;
            set
            {

                if (_XValue != value)
                {
                    _XValue = value;
                    OnPropertyChanged(nameof(XValue));
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

        public UserDefinedPlotViewModel(ScottPlot.Color fillColor)
        {
            InitAlarmUpdateTimer();
            FillColor = fillColor;
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

            int r = rand.Next(0, 100);

            XValue = r;

            FillColor = FillColor;

            //OnPropertyChanged(nameof(XValue));
            //UpdateTimer.Start();


        }


    }
}
