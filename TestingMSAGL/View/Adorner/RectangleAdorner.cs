using System.Windows;
using System.Windows.Media;

namespace TestingMSAGL.View.Adorner
{
    public class RectangleAdorner : System.Windows.Documents.Adorner
    {
        public RectangleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);

            SolidColorBrush renderBrush = new(Colors.Green);
            renderBrush.Opacity = 0.2;
            Pen renderPen = new(new SolidColorBrush(Colors.Navy), 1.5);
            //var renderRadius = 5.0;

            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
        }
    }
}