using System.ComponentModel;

namespace MonitoringGUI.ViewModels
{
    public class ClientViewModel : INotifyPropertyChanged
    {
        private string _pcName;
        private string _status;
        private bool _unityRunning;
        private int _idleMinutes;
        private double _focusLevel;
        private string _activeWindow;

        public string PcName
        {
            get => _pcName;
            set { _pcName = value; OnPropertyChanged(nameof(PcName)); }
        }

        public string Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(nameof(Status)); }
        }

        public bool UnityRunning
        {
            get => _unityRunning;
            set { _unityRunning = value; OnPropertyChanged(nameof(UnityRunning)); }
        }

        public int IdleMinutes
        {
            get => _idleMinutes;
            set { _idleMinutes = value; OnPropertyChanged(nameof(IdleMinutes)); }
        }

        public double FocusLevel
        {
            get => _focusLevel;
            set { _focusLevel = value; OnPropertyChanged(nameof(FocusLevel)); }
        }

        public string ActiveWindow
        {
            get => _activeWindow;
            set { _activeWindow = value; OnPropertyChanged(nameof(ActiveWindow)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

