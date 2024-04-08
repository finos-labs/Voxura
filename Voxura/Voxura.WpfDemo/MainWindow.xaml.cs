using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Voxura.WpfDemo.ViewModels;
using WindowsInput;
using Wpf.Ui.Controls;

namespace Voxura.WpfDemo;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : FluentWindow

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

    private void UserText_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        Debug.WriteLine($"UserText_OnPreviewTextInput: {e.Text}");
    }

    private void UserText_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.UserText = UserText.Text; // needed in addition to binding to get quicker updates
    }
}
