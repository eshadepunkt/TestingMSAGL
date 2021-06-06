using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Msagl.Core.Geometry;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using TestingMSAGL.DataLinker;
using TestingMSAGL.DataStructure;
using TestingMSAGL.DataStructure.RoutedOperation;
using Color = Microsoft.Msagl.Drawing.Color;
using Point = Microsoft.Msagl.Core.Geometry.Point;
using Single = TestingMSAGL.DataStructure.RoutedOperation.Single;

namespace TestingMSAGL
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GraphViewer _graphViewer;
        private Point _mMouseLeftButtonDownPoint;
        private Point _mMouseRightButtonDownPoint;
        private Node _nodeUnderCursor;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += InitGraph;
            _graphViewer = new GraphViewer();
            ViewerPanel.ClipToBounds = true;
            ViewerPanel.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#4d22ff");
            _graphViewer.GraphCanvas.Height = ViewerPanel.Height;
            _graphViewer.GraphCanvas.Width = ViewerPanel.Width;
            _graphViewer.GraphCanvas.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
            _graphViewer.ObjectUnderMouseCursorChanged += graphViewer_ObjectUnderMouseCursorChanged;
                _graphViewer.BindToPanel(ViewerPanel);

            
            _graphViewer.LayoutEditingEnabled = true;
            //todo graphViewer props for pane moving
            NodeTreeView();
        }

        private void graphViewer_ObjectUnderMouseCursorChanged(object sender, ObjectUnderMouseCursorChangedEventArgs e)
        {
            var node = _graphViewer.ObjectUnderMouseCursor as IViewerNode;
            if (node != null)
            {
                _nodeUnderCursor = (Node) node.DrawingObject;
                statusTextBox.Text = _nodeUnderCursor.Label.Text;
            }
        }
        private GraphExtension Graph { get; } = new("root", "0");
        private int NodeCounter { get; set; }


        private void NodeTreeView()
        {
            var root = new MenuItem {Title = "Root"};


            var childItem1 = new MenuItem {Title = "Child item #1"};
            childItem1.Items.Add(new MenuItem {Title = "Child item #1.1"});
            childItem1.Items.Add(new MenuItem {Title = "Child item #1.2"});
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem {Title = "Child item #2"});
            NodeTree.Items.Add(root);
        }

        /// <summary>
        ///     Static graph creating for testing purposes only.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitGraph(object sender, RoutedEventArgs e)
        {
            // Drawing Board
            var rootSubgraph = new Subgraph("rootSubgraph");


            // first element 
            var root = new NodeComplex(Graph, "Root");
            rootSubgraph.AddSubgraph(root.Subgraph);
            

            var ros = new Alternative(Graph, "Root Of Subgraph");
            ros.Composite.Predecessor = root.Composite;

            root.Composite.Successor = ros.Composite;
            root.AddMember(ros);

            var c1 = new Single(Graph, "Complex: 104");
            var c2 = new Parallel(Graph, "Complex: 105");
            //NodeComplex c3 = new Alternative(Graph, "Complex: 106");
            ros.AddMember(c1); // layout Problem. - does not contain any nodes - that's why :)
            ros.AddMember(c2);
            ros.Composite.Successor = c2.Composite;
            c2.Composite.Predecessor = ros.Composite;
            //ros.AddMember(c3);
            //c3.Composite.Predecessor = c2.Composite;
            //c2.Composite.Successor = c3.Composite;


            ////
            var e1 = new NodeElementary(Graph, "I am groot!");
            var e2 = new NodeElementary(Graph, "We are groot!");
            var e3 = new NodeElementary(Graph, "We are not groot!");
            c2.AddMember(e1);
            c2.AddMember(e2);
            c2.AddMember(e3);
            var c4 = new Fixed(Graph, "heya");
            //var c5 = new Fixed(Graph, "fix it!");
            //var c6 = new Parallel(Graph, "fix it!");

            //c4.Composite.Successor = c6.Composite;
            //c6.Composite.Predecessor = c4.Composite;
            c2.AddMember(c4);
            //c2.AddMember(c5);
            //c2.AddMember(c6);
            //var e4 = new NodeElementary(Graph, "I am Inevitable!");
            //var e5 = new NodeElementary(Graph, "I am Ironman!");
            //c3.AddMember(e4);  
            //c3.AddMember(e5);

            // if(c3 is Alternative alternative)
            Console.WriteLine("Test");
            var composites = root.Composite.Members;

            Graph.RootSubgraph = rootSubgraph;


            NodeCounter = Graph.NodeCount;
            Graph.Attr.Margin = 5;
            Graph.Attr.OptimizeLabelPositions = true;
            Graph.Attr.MinNodeHeight = 10;
            Graph.Attr.MinNodeWidth = 10;
            Graph.Attr.MinimalWidth = 10;
            Graph.Attr.MinimalHeight = 10;
            Graph.Attr.BackgroundColor = Color.Orange;
            Graph.Attr.ClearStyles();
            _graphViewer.Graph = Graph;
            _graphViewer.RunLayoutAsync = true;
        }


        /// <summary>
        ///     Method to create a new node in given Subgraph/Complex
        /// </summary>
        private void InsertNode()
        {
            // Possible to Insert into more then one ?
            try
            {
                var forDragging = _graphViewer.Entities
                    .SingleOrDefault(x => x.MarkedForDragging);
                if (forDragging == null) return;

                var subgraph = ((IViewerNode) forDragging).Node;
                subgraph.Attr.LineWidth = 1;
                if (subgraph is Subgraph)
                {
                    var node = new NodeElementary(Graph, "New Node");
                    var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                    //todo check if member exists?
                    if (!nodeComplex.AddMember(node)) MessageBox.Show("Could not add to member list");
                    //todo add method to detect if children are already present, sort new node as successor of last child
                    //node.Composite.Predecessor = nodeComplex.Composite;
                    //nodeComplex.Composite.Successor = node.Composite;
                    //todo fix issues
                    //_graphViewer.Graph = Graph;
                }
            }

            // todo repair exception handling and message

            catch (ArgumentNullException e)
            {
                MessageBox.Show("No complex selected!" + e);
                // throw;
            }
            catch (InvalidOperationException invalid)
            {
                MessageBox.Show("More than one complex selected!" + invalid);
            }
        } // todo buggy as hell
        //todo probably needs to be extracted to NodeComplex / Elementary 

        /// <summary>
        ///     Takes any type of a complex and merges all selected elementaries or complex into it
        /// </summary>
        /// <param name="newComplex"></param>
        private void ConvertGroupOfElementariesToComplex(NodeComplex newComplex)
        {
            try
            {
                var forDragging = _graphViewer.Entities
                    .Where(x => x.MarkedForDragging)
                    .Cast<IViewerNode>()
                    .ToList();
                if (forDragging.Count < 1) return;

                foreach (var viewerNode in forDragging)
                {
                    //workaround for select issue
                    viewerNode.Node.Attr.LineWidth = 1;
                    if (viewerNode.Node is Subgraph subgraph)
                    {
                        //todo rework this check, because newComplex is already instantiated but whether deleted nor used
                        if (subgraph.ParentSubgraph.ParentSubgraph == null)
                        {
                            MessageBox.Show("Error Root can't be part of a Group!");
                            return;
                        }

                        var parent = Graph.GetComplexNodeById(subgraph.ParentSubgraph.Id);
                        var child = Graph.GetNodeById(subgraph.Id);
                        parent.AddMember(newComplex);
                        parent.RemoveMember(child);
                        newComplex.AddMember(child);
                    }
                    else
                    {
                        var child = Graph.GetNodeById(viewerNode.Node.Id);
                        var parent = Graph.GetComplexNodeById(child.ParentId);
                        parent.RemoveMember(child);
                        parent.Subgraph.RemoveNode(viewerNode.Node);
                        parent.AddMember(newComplex);
                        newComplex.AddMember(child);
                    }
                }

                _graphViewer.Graph = Graph;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in GroupNodes()\n" + e);
                throw;
            }
        }

        private void AddEdge()
        {
            //todo predecessor successor

            var forDragging = _graphViewer.Entities
                .Where(x => x.MarkedForDragging)
                .Cast<IViewerNode>()
                .ToArray();
            if (forDragging.Length < 1) return;

            var nodes = new List<NodeComplex>();
            foreach (var node in forDragging)
            {
                nodes.Add(Graph.GetComplexNodeById(node.Node.Id));
                node.Node.Attr.LineWidth = 1;
            }

            foreach (var composite in nodes)
                if (composite?.Composite.Successor != null)
                {
                    Graph.AddEdge(
                        composite.NodeId,
                        composite.Composite.Successor.DrawingNodeId
                    );
                    // adds nodes side-by-side (same layer left -> right)
                    Graph.LayerConstraints.AddSameLayerNeighbors(
                        Graph.FindNode(composite.Composite.DrawingNodeId),
                        Graph.FindNode(composite.Composite.Successor.DrawingNodeId)
                    );
                }
            _graphViewer.Graph = Graph;
            //todo magic for constraints?!
        }

        /// <summary>
        ///     Testing purpose only
        ///     Creates 10 Nodes, if a complex is marked.
        /// </summary>
        private void CreateTenNodesForTesting()
        {
            if (!_graphViewer.Entities.Any(x => x.MarkedForDragging)) return;

            for (var i = 0; i < int.Parse(CounterText.Text); i++) InsertNode();

            _graphViewer.Graph = Graph;
        }

        private void InsertSubgraph(IWithId complex, Node subgraph)
        {
            // Possible to Insert into more then one ?
            try
            {
                subgraph.Attr.LineWidth = 1;
                var nodeComplex = Graph.GetComplexNodeById(subgraph.Id);
                nodeComplex.AddMember(complex);

                _graphViewer.Graph = Graph;
            }
            catch (Exception e)
            {
                MessageBox.Show("More than one complex selected! " + e);
            }
            // todo can't handle clicked elementary nodes
        } // todo buggy as hell

        /// <summary>
        ///     Checks for any selected node (subgraph) and deletes the node (subgraph and all children)
        /// </summary>
        private void DeleteNode()
        {
            try
            {
                var forDragging = _graphViewer.Entities
                    .Where(x => x.MarkedForDragging)
                    .Cast<IViewerNode>()
                    .ToList();
                if (forDragging.Count < 1) return;

                foreach (var viewerNode in forDragging)
                    //todo extract as method to Graph Extension
                    if (viewerNode.Node is Subgraph subgraph)
                    {
                        if (subgraph.ParentSubgraph.ParentSubgraph == null)
                        {
                            MessageBox.Show("Error: Root can't be deleted!");
                            return;
                        }

                        //check for Children
                        if (subgraph.Subgraphs.Any())
                        {
                            var allChildren = subgraph.AllSubgraphsDepthFirst().ToList();
                            foreach (var child in allChildren) Graph.DeleteRecursive(child);
                        }
                        else
                        {
                            var allNodes = subgraph.Nodes.ToList();
                            foreach (var node in allNodes) Graph.RemoveNode(node);

                            subgraph.ParentSubgraph.RemoveSubgraph(subgraph);
                            Graph.DeleteById(subgraph.Id);
                            Graph.RemoveNode(subgraph);
                        }
                    }
                    else
                    {
                        //todo extract as method to Graph Extension
                        var childComposite = Graph.GetNodeById(viewerNode.Node.Id);
                        var parentComposite = Graph.GetComplexNodeById(childComposite.ParentId);
                        parentComposite.RemoveMember(childComposite);
                        // removes nodes from (root)subgraph into Graph
                        parentComposite.Subgraph.RemoveNode(viewerNode.Node);
                        // removes straying nodes from Graph
                        Graph.RemoveNode(viewerNode.Node);
                    }

                _graphViewer.Graph = Graph;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error in Delete()\n" + e);
                //throw;
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
            CreateTenNodesForTesting();
        }

        private void DeleteSelectedNode_Click(object sender, RoutedEventArgs e)
        {
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
            _mMouseRightButtonDownPoint = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
        }


        private void ResetGraph_Click(object sender, RoutedEventArgs e)
        {
        }

        private void EdgeSelectedNode_Click(object sender, RoutedEventArgs e)
        {
            AddEdge();
        }

        private void EditMode_Click(object sender, RoutedEventArgs e)
        {
            var defaultColor = EditMode.Background;
            if (_graphViewer.InsertingEdge)
            {
                _graphViewer.InsertingEdge = false;
                EditMode.Background = defaultColor;
            }
            else
            {
                _graphViewer.InsertingEdge = true;
                EditMode.Background = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
            }
        }

        private void InsertNodeCM_Click(object sender, RoutedEventArgs e)
        {
            InsertNode();
            _graphViewer.Graph = Graph;
        }
          
        private void ViewerPanel_OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // todo get this to work!
            var p0 = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
            var p1 = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                p0.X = e.GetPosition(this).X;
                p0.Y = e.GetPosition(this).Y;
            }

            if (e.LeftButton == MouseButtonState.Released)
            {
                p1.X = e.GetPosition(this).X;
                p1.Y = e.GetPosition(this).Y;
            }

            var rubberRec = new Rectangle(p0, p1);
        }


        private void AddAlternativMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var forDragging = _graphViewer.Entities
                .Single(x => x.MarkedForDragging);
            var subgraph = ((IViewerNode) forDragging).Node;
            if (subgraph is Subgraph)
                InsertSubgraph(new Alternative(Graph, "New Alternative"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddParallelMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var forDragging = _graphViewer.Entities
                .Single(x => x.MarkedForDragging);
            var subgraph = ((IViewerNode) forDragging).Node;
            if (subgraph is Subgraph)
                InsertSubgraph(new Parallel(Graph, "New Parallel"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddFixedMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var forDragging = _graphViewer.Entities
                .Single(x => x.MarkedForDragging);
            var subgraph = ((IViewerNode) forDragging).Node;
            if (subgraph is Subgraph)
                InsertSubgraph(new Fixed(Graph, "New Fixed"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void AddSingleMenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            var forDragging = _graphViewer.Entities
                .Single(x => x.MarkedForDragging);
            var subgraph = ((IViewerNode) forDragging).Node;
            if (subgraph is Subgraph)
                InsertSubgraph(new Single(Graph, "New Single"), subgraph);
            else
                MessageBox.Show("Error: You tried to insert into an elementary.");
        }

        private void GroupAlternative_OnClick(object sender, RoutedEventArgs e)
        {
            ConvertGroupOfElementariesToComplex(new Alternative(Graph, "New Group Alternative"));
        }

        private void GroupParallel_OnClick(object sender, RoutedEventArgs e)
        {
            ConvertGroupOfElementariesToComplex(new Parallel(Graph, "New Group Parallel"));
        }

        private void GroupFixed_OnClick(object sender, RoutedEventArgs e)
        {
            ConvertGroupOfElementariesToComplex(new Fixed(Graph, "New Group Fixed"));
        }

        private void GroupSingle_OnClick(object sender, RoutedEventArgs e)
        {
            ConvertGroupOfElementariesToComplex(new Single(Graph, "New Group Single"));
        }

        private void DeleteNodeCM_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteNode();
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



        private void Alternative_OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (e.LeftButton is MouseButtonState.Pressed)
            {
                var data = new DataObject();
                data.SetData(DataFormats.StringFormat, Alternative.Name);

                DragDrop.DoDragDrop(this, data, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

    
        private void ViewerPanel_OnDrop(object sender, DragEventArgs e)
        {
            base.OnDrop(e);
            
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                var dataString = e.Data.GetData(DataFormats.StringFormat) as string;

                var alternative = new Alternative(Graph, dataString);

                if (_nodeUnderCursor != null)
                {
                    var markedNode = _graphViewer.Entities.Cast<IViewerNode>().Single(x => x.DrawingObject.Equals(_nodeUnderCursor));
                    markedNode.MarkedForDragging = true;
                    InsertSubgraph(alternative, _nodeUnderCursor);
                }
            }
            e.Handled = true;
        }
    }
}