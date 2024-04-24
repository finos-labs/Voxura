using System;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace Voxura.MauiDemo
{

    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();
            ApiKey.Text = Preferences.Get("OpenAI.ApiKey", "");
        }

        public async void SaveButton_Click(object sender, EventArgs e)
        {
            Preferences.Set("OpenAI.ApiKey", ApiKey.Text);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
            await Toast.Make("Saved!", ToastDuration.Long, 16) .Show(cancellationTokenSource.Token);
        }
    }
}