using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace MonitoringGUI
{
    public partial class MainWindow : Window
    {
        WebSocketService ws = new();

        public MainWindow()
        {
            InitializeComponent();
            ws.OnUpdate = UpdateUI;
            ws.Start();
        }

        void UpdateUI(ClientStatusModel model)
        {
            Dispatcher.Invoke(() =>
            {
                var existing = ws.Clients.FirstOrDefault(c => c.PcName == model.PcName);
                if (existing != null)
                {
                    existing.Status = model.Status;
                    existing.UnityRunning = model.UnityRunning;
                    existing.IdleMinutes = model.IdleMinutes;
                    existing.FocusLevel = model.FocusLevel;
                    existing.ActiveWindow = model.ActiveWindow;
                    existing.Events = model.Events;
                    existing.CpuUsage = model.CpuUsage;
                    existing.MemoryUsage = model.MemoryUsage;
                    existing.UnityCpuUsage = model.UnityCpuUsage;
                    existing.UnityMemoryUsage = model.UnityMemoryUsage;
                    existing.TodayWorkMinutes = model.TodayWorkMinutes;
                    existing.ScreenshotPath = model.ScreenshotPath;
                }
                else
                {
                    ws.Clients.Add(model);
                }
                
                ClientList.ItemsSource = null;
                ClientList.ItemsSource = ws.Clients;
                
                // 통계 업데이트
                ClientCount.Text = ws.Clients.Count.ToString();
                UnityRunningCount.Text = ws.Clients.Count(c => c.UnityRunning).ToString();
            });
        }
    }

    public class BoolToYesNoConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "예" : "아니오";
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? System.Windows.Media.Brushes.Green : System.Windows.Media.Brushes.Red;
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}

