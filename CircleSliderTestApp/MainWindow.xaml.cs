using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace CircleSliderTestApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private int intValue;
        private double doubleValue;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int IntValue
        {
            get => intValue;
            set
            {
                intValue = value;
                this.NotifyPropertyChanged();
            }
        }

        public double DoubleValue
        {
            get => doubleValue;
            set
            {
                doubleValue = value;
                this.NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;
            this.IntValue = 50;
            this.DoubleValue = 50;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
