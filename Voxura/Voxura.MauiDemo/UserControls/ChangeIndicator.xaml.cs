using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System.ComponentModel;

namespace Voxura.MauiDemo.UserControls
{

    /// <summary>
    /// Interaction logic for RFQForm.xaml
    /// </summary>
    public partial class  ChangeIndicator : ContentView
    {
        /// <summary>
        /// The property name to react changes to
        /// </summary>
        public string? PropertyName { get; set; }
        private INotifyPropertyChanged? _dc;

        public ChangeIndicator()
        {
            InitializeComponent();
            BindingContextChanged += ChangeIndicator_BindingContextChanged;

            if (DesignMode.IsDesignModeEnabled)
            {
                Opacity = 0.5;
            }
        }

        private void ChangeIndicator_BindingContextChanged(object sender, System.EventArgs e)
        {
            if (_dc != null)
            {
                _dc.PropertyChanged -= DC_PropertyChanged;
            }

            if (BindingContext is INotifyPropertyChanged dc)
            {
                _dc = dc;
                _dc.PropertyChanged += DC_PropertyChanged;
            }
        }

        private void DC_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                var anim = new Animation(v => Opacity = v, 1, 0, easing: Easing.Linear);
                anim.Commit(this, "FadeAnimation", length: 2000);
            }
        }
    }
}