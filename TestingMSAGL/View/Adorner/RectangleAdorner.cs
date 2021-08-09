using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TestingMSAGL.View.Adorner
{
    public class RectangleAdorner : System.Windows.Documents.Adorner
    {
        private readonly UIElement _adornedElement;

        public RectangleAdorner(UIElement adornedElement)
            : base(adornedElement)
        {
            _adornedElement = adornedElement;
            IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);
            if (_adornedElement is not Border border) return;
            var renderBrush = border.Background.Clone();
            renderBrush.Opacity = 0.5;
            Pen renderPen = new(new SolidColorBrush(Colors.Black), 1.5);
            drawingContext.DrawRectangle(renderBrush, renderPen, adornedElementRect);
            //var renderRadius = 5.0;
        }
    }
}