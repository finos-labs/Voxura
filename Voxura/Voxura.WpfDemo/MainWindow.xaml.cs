using System.ComponentModel;
using System.Windows;
using Voxura.WpfDemo.ViewModels;

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
    }
}