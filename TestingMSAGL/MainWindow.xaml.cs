using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Msagl.Drawing;
using Microsoft.Win32;
using TestingMSAGL.ComplexEditor;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure.RoutedOperation;
using Single = TestingMSAGL.DataStructure.RoutedOperation.Single;

namespace TestingMSAGL
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point _mMouseLeftButtonDownPoint;
        private Point _mMouseRightButtonDownPoint;
        private IViewerNode _nodeUnderCursor;
        private Brush _oldShapeFill = Brushes.Transparent;


        public MainWindow()
        {
            InitializeComponent();
            Loaded += Editor.initGraph;
            Editor.GraphViewer.GraphCanvas.Height = ViewerPanel.Height;
            Editor.GraphViewer.GraphCanvas.Width = ViewerPanel.Width;
            Editor.GraphViewer.GraphCanvas.Background =
                (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
            Editor.GraphViewer.ObjectUnderMouseCursorChanged += graphViewer_ObjectUnderMouseCursorChanged;
            Editor.GraphViewer.BindToPanel(ViewerPanel);
            var test = Editor.GraphViewer.GraphCanvas.Children;
            Editor.GraphViewer.GraphCanvas.IsEnabledChanged += GraphCanvasOnIsEnabledChanged;
            ViewerPanel.ClipToBounds = true;
            //Editor.GraphViewer.GraphCanvas.MouseMove += GraphCanvasOnMouseMove;
            // Editor.GraphViewer.GraphCanvas.MouseDown += GraphCanvasOnMouseDown;
            ViewerPanel.Initialized += ViewerPanelOnInitialized;
            ViewerPanel.Loaded += ViewerPanelOnLoaded;
            Editor.GraphViewer.GraphCanvas.Initialized += GraphCanvasOnInitialized;
            Editor.GraphViewer.GraphCanvas.Loaded += GraphCanvasOnLoaded;
            Editor.GraphViewer.LayoutComplete += GraphViewerOnLayoutComplete;

            //todo graphViewer props for pane moving
            //NodeTreeView();
        }

        private Editor Editor { get; } = new();

        private void GraphViewerOnLayoutComplete(object sender, EventArgs e)
        {
            dynamic test = ViewerPanel.Children[0];
            UIElementCollection children = test.Children;
            foreach (var child in children)
                if (child is Path path)
                {
                    path.AllowDrop = true;
                    path.DragEnter += (o, args) =>
                    {
                        if (child is Path path)
                        {
                            _oldShapeFill = path.Fill.Clone();
                            path.Fill = Brushes.CadetBlue;
                        }
                    };
                    path.DragLeave += (o, args) =>
                    {
                        if (child is Path path) path.Fill = _oldShapeFill;
                    };
                }
        }

        private void GraphCanvasOnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void GraphCanvasOnInitialized(object sender, EventArgs e)
        {
        }

        private void ViewerPanelOnLoaded(object sender, RoutedEventArgs e)
        {
        }

        private void ViewerPanelOnInitialized(object sender, EventArgs e)
        {
        }

        private void GraphCanvasOnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
        }


        private void graphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            if (Editor.GraphViewer.ObjectUnderMouseCursor is IViewerNode node)
            {
                _nodeUnderCursor = node;
                statusTextBox.Text = _nodeUnderCursor.Node.Label.Text;
            }
        }


        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Open_OnClick(object sender, RoutedEventArgs e)
        {
            var openFileDlg = new OpenFileDialog();

            // Launch OpenFileDialog by calling ShowDialog method
            openFileDlg.ShowDialog();
            // Get the selected file name and display in a TextBox.
            // Load content of file in a TextBlock
        }


        private void Hexa_button_Click(object sender, RoutedEventArgs e)
        {
            var count = int.Parse(CounterText.Text);

            Editor.CreateTenNodesForTesting(count);
            if (count > 0) Editor.refreshLayout();
        }

        private void cm_Opened(object sender, RoutedEventArgs e)
        {
        }

        private void cm_Closed(object sender, RoutedEventArgs e)
        {
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLabel.Content = e.GetPosition(this).ToString();
        }

        private void cm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mMouseRightButtonDownPoint = e.GetPosition(this);
        }


        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EdgeSelectedNode_Click(object sender, RoutedEventArgs e)
        {
            Editor.AddEdge();
        }

        // todo fix color issues of button
        private void EditMode_Click(object sender, RoutedEventArgs e)
        {
            var defaultColor = EditMode.Background.CloneCurrentValue();
            if (Editor.GraphViewer.InsertingEdge)
            {
                Editor.GraphViewer.InsertingEdge = false;
                EditMode.Background = null;
                EditMode.Background = (SolidColorBrush) defaultColor;
            }
            else
            {
                Editor.GraphViewer.InsertingEdge = true;
                EditMode.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
            }
        }

        private void InsertNodeCM_Click(object sender, RoutedEventArgs e)
        {
            Editor.InsertNode();
            Editor.refreshLayout();
        }


        private void AddAlternativMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode) Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Alternative(Editor.Graph, "New Alternative"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddParallelMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode) Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Parallel(Editor.Graph, "New Parallel"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddFixedMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode) Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Fixed(Editor.Graph, "New Fixed"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddSingleMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode) Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Single(Editor.Graph, "New Single"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void GroupAlternative_OnClick(object sender, RoutedEventArgs e)
        {
            Editor.ConvertGroupOfElementariesToComplex(new Alternative(Editor.Graph, "New Group Alternative"));
        }

        private void GroupParallel_OnClick(object sender, RoutedEventArgs e)
        {
            Editor.ConvertGroupOfElementariesToComplex(new Parallel(Editor.Graph, "New Group Parallel"));
        }

        private void GroupFixed_OnClick(object sender, RoutedEventArgs e)
        {
            Editor.ConvertGroupOfElementariesToComplex(new Fixed(Editor.Graph, "New Group Fixed"));
        }

        private void GroupSingle_OnClick(object sender, RoutedEventArgs e)
        {
            Editor.ConvertGroupOfElementariesToComplex(new Single(Editor.Graph, "New Group Single"));
        }

        private void DeleteNodeCM_OnClick(object sender, RoutedEventArgs e)
        {
            Editor.DeleteNode();
        }


        private void Alternative_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Alternative.Name);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }

        private void Fixed_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Fixed.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void Parallel_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Parallel.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void Single_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Single.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void Node_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            graphViewer_ObjectUnderMouseCursorChanged(sender, null);
            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Node.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void GraphViewer_MouseUp(object sender, EventArgs e)
        {
        }


        /// <summary>
        ///     holds logic for type switching
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewerPanel_OnDrop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);

            //todo fix work around for _nodeUnderCursor being null and crashing the app
            if (e.Data.GetDataPresent(DataFormats.StringFormat) && _nodeUnderCursor != null)
            {
                NodeComplex createdComplex = null;
                var dataString = e.Data.GetData(DataFormats.StringFormat) as string;
                var hit = e.Source as Path;
                if (hit?.Tag is not IViewerObject markedNode) return;
                markedNode.MarkedForDragging = true;
                var subgraph = markedNode as IViewerNode;


                //todo refactoring 
                if (dataString != null)
                {
                    if (dataString.Contains("Alternative"))
                        createdComplex = new Alternative(Editor.Graph, dataString);
                    else if (dataString.Contains("Fixed"))
                        createdComplex = new Fixed(Editor.Graph, dataString);
                    else if (dataString.Contains("Parallel"))
                        createdComplex = new Parallel(Editor.Graph, dataString);
                    else if (dataString.Contains("Single"))
                        createdComplex = new Single(Editor.Graph, dataString);
                    else if (dataString.Contains("Node"))
                        if (_nodeUnderCursor != null)
                        {
                            Editor.InsertNode();
                            Editor.GraphViewer.Graph = Editor.Graph;
                        }

                    if (_nodeUnderCursor != null && createdComplex != null)
                        Editor.InsertSubgraph(createdComplex, subgraph);
                    markedNode.MarkedForDragging = false;

                    Editor.GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(markedNode);
                }
            }

            e.Handled = true;
        }


        private void GraphCanvasOnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                _mMouseLeftButtonDownPoint = e.GetPosition(Editor.GraphViewer.GraphCanvas);
        }

        private void GraphCanvasOnMouseMove(object sender, MouseEventArgs e)
        {
        }

        public class MenuItem
        {
            public MenuItem()
            {
                Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }
    }
}