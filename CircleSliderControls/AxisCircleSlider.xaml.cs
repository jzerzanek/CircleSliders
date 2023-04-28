using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircleSliderControls
{
    /// <summary>
    /// Interaction logic for AxisCircleSlider.xaml
    /// </summary>
    public partial class AxisCircleSlider : CircleSliderBase
    {
        protected override Ellipse MainEllipse => this.mainEllipse;
        protected override Grid MainGrid => this.grid;

        public AxisCircleSlider()
        {
            InitializeComponent();
        }
    }
}
