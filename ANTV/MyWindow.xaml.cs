using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ANTV
{
    /// <summary>
    /// MyWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MyWindow : Window
    {
        public ObservableCollection<Channel> Channels { get; set; }
        public ICommand CopyToClipboardCommand { get; set; }
        public MyWindow()
        {
            Channels = new ObservableCollection<Channel>
            {
                new Channel { MediaUrl = "http://example.com/video1" },
                new Channel { MediaUrl = "http://example.com/video2" }
            };
            CopyToClipboardCommand = new RelayCommand<string>(CopyToClipboard);
            InitializeComponent();
        }

        private void CopyToClipboard(string url)
        {
            if (!string.IsNullOrEmpty(url)) { Clipboard.SetText(url); }
        }
    } 



    public class Channel { public string MediaUrl { get; set; } }
    public class RelayCommand<T> : ICommand 
    { 
        private readonly Action<T> _execute; 
        private readonly Func<T, bool> _canExecute; 
        public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null) 
        { 
            _execute = execute ?? throw new ArgumentNullException(nameof(execute)); 
            _canExecute = canExecute; 
        } 
        public bool CanExecute(object parameter) 
        { 
            return _canExecute == null || _canExecute((T)parameter); 
        } 
        public void Execute(object parameter) 
        { 
            _execute((T)parameter); 
        } 
        public event EventHandler CanExecuteChanged 
        { 
            add 
            { 
                CommandManager.RequerySuggested += value; 
            } 
            remove 
            { 
                CommandManager.RequerySuggested -= value; 
            } 
        }
    }
}
