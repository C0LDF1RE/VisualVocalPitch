using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using VocalPitch.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace VocalPitch.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MonitorPage : Page
    {

        ObservableCollection<MonitorPageViewModel> metadatas = new ObservableCollection<MonitorPageViewModel>();
        DispatcherTimer timer;

        private const int SAMPLE_RATE = 128;

        public MonitorPage()
        {
            this.InitializeComponent();

            radChart.DataContext = metadatas;
        }

        private double Sine(double angle)
        {
            double sinRadianValue = Math.PI * angle / 180;//求弧度值
            double sinValue = Math.Sin(sinRadianValue);//求sin30度，其实sin30度 = sin(PI/6)，但是，数学上或代码上，常常用弧度PI/6,来计算sin(PI/6)，其他函数同理。
            return sinValue;
        }

        private ObservableCollection<MonitorPageViewModel> CreateData()
        {
            ObservableCollection<MonitorPageViewModel> collection = new ObservableCollection<MonitorPageViewModel>();
            Complex[] tbt = new Complex[SAMPLE_RATE];
            for (int i = 0; i < SAMPLE_RATE; i++)
            {
                double val = Sine((double)(i / (double)SAMPLE_RATE * 720));
                tbt[i] = new Complex(val, 0);
                //MonitorPageViewModel nextElement = new MonitorPageViewModel() { Timing = i.ToString(), Value = val };
                //collection.Add(nextElement);
            }
            Complex[] res = FFT.DFT(tbt, SAMPLE_RATE);
            for (int i = 0; i < SAMPLE_RATE/2; i++)
            {
                //Debug.Write($"{res[i].GetModul()} ");
                MonitorPageViewModel nextElement = new MonitorPageViewModel() { Timing = i.ToString(), Value = res[i].GetModul()/32 };
                collection.Add(nextElement);

            }
            return collection;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += Timer_Tick;//每秒触发这个事件，以刷新指针
                                     //timer.Start();
            radChart.Series[0].ItemsSource = CreateData();
        }

        private void Timer_Tick(object sender, object e)
        {
            Random rand = new Random();
            MonitorPageViewModel nextElement = new MonitorPageViewModel() { Timing = DateTime.Now.ToLongTimeString().ToString(), Value = rand.Next(1, 100) };
            metadatas.Add(nextElement);
        }
    }
}
