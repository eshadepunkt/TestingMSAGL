using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using TestingMSAGL.ComplexEditor;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure.RoutedOperation;
using TestingMSAGL.DataStructure.XmlProvider;
using TestingMSAGL.View.Adorner;
using Panel = System.Windows.Controls.Panel;
using Parallel = TestingMSAGL.DataStructure.RoutedOperation.Parallel;
using Single = TestingMSAGL.DataStructure.RoutedOperation.Single;

namespace TestingMSAGL
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private Point _mMouseLeftButtonDownPoint;
        private Point _mMouseRightButtonDownPoint;
        private IViewerNode _nodeUnderCursor;
        private Brush _oldShapeFill = Brushes.Transparent;
        private AdornerLayer _adornerLayer;
        private Adorner _adorner;


        public MainWindow()
        {
            InitializeComponent();
            Loaded += Editor.initGraph;
            Editor.GraphViewer.GraphCanvas.Height = ViewerPanel.Height;
            Editor.GraphViewer.GraphCanvas.Width = ViewerPanel.Width;
            //Editor.GraphViewer.GraphCanvas.Background =
            //    (SolidColorBrush)new BrushConverter().ConvertFromString("#4dd2ff");
            Editor.GraphViewer.ObjectUnderMouseCursorChanged += graphViewer_ObjectUnderMouseCursorChanged;
            Editor.GraphViewer.BindToPanel(ViewerPanel);
            ViewerPanel.ClipToBounds = true;
            Editor.GraphViewer.LayoutComplete += GraphViewerOnLayoutComplete;

        }

        

        /// <summary>
        /// creates all operations found in a given XML
        /// displayed as a border with a text block inside
        /// </summary>
        /// <param name="operations"></param>
        private void OperationsLoad(IEnumerable<NamedOperation> operations)
        {
            foreach (var operation in operations)
            {
                var border = new Border();
                border.MouseMove += Node_OnMouseMove;
                border.Width = 100;
                border.Height = 50;
                border.Margin = new Thickness(5, 5, 0, 10);
                border.BorderBrush = new SolidColorBrush(Colors.Black);
                border.BorderThickness = new Thickness(2);
                border.Background = new SolidColorBrush(Colors.White);
                border.CornerRadius = new CornerRadius(10);
                border.Padding = new Thickness(5);
                border.ClipToBounds = true;

                var textBlock = new TextBlock
                {
                    Text = operation.Name,
                    Margin = new Thickness(10),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    FontSize = 8,
                    ClipToBounds = true
                };

                border.Child = textBlock;
                compositePanel.Children.Add(border);
            }

            var separator = new Separator
            {
                BorderBrush = (Brush)new BrushConverter().ConvertFromString("#ccf2ff"),
                BorderThickness = new Thickness(5),
                Height = 5
            };
            compositePanel.Children.Add(separator);

            
        }


        private void CreateAdornerForAllComposites(Panel stackPanel)
        {
            _adornerLayer = AdornerLayer.GetAdornerLayer(stackPanel);
            foreach (UIElement toAdorn in stackPanel.Children)
                if (toAdorn is Border)
                {
                    _adorner = new RectangleAdorner(toAdorn);
                    _adorner.Visibility = Visibility.Collapsed;
                    _adornerLayer?.Add(_adorner);
                }
        }

        private Editor Editor { get; } = new();

        private void GraphViewerOnLayoutComplete(object sender, EventArgs e)
        {
            CreateAdornerForAllComposites(compositePanel);
            dynamic test = ViewerPanel.Children[0];
            UIElementCollection children = test.Children;
            foreach (var child in children)
            {
                if (child is TextBlock textBlock)
                {
                }

                if (child is not Path { Tag: VNode { DrawingObject: Subgraph } } path) continue;
                {
                    var tag = child as Path;
                    var vnode = tag.Tag as Cluster;
                    if (vnode != null) vnode.IsCollapsed = false;
                    path.AllowDrop = true;
                    path.DragEnter += (o, args) =>
                    {
                        //if (child is not Path path) return;
                        _oldShapeFill = path.Fill.Clone();
                        path.Fill = Brushes.CadetBlue;
                    };
                    path.DragLeave += (o, args) =>
                    {
                        //if (child is Path path) 
                        path.Fill = _oldShapeFill;
                    };
                }
            }
        }


        private void graphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            if (Editor.GraphViewer.ObjectUnderMouseCursor is IViewerNode node)
            {
                _nodeUnderCursor = node;
                if (_nodeUnderCursor.Node.Label != null)
                    statusTextBox.Text = _nodeUnderCursor.Node.Label.Text;
                else
                    statusTextBox.Text = _nodeUnderCursor.Node.Id;
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


        private void CreateNodesButtonClick(object sender, RoutedEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(CounterText.Text.ToString(), "\\d+"))
            {
                MessageBox.Show("Es sind nur ganze Zahlen erlaubt!");
                return;
            }
            var amount = int.Parse(CounterText.Text);

            Editor.CreateAnyAmountOfNodesForTesting(amount);
            if (amount > 0) Editor.refreshLayout();
        }
        private void CounterText_KeyDown(object sender, KeyEventArgs e)
        {
            if (!System.Text.RegularExpressions.Regex.IsMatch(e.Key.ToString(), "\\d+"))
                e.Handled = true;
        }


        private void cm_Opened(object sender, RoutedEventArgs e)
        {
           
            
        }

        private void cm_Closed(object sender, RoutedEventArgs e)
        {
            ClearAllNodeDecorations();
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            MouseLabel.Content = e.GetPosition(this).ToString();
        }

        private void cm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mMouseRightButtonDownPoint = e.GetPosition(this);

            if (Editor.GraphViewer.Entities.Count(entity => entity.MarkedForDragging == true) > 1 && _nodeUnderCursor.MarkedForDragging == false)
                return;
            if (!_nodeUnderCursor.MarkedForDragging)
            {
                ClearAllNodeDecorations();
                Editor.GraphViewer.LayoutEditor.DecorateObjectForDragging(_nodeUnderCursor);
                _nodeUnderCursor.MarkedForDragging = true;
            }
        }


        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
            Editor.DeleteNode();
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
                EditMode.Background = (SolidColorBrush)defaultColor;
            }
            else
            {
                Editor.GraphViewer.InsertingEdge = true;
                EditMode.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#4dd2ff");
            }
        }

        private void InsertNodeCM_Click(object sender, RoutedEventArgs e)
        {
            Editor.InsertNode(null);
            Editor.refreshLayout();
        }


        private void AddAlternativMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode)Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Alternative(Editor.Graph, "New Alternative"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddParallelMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode)Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Parallel(Editor.Graph, "New Parallel"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddFixedMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode)Editor.findOneNodeSelected();
            if (subgraph.Node is Subgraph)
                Editor.InsertSubgraph(new Fixed(Editor.Graph, "New Fixed"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddSingleMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var subgraph = (IViewerNode)Editor.findOneNodeSelected();
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
            var border = sender as Border;
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;
            ClearAllNodeDecorations();
            _adorner = _adornerLayer.GetAdorners(sender as UIElement).FirstOrDefault();


            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, border.Name);
            DragDrop.DoDragDrop(this, data, DragDropEffects.Move);
        }

        private void Fixed_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);


            if (e.LeftButton is not MouseButtonState.Pressed) return;
            ClearAllNodeDecorations();
            _adorner = _adornerLayer.GetAdorners(sender as UIElement).FirstOrDefault();

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Fixed.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void Parallel_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);


            if (e.LeftButton is not MouseButtonState.Pressed) return;
            ClearAllNodeDecorations();
            _adorner = _adornerLayer.GetAdorners(sender as UIElement).FirstOrDefault();

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Parallel.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        private void Single_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            ClearAllNodeDecorations();

            _adorner = _adornerLayer.GetAdorners(sender as UIElement).FirstOrDefault();

            var data = new DataObject();
            data.SetData(DataFormats.StringFormat, Single.Name);

            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }


        private void Node_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.LeftButton is not MouseButtonState.Pressed) return;

            ClearAllNodeDecorations();


            var data = new DataObject();
            var borderTextfield = ((Border)sender).Child;
            if (borderTextfield is TextBlock textBlock)
                data.SetData(DataFormats.StringFormat, textBlock.Text);
            if (sender is UIElement element)
                _adorner =
                    (_adornerLayer.GetAdorners(element) ?? throw new InvalidOperationException()).FirstOrDefault();
            DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
        }

        /// <summary>
        ///     removes the adorner, if a non droppable area is dragged above
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnPreviewDragLeave(object sender, DragEventArgs e)
        {
            _adorner.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        ///     adds the adorner again, if the area is a droppable area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_OnPreviewDragEnter(object sender, DragEventArgs e)
        {
            _adorner.Visibility = Visibility.Visible;
        }

        /// <summary>
        ///     holds logic for clearing the dragging decorations - possible fix for unintentional drag 'n drop into multiple
        ///     complex nodes
        /// </summary>
        private void ClearAllNodeDecorations()
        {
            foreach (var entity in Editor.GraphViewer.Entities)
            {
                entity.MarkedForDragging = false;
                Editor.GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(entity);
            }
        }

        /// <summary>
        ///     holds logic for type switching based on the DoDragDrop Data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ViewerPanel_OnDrop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);

            if (!e.Data.GetDataPresent(DataFormats.StringFormat)) return;

            var dataString = e.Data.GetData(DataFormats.StringFormat) as string;
            var hit = e.Source as Path;

            if (hit?.Tag is not IViewerObject markedNode) return;

            markedNode.MarkedForDragging = true;
            var subgraph = markedNode as IViewerNode;

            if (dataString != null)
            {
                var complex = Editor.GetIWithId(dataString);
                // todo remove legacy code - probably move to factory as well
                if (complex is NodeElementary)
                {
                    Editor.InsertNode(complex);
                    Editor.refreshLayout();
                }
                else
                {
                    Editor.InsertSubgraph(complex, subgraph);
                }

                markedNode.MarkedForDragging = false;
                //Editor.GraphViewer.LayoutEditor.RemoveObjDraggingDecorations(markedNode);
            }

            _adornerLayer.Remove(_adorner);
            e.Handled = true;
        }

        private void MainWindow_OnPreviewDragOver(object sender, DragEventArgs e)
        {
            var location = e.GetPosition(this);
            MouseLabel.Content = location;

            _adorner.RenderTransform = new TranslateTransform(location.X, location.Y);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
            {
                var xdoc = new XmlProvider();
                var operations = xdoc.GetAllXmlElements(openFileDialog.FileName);
                OperationsLoad(operations);
                buildNodePropertyDisplay(operations);
            }
        }
        /// <summary>
        /// hardcoded to main grid
        /// </summary>
        /// <param name="operations"></param>
        private void buildNodePropertyDisplay(IEnumerable<NamedOperation> operations)
        {
            if (operations == null) return;
            ListViewForProps.Visibility = Visibility.Visible;
            operationsElements.Visibility = Visibility.Visible;
            //todo needs generics
            foreach (var operation in operations)
            {
                ListViewForProps.Items.Add("\n" + operation.Id + " : " + operation.Name);
                foreach (var data in operation.dataSet)
                    ListViewForProps.Items.Add("\t" + data.Split(':')[0] +  ": " + data.Split(':')[1]);
            }
            CreateAdornerForAllComposites(compositePanel);
        }

      
    }
}