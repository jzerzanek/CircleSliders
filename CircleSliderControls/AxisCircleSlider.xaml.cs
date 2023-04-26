using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace CircleSliderControls
{
    /// <summary>
    /// Interaction logic for AxisCircleSlider.xaml
    /// </summary>
    public partial class AxisCircleSlider : UserControl
    {
        public static readonly DependencyProperty ValueProperty =
         DependencyProperty.Register("Value", typeof(double), typeof(AxisCircleSlider), new FrameworkPropertyMetadata(0.0, ValueChange)
         {
             BindsTwoWayByDefault = true,
             DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
         });

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(AxisCircleSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(AxisCircleSlider), new PropertyMetadata(0.0));

        private static void ValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not AxisCircleSlider circleSlider)
            {
                return;
            }

            double value = (double)e.NewValue;

            double multiplier = (value / (circleSlider.MaxValue - circleSlider.MinValue));

            circleSlider.Angle = circleSlider.MinAngle + (multiplier * (circleSlider.MaxAngle - circleSlider.MinAngle));

            circleSlider.grid.RenderTransform = new RotateTransform(circleSlider.Angle, circleSlider.centerPosition.X, circleSlider.centerPosition.Y);
        }

        private readonly Point centerPosition;

        private double previousAngle;

        public double Value
        {
            get { return (double)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
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

        public double Angle { set; get; }

        public double MinAngle
        {
            get
            {
                return 0.0;
            }
        }

        public double MaxAngle
        {
            get
            {
                return 360.0;
            }
        }

        public AxisCircleSlider()
        {
            InitializeComponent();

            double halfWidth = this.mainEllipse.Width / 2.0;
            double halfHeight = this.mainEllipse.Height / 2.0;

            this.centerPosition = new Point(halfWidth, halfHeight);
        }

        private void EllipseOnMouseDown(object sender, MouseButtonEventArgs e)
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

        private void EllipseOnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Mouse.Capture(null);
        }

        private void EllipseOnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.Rotate(e.GetPosition(this.mainEllipse));
            }
        }

        private void Rotate(Point newPosition)
        {
            double x = 0;

            double halfWidth = this.mainEllipse.Width / 2.0;
            double halfHeight = this.mainEllipse.Height / 2.0;

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

            double angle = Math.Atan2(startY, startX) - Math.Atan2(y, x);

            angle = angle * this.MaxAngle / (2 * Math.PI);

            angle += this.previousAngle;

            if (angle < 0)
            {
                angle = 0;
            }

            if (angle > this.MaxAngle)
            {
                angle = this.MaxAngle;
            }

            if ((this.previousAngle == 0) && (this.Angle == this.MaxAngle))
            {
                angle = this.MaxAngle;
            }

            var renderTransform = new RotateTransform(angle, this.centerPosition.X, this.centerPosition.Y);

            this.Angle = angle;

            double multiplier = (angle / (this.MaxAngle - this.MinAngle));

            this.Value = this.MinValue + (multiplier * (this.MaxValue - this.MinValue));

            this.previousAngle = angle;
            this.grid.RenderTransform = renderTransform;
        }

        private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
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
