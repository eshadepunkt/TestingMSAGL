using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.WpfGraphControl;
using Microsoft.Win32;
using Color = Microsoft.Msagl.Drawing.Color;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace TestingMSAGL
{
    /// <summary>
    ///     Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GraphViewer _graphViewer = new GraphViewer();
        private Point _mMouseRightButtonDownPoint;
        private Point _mMouseLeftButtonDownPoint;
        
        public MainWindow()
        {
            InitializeComponent();
            Loaded += InitGraph;
            viewerPanel.ClipToBounds = true;
            _graphViewer.BindToPanel(viewerPanel);
            Graph.LayoutAlgorithmSettings.EdgeRoutingSettings.RouteMultiEdgesAsBundles = true;
            _graphViewer.LayoutEditingEnabled = true;
            //todo graphViewer props for pane moving
            _graphViewer.Graph = Graph;
            NodeTreeView();
            
        }

        private void NodeTreeView()
        {
            MenuItem root = new MenuItem() {Title = "Menu"};
            MenuItem childItem1 = new MenuItem() {Title = "Child item #1"};
            childItem1.Items.Add(new MenuItem() {Title = "Child item #1.1"});
            childItem1.Items.Add(new MenuItem() {Title = "Child item #1.2"});
            root.Items.Add(childItem1);
            root.Items.Add(new MenuItem() {Title = "Child item #2"});
            NodeTree.Items.Add(root);
        }

        public class MenuItem
        {
            public MenuItem()
            {
                this.Items = new ObservableCollection<MenuItem>();
            }

            public string Title { get; set; }

            public ObservableCollection<MenuItem> Items { get; set; }
        }

        private Graph Graph { get; } = new Graph("root", "0");
        private int NodeCounter { get; set; }
        private int SubGraphCounter { get; set; }

        private void InitGraph(object sender, RoutedEventArgs e)
        {
            var maxNodesCount = 10;
            for (var i = 1; i < maxNodesCount; i++)
            {
                var curNode = Graph.AddNode("ID: " + i);
                var node = Graph.FindNode(curNode.Id);
                node.Attr.LineWidth = 1;
                node.LabelText = "Label Nr. " + node.Id.Split(':')[1];
                node.Label.FontSize = 5;
                node.Label.FontName = "New Courier";
                node.Label.FontColor = Color.Blue;
                if (i != 1) Graph.AddEdge("ID: " + (i - 1), "ID: " + i);
            }

            UpdateNodeCount();
            Subgraph rootSubgraph = new Subgraph("root of Subgraphs");
            Subgraph roots = new Subgraph("ID: 104");
            Subgraph subgraph1 = new Subgraph("ID: 102");
            Subgraph subgraph2 = new Subgraph("ID: 103");

            ComplexNode rootComplex = new ComplexNode(rootSubgraph);
            ComplexNode complexNode1 = new ComplexNode(roots);
            ComplexNode complexNode2 = new ComplexNode(subgraph1);
            ComplexNode complexNode3 = new ComplexNode(subgraph2);
            
            
            
            complexNode2.Subgraph.AddNode(Graph.FindNode("ID: 3"));
            complexNode2.Subgraph.AddNode(Graph.FindNode("ID: 4"));
            complexNode2.Subgraph.AddNode(Graph.FindNode("ID: 5"));
            
            complexNode3.Subgraph.AddNode(Graph.FindNode("ID: 6"));
            complexNode3.Subgraph.AddNode(Graph.FindNode("ID: 7"));
            complexNode3.Subgraph.AddNode(Graph.FindNode("ID: 8"));
            
            complexNode1.Subgraph.AddSubgraph(complexNode2.Subgraph);
            complexNode1.Subgraph.AddSubgraph(complexNode3.Subgraph);
            
            rootComplex.Subgraph.AddSubgraph(complexNode1.Subgraph);
            // Subgraph2.AddNode(Graph.FindNode("ID: 1")); 
            // Subgraph2.AddNode(Graph.FindNode("ID: 2"));
            // Subgraph2.AddNode(Graph.FindNode("ID: 3"));
            //
            //
            // Subgraph1.AddNode(Graph.FindNode("ID: 4"));
            // Subgraph1.AddNode(Graph.FindNode("ID: 5"));
            // Subgraph1.AddNode(Graph.FindNode("ID: 6"));
            // rootSubgraph.AddSubgraph(roots);
            // roots.AddSubgraph(Subgraph1);
            // roots.AddSubgraph(Subgraph2);
            
            
            Graph.RootSubgraph = rootComplex.Subgraph;
            NodeCounter = Graph.NodeCount;
            _graphViewer.Graph = Graph;
            // var arrayList = new ArrayList();
            // foreach (var obj in _graphViewer.Entities)
            //     arrayList.Add(obj);
            // foreach (IViewerObject obj in arrayList)
            //     if (obj is IViewerNode node)
            //         NodeTree.Items.Add(node.Node.Attr.Id);
            
        }

        private void UpdateNodeCount()
        {
            CounterText.Text = Graph.NodeCount.ToString();
        }

        private string IncrementNodeId()
        {
            NodeCounter++;
            UpdateNodeCount();
            return "ID: " + NodeCounter;
        }
        private string IncrementSubGraphId()
        {
            SubGraphCounter++;
            UpdateNodeCount();
            return ":SUBG: " + SubGraphCounter;
        }

        private string getNodeID_before()
        {
            var preNode = NodeCounter - 1;
            return "ID: " + preNode;
        }

        private void AddNode()
        {
            var curId = NodeCounter;
            Graph.AddNode(IncrementNodeId());
            Graph.AddEdge(getNodeID_before(), curId.ToString());
            _graphViewer.Graph = Graph;
            _graphViewer.GraphCanvas.UpdateLayout();
            UpdateNodeCount();
        }

        private void InsertNode()
        {
            var arrayList = new ArrayList();
            foreach (var obj in _graphViewer.Entities)
                if (obj.MarkedForDragging)
                    arrayList.Add(obj);

            foreach (IViewerObject obj in arrayList)
            {
                var node = obj as IViewerNode;

                if (node?.Node.Id.Split(':')[2] != null)
                    //incrementNodeID();
                    foreach (var subgraph in Graph.RootSubgraph.Subgraphs)
                        if (subgraph.Id.Contains(node.Node.Id))
                        {
                            var curId = IncrementNodeId();
                            Graph.AddNode(curId);
                            subgraph.AddNode(Graph.FindNode(curId));
                            Graph.RemoveNode(Graph.FindNode(curId));
                        }

                _graphViewer.Graph = Graph;
            }
        } // todo buggy as hell

        private void AddNode(Point center)
        {
            var arrayList = new ArrayList();
            foreach (var obj in _graphViewer.Entities)
                if (obj.MarkedForDragging)
                    arrayList.Add(obj);

            foreach (IViewerObject obj in arrayList)
            {
                var node = obj as IViewerNode;
                if (node == null) continue;
                IncrementNodeId();
                node.Node.Attr.LineWidth = 1;
                Graph.AddNode(TagAStringToInt(NodeCounter));
                Graph.AddEdge(node.Node.Id, TagAStringToInt(NodeCounter));
                UpdateNodeCount();
            }

            _graphViewer.Graph = Graph;
        }

        private void AddEdge()
        {
            var arrayList = new ArrayList();
            foreach (var obj in _graphViewer.Entities)
                if (obj.MarkedForDragging)
                    arrayList.Add(obj);

            foreach (IViewerObject obj in arrayList)
            {
                var node = obj as IViewerNode;
                if (node != null)
                {
                    foreach (IViewerObject obj2 in arrayList)
                    {
                        if (obj == obj2)
                            break;
                        var node2 = obj2 as IViewerNode;


                        if (node2 != null && Convert.ToInt32(node.Node.Id.Split(':')[1]) <
                            Convert.ToInt32(node2.Node.Id.Split(':')[1]))
                            Graph.AddEdge(node.Node.Id, node2.Node.Id);
                        else if (node2 != null) Graph.AddEdge(node2.Node.Id, node.Node.Id);
                    }

                    UpdateNodeCount();
                }

                if (node != null) node.Node.Attr.LineWidth = 1;
            }

            _graphViewer.Graph = Graph;
        }

         private void CreateSubgraphFromSelectedNodes()
         {
        Node lowestNode = null;
        Node highestNode = null;
        Subgraph lowestSubgraph = null;
        Subgraph highestSubgraph = null;
        NodeComparer comparer = new NodeComparer();
        var newSubgraph = new Subgraph(IncrementNodeId() + " " + IncrementSubGraphId());
        var arrayList = _graphViewer.Entities.Where(entity => entity.MarkedForDragging);

        var nodes = arrayList.Cast<IViewerNode>();
        
        
        foreach (var obj in arrayList )
        {
        
            var node = obj as IViewerNode;
            if (node == null) break;
        
          int retValMax, retValMin;
          
          if (node.Node.Id.Contains("SUBG123"))
              
          {
              // var subgraph = node;
              // if (lowestSubgraph == null)
              //     lowestSubgraph = subgraph;
              // if (highestSubgraph == null)
              //     highestSubgraph  = subgraph;
              //
              // retValMax = subComparer.Compare(subgraph, highestSubgraph);
              // retValMin = subComparer.Compare(subgraph, lowestSubgraph);
              //
              // if (retValMax == 1)
              //     highestSubgraph = subgraph;
              // if (retValMin == -1)
              //     lowestSubgraph = subgraph;
              // subgraph.Attr.LineWidth = 1;
              // newSubgraph.AddSubgraph(subgraph);
          }
          else
          {
        
              if (lowestNode == null)
                  lowestNode = node.Node;
              if (highestNode == null)
                  highestNode = node.Node;
        
              retValMax = comparer.Compare(node.Node, highestNode);
              retValMin = comparer.Compare(node.Node, lowestNode); 
              if (retValMax == 1)
                  highestNode = node.Node;
              if (retValMin == -1)
                  lowestNode = node.Node;
          
              node.Node.Attr.LineWidth = 1;
              newSubgraph.AddNode(node.Node);
          }
          
        
          
        }
        
        if (!newSubgraph.Nodes.Any() ) return;
        Graph.RootSubgraph.AddSubgraph(newSubgraph);
        
        List<Edge> toBeDeleted = new List<Edge>();
        if (lowestNode != null)
            foreach (var edge in lowestNode.InEdges)
            {
                var parentNodeId = edge.SourceNode.Id;
                toBeDeleted.Add(edge);
                Graph.AddEdge(parentNodeId, newSubgraph.Id);
            }
        
        if (highestNode != null)
            foreach (var edge in highestNode.OutEdges)
            {
                var childNodeId = edge.TargetNode.Id;
                toBeDeleted.Add(edge);
                Graph.AddEdge(newSubgraph.Id, childNodeId);
            }
        
        toBeDeleted.ForEach(edge => {Graph.RemoveEdge(edge);});
        _graphViewer.Graph = Graph;
        }


        private void DeleteNode()
        {
            var arrayList = new ArrayList();
            foreach (var obj in _graphViewer.Entities)
                if (obj.MarkedForDragging)
                    arrayList.Add(obj);
            foreach (IViewerObject obj in arrayList)
            {
                var edge = obj.DrawingObject as IViewerEdge;
                if (edge != null)
                {
                    _graphViewer.RemoveEdge(edge, true);
                }
                else
                {
                    var node = obj as IViewerNode;
                    if (node != null) _graphViewer.RemoveNode(node, true);
                }
            }

            UpdateNodeCount();
            //todo visualize 
        }


        private string TagAStringToInt(int id, string tag = "ID")
        {
            if (tag.Length < 6)
                return tag + ": " + id;
            return "ID: " + id;
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


        private void Rectangle1_OnMouseEnter(object sender, MouseEventArgs e)
        {
            rectangle1.Fill = (SolidColorBrush) new BrushConverter().ConvertFromString("#4dd2ff");
        }

        private void Rectangle1_OnMouseLeave(object sender, MouseEventArgs e)
        {
            rectangle1.Fill = (SolidColorBrush) new BrushConverter().ConvertFromString("#00ace6");
        }

        private void Hexa_button_Click(object sender, RoutedEventArgs e)
        {
            if (Graph == null) return;
            var rnd = new Random(1024);
            var curCount = NodeCounter;
            Graph.AddNode(IncrementNodeId());
            Graph.AddEdge("ID: " + rnd.Next(curCount), "ID: " + NodeCounter);
            UpdateNodeCount();
            _graphViewer.Graph = Graph;
            _graphViewer.GraphCanvas.UpdateLayout();
        }

        private void DeleteSelectedNode_Click(object sender, RoutedEventArgs e)
        {
            DeleteNode();
            UpdateNodeCount();
        }

        private void DeleteNodeCM_Click(object sender, RoutedEventArgs e)
        {
            DeleteNode();
            UpdateNodeCount();
        }

        private void cm_Opened(object sender, RoutedEventArgs e)
        {
        }

        private void cm_Closed(object sender, RoutedEventArgs e)
        {
        }


        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            mouseLabel.Content = e.GetPosition(this).ToString();
        }


        private void AddToSelectedNodeCM_Click(object sender, RoutedEventArgs e)
        {
            AddNode(_mMouseRightButtonDownPoint);
        }

        private void cm_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            _mMouseRightButtonDownPoint = new Point(e.GetPosition(this).X, e.GetPosition(this).Y);
            mouseLabel.Content = "Right Click: " + _mMouseRightButtonDownPoint;
        }

        private void AddSubgraphToSelectedNodeCM_Click(object sender, RoutedEventArgs e)
        {
            CreateSubgraphFromSelectedNodes();
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
                EditMode.Background = (SolidColorBrush) new BrushConverter().ConvertFromString(defaultColor.ToString());
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
        }

        private void ViewerPanel_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _graphViewer.StartDrawingRubberLine(_mMouseLeftButtonDownPoint);
        }
    }
}