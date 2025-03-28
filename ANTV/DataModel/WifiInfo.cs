using System.ComponentModel;

public class WifiInfo : INotifyPropertyChanged
{
    private string _name;
    public string Name { get=>_name; set { _name=value;} }

    private int _latency;
    public int Latency
    {
        get => _latency;
        set
        {
            if (_latency != value)
            {
                _latency = value;
                OnPropertyChanged(nameof(Latency));
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string Color { get; set; }
    public string GroupName { get; set; }

    public string Operators { get; set; }
    public bool IsRadio { get; set; }

    public string? ScreenshotPath { get; set; }

    //public string DefaultImagePath = "C:\\Users\\Sky\\source\\repos\\ConsoleAppNet8\\ANTV\\Image\\placeholder.jpg";
}
