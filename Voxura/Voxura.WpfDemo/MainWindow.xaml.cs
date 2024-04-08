using System.ComponentModel;
using System.Windows;
using Voxura.WpfDemo.ViewModels;
using WindowsInput;

namespace Voxura.WpfDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private MainViewModel _viewModel = new MainViewModel();
        
        public MainWindow()
        {
            InitializeComponent();
            _viewModel.Initialize(DesignerProperties.GetIsInDesignMode(this));
            DataContext = _viewModel;
        }

        private void StartDictation()
        {
            UserText.Focus();
            // send win+h to start dictation
            new InputSimulator().Keyboard.ModifiedKeyStroke(VirtualKeyCode.LWIN, VirtualKeyCode.VK_H);
        }

        private void DictateButton_Click(object sender, RoutedEventArgs e)
        {
            StartDictation();
        }
    }
}