using Voxura.Core;
using Voxura.MauiDemo.Model;
using Voxura.MauiDemo.ViewModel;
using System.Text.Json;
using System;

namespace Voxura.MauiDemo;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();

        BindingContext = new MainViewModel(new ApplicationConfig());
    }

    private void OnDebugModeToggled(object sender, ToggledEventArgs e)
    {
        Debug.IsVisible = e.Value;
        InterimTranscript.IsVisible = e.Value;
    }
}
