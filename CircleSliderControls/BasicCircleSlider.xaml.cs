using System.Windows.Controls;
using System.Windows.Shapes;

namespace CircleSliderControls
{
    /// <summary>
    /// Interaction logic for BasicCircleSlider.xaml
    /// </summary>
    public partial class BasicCircleSlider : CircleSliderBase
    {
        protected override Ellipse MainEllipse => this.mainEllipse;
        protected override Grid MainGrid => this.grid;

        public BasicCircleSlider()
        {
            InitializeComponent();
        }
    }
}
