using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ComplexEditor.View.Adorner
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
            if (_adornedElement is not Border border) return;
            var adornedElementRect = new Rect(AdornedElement.DesiredSize);
            var textBlockOfAdornedElement = border.Child as TextBlock;
            var textBlock = new TextBlock
            {
                //todo find a better way to display the text
                Text = textBlockOfAdornedElement?.Text,
                IsHitTestVisible = false,
                FontSize = 24
            };

            var renderBrush = border.Background.Clone();
            renderBrush.Opacity = 0.5;
            Pen renderPen = new(new SolidColorBrush(Colors.Black), 1.5);

            var borderForTextBlockAndBrush = new Border
            {
                Background = renderBrush,
                RenderSize = AdornedElement.DesiredSize,
                Child = textBlock
            };

            BitmapCacheBrush bcb = new(borderForTextBlockAndBrush);
            drawingContext.DrawRoundedRectangle(bcb, renderPen, adornedElementRect, 3, 3);

            //var renderRadius = 5.0;
        }
    }
}