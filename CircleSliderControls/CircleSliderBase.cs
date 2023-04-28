using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircleSliderControls
{
    public class CircleSliderBase : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(nameof(Value), typeof(double), typeof(CircleSliderBase), new FrameworkPropertyMetadata(0.0, ValueChange)
            {
                BindsTwoWayByDefault = true,
                DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(nameof(MinValue), typeof(double), typeof(CircleSliderBase), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(nameof(MaxValue), typeof(double), typeof(CircleSliderBase), new PropertyMetadata(0.0));


        private Point centerPosition;
        private double previousAngle;
        private double angle;

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public string FormatedValue
        {
            get => this.Value.ToString(ValueStringFormat);
            set
            {
                this.Value = Convert.ToDouble(value);
                this.NotifyPropertyChanged();
            }
        }

        public double MinValue
        {
            get { return (double)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        public double MaxValue
        {
            get { return (double)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private double Angle
        {
            get { return this.angle; }
            set
            {
                if (Math.Abs(this.angle - value) < 0)
                {
                    return;
                }

                this.previousAngle = this.angle;

                this.angle = value;
            }
        }
        private double MinAngle => 0;
        private double MaxAngle => 360;

        public string? ValueStringFormat { get; set; }

        protected virtual Ellipse MainEllipse { get; }
        protected virtual Grid MainGrid { get; }

        public CircleSliderBase()
        {

        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            double halfWidth = this.MainEllipse.Width / 2.0;
            double halfHeight = this.MainEllipse.Height / 2.0;

            this.centerPosition = new Point(halfWidth, halfHeight);
        }

        internal static void ValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not CircleSliderBase circleSlider)
            {
                return;
            }

            double value = (double)e.NewValue;

            if (value > circleSlider.MaxValue)
            {
                circleSlider.Value = circleSlider.MaxValue;

                return;
            }

            if (value < circleSlider.MinValue)
            {
                circleSlider.Value = circleSlider.MinValue;

                return;
            }

            double multiplier = (value / (circleSlider.MaxValue - circleSlider.MinValue));

            circleSlider.Angle = circleSlider.MinAngle + (multiplier * (circleSlider.MaxAngle - circleSlider.MinAngle));

            circleSlider.MainGrid.RenderTransform = new RotateTransform(circleSlider.Angle, circleSlider.centerPosition.X, circleSlider.centerPosition.Y);
            circleSlider.NotifyPropertyChanged(nameof(FormatedValue));
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void EllipseOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Polygon)
            {
                Mouse.Capture((Polygon)sender);
            }
            else
            {
                Mouse.Capture((Ellipse)sender);
            }
        }

        public void EllipseOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        public void EllipseOnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Rotate(e.GetPosition(this.MainEllipse));
            }
        }

        protected void Rotate(Point newPosition)
        {
            double x = 0;

            double halfWidth = this.MainEllipse.Width / 2.0;
            double halfHeight = this.MainEllipse.Height / 2.0;

            if (newPosition.X < halfWidth)
            {
                x = -1 - (-1 * newPosition.X / halfWidth);
            }
            else
            {
                x = ((newPosition.X - halfWidth) / halfWidth);
            }

            double y = 0;

            if (newPosition.Y < halfHeight)
            {
                y = 1 - (newPosition.Y / halfHeight);
            }
            else
            {
                y = -1 * (newPosition.Y - halfHeight) / halfHeight;
            }

            double startX = 0;
            double startY = 1;

            double newAngle = Math.Atan2(startY, startX) - Math.Atan2(y, x);

            newAngle = newAngle * this.MaxAngle / (2 * Math.PI);

            newAngle += this.previousAngle;

            if (newAngle < 0)
            {
                newAngle = 0;
            }

            if ((Math.Abs(this.previousAngle - newAngle) < 0) && (newAngle >= this.MaxAngle))
            {
                return;
            }

            if (newAngle > this.MaxAngle)
            {
                newAngle = this.MaxAngle;
            }

            if ((Math.Abs(this.previousAngle) < 0) && (Math.Abs(this.Angle - this.MaxAngle) < 0))
            {
                newAngle = this.MaxAngle;
            }

            double multiplier = (newAngle / (this.MaxAngle - this.MinAngle));

            double newValue = this.MinValue + (multiplier * (this.MaxValue - this.MinValue));

            double previousValue = this.Value;

            if (Math.Abs(newValue - previousValue) >= 25)
            {
                newValue = previousValue + 1;

                if (newValue > 100)
                {
                    newValue -= 2;
                }

                this.Value = newValue;

                CircleSliderBase.ValueChange(this, new DependencyPropertyChangedEventArgs(ValueProperty, previousValue, newValue));

                return;
            }

            this.Value = newValue;
            //this.Value = Math.Round(newValue, 0);

            multiplier = (this.Value / (this.MaxValue - this.MinValue));

            newAngle = this.MinAngle + (multiplier * (this.MaxAngle - this.MinAngle));

            this.Angle = newAngle;

            var renderTransform = new RotateTransform(newAngle, this.centerPosition.X, this.centerPosition.Y);

            this.MainGrid.RenderTransform = renderTransform;
        }

        public void OnTextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender is not UIElement textBox)
                {
                    return;
                }

                var binding = BindingOperations.GetBindingExpression(textBox, TextBox.TextProperty);

                if (binding != null)
                {
                    binding.UpdateSource();
                }

                e.Handled = true;
            }
        }
    }
}
