using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Voxura.WpfDemo.UserControls
{
    /// <summary>
    /// Interaction logic for ChangeIndicator.xaml
    /// </summary>
    public partial class ChangeIndicator : UserControl
    {
        /// <summary>
        /// The property name to react changes to
        /// </summary>
        public string? PropertyName { get; set; }
        private INotifyPropertyChanged? _dc;

        public ChangeIndicator()
        {
            InitializeComponent();
            DataContextChanged += ChangeIndicator_DataContextChanged;

            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Opacity = 0.5; 
            }
        }

        private void ChangeIndicator_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_dc != null)
            {
                _dc.PropertyChanged -= DC_PropertyChanged;
            }

            if (DataContext is INotifyPropertyChanged dc)
            {
                _dc = dc;
                _dc.PropertyChanged += DC_PropertyChanged;
            }

        }

        private void DC_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == PropertyName)
            {
                var anim = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(2));
                BeginAnimation(OpacityProperty, anim);
            }
        }
    }
}
