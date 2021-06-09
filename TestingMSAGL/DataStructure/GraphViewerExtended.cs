using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;

namespace TestingMSAGL.DataStructure
{
    /// <summary>
    /// used to implement a proper hit detection while dragging
    /// </summary>
    public class GraphViewerExtended : GraphViewer
    {
        private object _objectUnderMouseCursor;
        private Point _objectUnderMouseDetectionLocation;
        
        public IViewerObject GetObjectUnderMouseCursorWhileDragging(Point position )
        {
            if (!(_objectUnderMouseDetectionLocation == position))
                UpdateWithWpfHitObjectUnderDragOverLocation(position, MyHitTestResultCallBackWithNoCallbacksToUser);
            return GetIViewerObjectFromObjectUnderCursor(_objectUnderMouseCursor);
        }

        private HitTestResultBehavior MyHitTestResultCallBackWithNoCallbacksToUser(HitTestResult result)
        {
            if (result.VisualHit is not FrameworkElement visualHit)
                return HitTestResultBehavior.Continue;
            var tag = visualHit.Tag;
            if (tag is IViewerObject viewerObject)
            {
                if (!viewerObject.DrawingObject.IsVisible) return HitTestResultBehavior.Continue;
                
                _objectUnderMouseCursor = viewerObject;
                switch (tag)
                {
                    case VNode _:
                    case Label _:
                        return HitTestResultBehavior.Stop;
                }
            }
            else
            {
                _objectUnderMouseCursor = tag;
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }

        private void UpdateWithWpfHitObjectUnderDragOverLocation(Point position, HitTestResultCallback hitTestResultCallback)
        {
            _objectUnderMouseDetectionLocation = position;
            var rectangleGeometry =
                new RectangleGeometry(
                    new Rect(
                        new Point(position.X - MouseHitTolerance, position.Y - MouseHitTolerance),
                        new Point(position.X + MouseHitTolerance, position.Y + MouseHitTolerance)
                        )
                    );
            VisualTreeHelper.HitTest(
                GraphCanvas, 
                null, 
                hitTestResultCallback,
                new GeometryHitTestParameters(rectangleGeometry)
                );
        }

        private IViewerObject GetIViewerObjectFromObjectUnderCursor(object obj) => obj as IViewerObject;
    }
}