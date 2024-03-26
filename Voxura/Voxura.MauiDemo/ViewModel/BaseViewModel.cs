using System.ComponentModel;
using System.Runtime.CompilerServices;


class BaseViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected T UpdateProperty<T>(ref T value, T? newValue, string name)
    {
        if (!EqualityComparer<T>.Default.Equals(value, newValue))
        {
            value = newValue;
            OnPropertyChanged(name);
        }

        return value;
    }


}